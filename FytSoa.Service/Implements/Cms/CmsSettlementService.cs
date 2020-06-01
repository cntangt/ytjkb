using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel.Cms;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsSettlementService : BaseService<CmsOrderOverview>, ICmsSettlementService
    {
        readonly IWxCloudService wx;
        readonly IConfiguration config;
        readonly ISysRoleService roleService;

        public CmsSettlementService(IWxCloudService wx, IConfiguration config, ISysRoleService roleService) : base(config)
        {
            this.wx = wx;
            this.config = config;
            this.roleService = roleService;
        }

        public async Task<ApiResult<string>> DailyJobAsync(DateTime day)
        {
            var result = new ApiResult<string>();

            var start = day.Date;
            var end = day.AddDays(1).Date;

            var shop_list = await Db.Queryable<ShopInfo, CmsMerchant>((shop, mch) => shop.out_sub_mch_id == mch.out_sub_mch_id)
                .Where((shop, mch) => mch.status)
                .Select((shop, mch) => new
                {
                    shop.out_shop_id,
                    mch.out_sub_mch_id,
                    mch.authen_key,
                }).ToListAsync();

            await Db.Deleteable<CmsOrderOverview>(p => p.business_date == start).ExecuteCommandAsync();

            var sps = Enum.GetNames(typeof(SubPayPlatform)).Select(p => Enum.Parse<SubPayPlatform>(p)).ToArray();

            foreach (var mch in shop_list)
            {
                var req = new QueryOrderOverviewRequest
                {
                    start_time = start,
                    end_time = end,
                    out_shop_id = mch.out_shop_id,
                    out_sub_mch_id = mch.out_sub_mch_id,
                    order_type = OrderType.全部单据,
                    sub_pay_platforms = sps,
                    AuthenKey = mch.authen_key
                };

                var res = await wx.QueryAsync(req);

                if (res.Status != 0 || res.Data == null)
                {
                    Logger.Default.ProcessError(res.Status, $"获取日结订单失败，res:{res.Description}，req:{JsonConvert.SerializeObject(req)}");

                    continue;
                }

                if (res.Data.overviews == null || res.Data.overviews.Count == 0)
                {
                    continue;
                }

                res.Data.overviews.ForEach(order =>
                {
                    order.out_shop_id = mch.out_shop_id;
                    order.out_sub_mch_id = mch.out_sub_mch_id;
                    order.business_date = start;
                });

                var count = await Db.Insertable(res.Data.overviews).ExecuteCommandAsync();

                if (count <= 0)
                {
                    Logger.Default.ProcessError(501, $"保存日结订单失败，mch:{mch.out_sub_mch_id}，day:{day}");
                }
            }

            result.data = DateTime.Now.ToString();
            result.message = "执行日结订单完成";
            result.statusCode = 200;
            result.success = true;

            return result;
        }

        public async Task<ApiResult<string>> MonthlyJobAsync(DateTime day)
        {
            var res = new ApiResult<string>();

            var count = await Db.Ado.UseStoredProcedure().GetIntAsync(config["monthly_proc"], new { });

            if (count == 0)
            {
                res.message = "执行月结任务失败";
                res.success = false;

                return res;
            }

            res.data = DateTime.Now.ToString();
            res.message = "执行月结任务完成";
            res.statusCode = 200;
            res.success = true;

            return res;
        }

        public async Task<IEnumerable<PlatformInfo>> GetPlatformInfoAsync(string admin_guid, DateTime start, DateTime end)
        {
            var data = await query(admin_guid, start, end)
                .GroupBy(daily => daily.sub_pay_platform)
                .Select(daily => new
                {
                    name = daily.sub_pay_platform,
                    trade_num = SqlFunc.AggregateSum(daily.success_count),
                    trade_total = SqlFunc.AggregateSum(daily.success_amount),
                    refund_total = SqlFunc.AggregateSum(daily.refund_settle_amount)
                }).ToListAsync();

            return typeof(SubPayPlatform).ToDropdown().Select(p =>
            {
                var info = data.FirstOrDefault(i => i.name.ToString() == p.Text);
                var d = new PlatformInfo
                {
                    Name = p.Text
                };
                if (info != null)
                {
                    d.Number = info.trade_num;
                    d.Total = info.trade_total - info.refund_total;
                }
                return d;
            });
        }

        public async Task<IEnumerable<TrendInfo>> GetTrendAsync(string admin_guid, DateTime start, DateTime end)
        {
            var data = await query(admin_guid, start, end)
                .GroupBy(daily => daily.business_date)
                .Select(daily => new TrendInfo
                {
                    Day = daily.business_date,
                    CountTrade = SqlFunc.AggregateSum(daily.success_count),
                    TotalTrade = SqlFunc.AggregateSum(daily.success_amount),
                    CountRefund = SqlFunc.AggregateSum(daily.refund_create_count),
                    TotalRefund = SqlFunc.AggregateSum(daily.refund_settle_amount)
                }).ToListAsync();

            return data;
        }

        private ISugarQueryable<CmsOrderOverview> query(string admin_guid, DateTime start, DateTime end)
        {
            var role_info = roleService.GetRoleByAdminGuid(admin_guid).Result;

            ISugarQueryable<CmsOrderOverview> query;

            if (role_info.isSystem)
            {
                query = Db.Queryable<CmsOrderOverview>();
            }
            else if (role_info.isAgent)
            {
                query = Db.Queryable<CmsOrderOverview, CmsMerchant>((daily, mch) => daily.out_sub_mch_id == mch.out_sub_mch_id)
                    .Where((daily, mch) => mch.agent_admin_guid == admin_guid).Select((daily, mch) => daily);
            }
            else if (role_info.isSubAdmin)
            {
                query = Db.Queryable<CmsOrderOverview, CmsMerchant>((daily, mch) => daily.out_sub_mch_id == mch.out_sub_mch_id)
                    .Where((daily, mch) => mch.admin_guid == admin_guid).Select((daily, mch) => daily);
            }
            else
            {
                query = Db.Queryable<CmsOrderOverview, AdminShopRel>((daily, rel) => daily.out_shop_id == rel.out_shop_id)
                    .Where((daily, rel) => rel.admin_guid == admin_guid).Select((daily, rel) => daily);
            }

            return query.Where(daily => daily.business_date >= start && daily.business_date <= end);
        }

        public async Task<ApiResult<Page<CmsOrderOverview>>> GetShopDailyReport(QueryOrderListRequest parm)
        {
            var res = new ApiResult<Page<CmsOrderOverview>>();

            var role_info = await roleService.GetRoleByAdminGuid(parm.createby);

            var query = Db.Queryable<CmsOrderOverview, AdminShopRel>((ds, rel) => new object[] { JoinType.Left, ds.out_shop_id == rel.out_shop_id });

            if (role_info.isSystem || role_info.isAgent || role_info.isSubAdmin)
            {
                //管理员不需要过滤
                //代理商没有该权限
                //商户管理员不过滤
            }
            else // 门店普通员工
            {
                query.Where((ds, rel) => rel.admin_guid == parm.createby);
            }

            query.WhereIF(!string.IsNullOrEmpty(parm.out_sub_mch_id), (ds, rel) => ds.out_sub_mch_id == parm.out_sub_mch_id)
                .WhereIF(!string.IsNullOrEmpty(parm.out_shop_id), (ds, rel) => ds.out_shop_id == parm.out_shop_id)
                .WhereIF(parm.sub_pay_platforms.Length > 0, (ds, rel) => parm.sub_pay_platforms.Contains(ds.sub_pay_platform))
                .WhereIF(parm.start_time != null, (ds, rel) => ds.business_date >= parm.start_time)
                .WhereIF(parm.end_time != null, (ds, rel) => ds.business_date <= parm.end_time);

            var data = await query.Select((ds, rel) => new CmsOrderOverview
            {
                business_date = ds.business_date,
                out_shop_id = ds.out_shop_id,
                success_count = SqlFunc.AggregateSum(ds.success_count),//交易笔数
                success_amount = SqlFunc.AggregateSum(ds.success_amount),//交易金额
                refund_create_count = SqlFunc.AggregateSum(ds.refund_create_count),//退货笔数
                order_refunded_amount = SqlFunc.AggregateSum(ds.order_refunded_amount),//订单已退金额
                discount_amount = SqlFunc.AggregateSum(ds.discount_amount),//优惠金额
                poundage = SqlFunc.AggregateSum(ds.poundage),//手续费
                income_amount = SqlFunc.AggregateSum(ds.income_amount)//入账金额
            }).GroupBy(ds => new { ds.business_date, ds.out_shop_id }).ToPageAsync(parm.page_num, parm.page_size);

            if (data.Items.Count > 0)
            {
                var ids = data.Items.Select(p => p.out_shop_id).ToArray();

                var shopInfos = await Db.Queryable<ShopInfo>().In(p => p.out_shop_id, ids).ToListAsync();

                data.Items.ForEach(shop =>
                {
                    var shopInfo = shopInfos.FirstOrDefault(m => shop.out_shop_id == m.out_shop_id);
                    if (shopInfo != null)
                    {
                        shop.shop_name = shopInfo.shop_name;
                        shop.erp_org = shopInfo.erp_org;
                    }

                    //应收金额
                    shop.pay_settle_amount = shop.success_amount - shop.order_refunded_amount;
                });
            }

            res.data = data;
            res.statusCode = (int)ApiEnum.Status;

            return res;
        }

        public async Task<ApiResult<Page<CmsOrderOverview>>> GetAgentTradeSummary(QueryOrderListRequest parm)
        {
            var res = new ApiResult<Page<CmsOrderOverview>>();

            var query = Db.Queryable<CmsOrderOverview, CmsMerchant>((ds, mch) =>
                new object[] { JoinType.Left, ds.out_sub_mch_id == mch.out_sub_mch_id });

            query.WhereIF(!string.IsNullOrEmpty(parm.createby), (ds, mch) => mch.agent_admin_guid == parm.createby)
                .WhereIF(!string.IsNullOrEmpty(parm.out_sub_mch_id), (ds, mch) => ds.out_sub_mch_id == parm.out_sub_mch_id)
                .WhereIF(parm.sub_pay_platforms.Length > 0, (ds, mch) => parm.sub_pay_platforms.Contains(ds.sub_pay_platform))
                .WhereIF(parm.start_time != null, (ds, mch) => ds.business_date >= parm.start_time)
                .WhereIF(parm.end_time != null, (ds, mch) => ds.business_date <= parm.end_time);

            var data = await query.Select((ds, mch) => new CmsOrderOverview
            {
                out_sub_mch_id = ds.out_sub_mch_id,
                success_count = SqlFunc.AggregateSum(ds.success_count),//交易笔数
                success_amount = SqlFunc.AggregateSum(ds.success_amount),//交易金额
                refund_create_count = SqlFunc.AggregateSum(ds.refund_create_count),//退货笔数
                order_refunded_amount = SqlFunc.AggregateSum(ds.order_refunded_amount),//退货金额
                refund_settle_amount = SqlFunc.AggregateSum(ds.refund_settle_amount),//交易净额
            }).GroupBy(ds => ds.out_sub_mch_id).ToPageAsync(parm.page_num, parm.page_size);

            if (data.Items.Count > 0)
            {
                var ids = data.Items.Select(p => p.out_sub_mch_id).ToArray();

                var shopInfos = await Db.Queryable<CmsMerchant>().In(p => p.out_sub_mch_id, ids).ToListAsync();

                data.Items.ForEach(shop =>
                {
                    var shopInfo = shopInfos.FirstOrDefault(m => shop.out_sub_mch_id == m.out_sub_mch_id);
                    if (shopInfo != null)
                    {
                        shop.shop_name = shopInfo.name;
                    }

                    //交易净额(同应收金额计算方式一样)
                    shop.pay_settle_amount = shop.success_amount - shop.order_refunded_amount;
                });
            }

            res.data = data;
            res.statusCode = (int)ApiEnum.Status;

            return res;
        }
    }
}
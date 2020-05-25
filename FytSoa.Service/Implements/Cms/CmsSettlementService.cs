﻿using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel.Cms;
using FytSoa.Service.DtoModel.Wx;
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
    public class CmsSettlementService : BaseService<CmsDailySettlement>, ICmsSettlementService
    {
        static readonly object daily_locker = new object();

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

            var mch_list = await Db.Queryable<CmsMerchant>().Where(p => p.status).ToListAsync();

            foreach (var mch in mch_list)
            {
                var order_list = new List<OrderDetails>();

                for (int i = 1; ; i++)
                {
                    var req = new QueryOrderListRequest
                    {
                        start_time = start,
                        end_time = end,
                        order_type = OrderType.全部单据,
                        page_num = i,
                        page_size = 50,
                        out_sub_mch_id = mch.out_sub_mch_id,
                        AuthenKey = mch.authen_key,
                    };

                    var res = await wx.QueryAsync(req);

                    if (res.Status != 0 || res.Data == null)
                    {
                        Logger.Default.ProcessError(res.Status, $"获取日结订单失败，res:{res.Description}，req:{JsonConvert.SerializeObject(req)}");

                        break;
                    }

                    if (res.Data.order_details == null || res.Data.order_details.Length == 0)
                    {
                        break;
                    }

                    order_list.AddRange(res.Data.order_details);
                }

                var order_groups = order_list.Where(p => p.order != null).Select(p => p.order).GroupBy(p => new
                {
                    p.out_shop_id,
                    p.staff_id,
                    p.device_id,
                    p.sub_pay_platform,
                    p.out_sub_mch_id,
                    p.shop_name
                });
                var refund_groups = order_list.Where(p => p.refund_order != null).Select(p => p.refund_order).GroupBy(p => new
                {
                    p.out_shop_id,
                    p.staff_id,
                    p.device_id,
                    p.sub_pay_platform
                });

                var daily_list = new List<CmsDailySettlement>();

                foreach (var og in order_groups)
                {
                    var daily = new CmsDailySettlement
                    {
                        create_time = DateTime.Now,
                        out_mch_id = config["out_mch_id"],
                        business_date = start,
                        out_shop_id = og.Key.out_shop_id,
                        sub_pay_platform = og.Key.sub_pay_platform,
                        staff_id = og.Key.staff_id,
                        device_id = og.Key.device_id,
                        out_sub_mch_id = og.Key.out_sub_mch_id,
                        shop_name = og.Key.shop_name,
                        erp_org = og.Key.shop_name.GetShopErpOrg(),
                        count_trade = og.Count(),
                        total_trade_fee = og.Sum(p => p.total_fee),
                        settlement_fee = og.Sum(p => p.remaining_settlement_fee),
                        discount_fee = og.Sum(p => p.discount_fee),
                        poundage_fee = og.Sum(p => p.poundage),
                        receivable_fee = og.Sum(p => p.settlement_total_fee),

                        count_refund = 0,
                        total_refund_fee = 0
                    };

                    var rg = refund_groups.FirstOrDefault(p =>
                    p.Key.sub_pay_platform == og.Key.sub_pay_platform &&
                    p.Key.out_shop_id == og.Key.out_shop_id &&
                    p.Key.staff_id == og.Key.staff_id &&
                    p.Key.device_id == og.Key.device_id);

                    if (rg != null)
                    {
                        daily.count_refund = rg.Count();
                        daily.total_refund_fee = rg.Sum(p => p.refund_fee);
                    }

                    daily_list.Add(daily);
                }

                if (daily_list.Count > 0)
                {
                    lock (daily_locker)
                    {
                        Db.Deleteable<CmsDailySettlement>(p => p.business_date == start).ExecuteCommand();

                        var count = Db.Insertable(daily_list).ExecuteCommand();

                        if (count <= 0)
                        {
                            Logger.Default.ProcessError(501, $"保存日结订单失败，mch:{mch.out_sub_mch_id}，day:{day}");
                        }
                    }
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
            var role_info = await roleService.GetRoleByAdminGuid(admin_guid);

            ISugarQueryable<CmsDailySettlement> query;

            if (role_info.isSystem)
            {
                query = Db.Queryable<CmsDailySettlement>();
            }
            else if (role_info.isAgent)
            {
                query = Db.Queryable<CmsDailySettlement, CmsMerchant>((daily, mch) => daily.out_sub_mch_id == mch.out_sub_mch_id)
                    .Where((daily, mch) => mch.agent_admin_guid == admin_guid).Select((daily, mch) => daily);
            }
            else if (role_info.isSubAdmin)
            {
                query = Db.Queryable<CmsDailySettlement, CmsMerchant>((daily, mch) => daily.out_sub_mch_id == mch.out_sub_mch_id)
                    .Where((daily, mch) => mch.admin_guid == admin_guid).Select((daily, mch) => daily);
            }
            else
            {
                query = Db.Queryable<CmsDailySettlement, AdminShopRel>((daily, rel) => daily.out_shop_id == rel.out_shop_id)
                    .Where((daily, rel) => rel.admin_guid == admin_guid).Select((daily, rel) => daily);
            }

            var data = await query
                .Where(daily => daily.business_date >= start && daily.business_date <= end)
                .GroupBy(daily => daily.sub_pay_platform)
                .Select(daily => new
                {
                    name = daily.sub_pay_platform,
                    trade_num = SqlFunc.AggregateSum(daily.count_trade),
                    trade_total = SqlFunc.AggregateSum(daily.total_trade_fee),
                    refund_total = SqlFunc.AggregateSum(daily.total_refund_fee)
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
    }
}
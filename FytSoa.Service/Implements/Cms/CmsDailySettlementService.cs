using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsDailySettlementService : BaseService<CmsDailySettlement>, ICmsDailySettlementService
    {
        static readonly object daily_locker = new object();

        readonly IWxCloudService wx;
        readonly IConfiguration config;

        public CmsDailySettlementService(IWxCloudService wx, IConfiguration config) : base(config)
        {
            this.wx = wx;
            this.config = config;
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
    }
}
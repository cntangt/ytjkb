using FytSoa.Common;
using FytSoa.Core.Model.Wx;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SyncController : Controller
    {
        readonly IWxCloudService wx;
        readonly ICmsMerchantService merchantService;
        readonly ICmsDailySettlementService cmsDaily;
        readonly IConfiguration config;

        public SyncController(IWxCloudService wx, ICmsMerchantService merchantService, ICmsDailySettlementService cmsDaily, IConfiguration config)
        {
            this.wx = wx;
            this.merchantService = merchantService;
            this.cmsDaily = cmsDaily;
            this.config = config;
        }

        [HttpPost]
        public async Task<PageResult<IEnumerable<TradeOrder>>> Trade(QueryOrderListRequest req)
        {
            var res = new PageResult<IEnumerable<TradeOrder>>();

            if (req.start_time == null)
            {
                req.start_time = DateTime.Now.Date;
            }
            if (req.end_time == null)
            {
                req.end_time = DateTime.Now.AddDays(1).Date;
            }

            req.order_type = OrderType.支付订单;

            var mch = await merchantService.GetModelAsync(p => p.out_sub_mch_id == req.out_sub_mch_id);
            if (mch == null || mch.data == null || mch.data.id == 0)
            {
                res.Code = ApiEnum.Error;
                res.Msg = "请选择子商户查询";
                return res;
            }

            req.AuthenKey = mch.data.authen_key;

            var data = await wx.QueryAsync(req);

            if (data.Status != 0)
            {
                res.Msg = data.Description;
                res.Code = ApiEnum.Error;

                return res;
            }

            if (data.Data == null)
            {
                res.Msg = "查询结果为空";
                res.Code = ApiEnum.Error;

                return res;
            }

            res.Msg = "success";
            res.Count = data.Data.total_count;

            if (data.Data.order_details != null)
            {
                res.Data = data.Data.order_details.Select(p => p.order);
            }

            return res;
        }

        public async Task<IActionResult> ShopInfo(int id) => Ok(await wx.SyncShopInfo(id));

        [HttpPost]
        public async Task<PageResult<IEnumerable<RefundOrder>>> Refund(QueryOrderListRequest req)
        {
            var res = new PageResult<IEnumerable<RefundOrder>>();

            if (req.start_time == null)
            {
                req.start_time = DateTime.Now.Date;
            }
            if (req.end_time == null)
            {
                req.end_time = DateTime.Now.AddDays(1).Date;
            }

            req.order_type = OrderType.退款单;

            var mch = await merchantService.GetModelAsync(p => p.out_sub_mch_id == req.out_sub_mch_id);
            if (mch == null || mch.data == null || mch.data.id == 0)
            {
                res.Code = ApiEnum.Error;
                res.Msg = "请选择子商户查询";
                return res;
            }

            req.AuthenKey = mch.data.authen_key;

            var data = await wx.QueryAsync(req);

            if (data.Status != 0)
            {
                res.Msg = data.Description;
                res.Code = ApiEnum.Error;

                return res;
            }

            if (data.Data == null)
            {
                res.Msg = "查询结果为空";
                res.Code = ApiEnum.Error;

                return res;
            }

            res.Msg = "success";
            res.Count = data.Data.total_count;

            if (data.Data.order_details != null)
            {
                res.Data = data.Data.order_details.Select(p => p.refund_order);
            }

            return res;
        }

        [HttpPost]
        public async Task<ApiResult<RefundResponse>> DoRefund(DoRefundRequest req_data)
        {
            var res = new ApiResult<RefundResponse>();

            var mch = await merchantService.GetModelAsync(p => p.out_sub_mch_id == req_data.out_sub_mch_id);
            if (mch == null || mch.data == null || mch.data.id == 0)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = "请选择子商户查询";
                return res;
            }
            var req = new RefundRequest
            {
                refund_content = new Refund_Content
                {
                    out_trade_no = req_data.out_trade_no,
                    out_refund_no = $"{mch.data.order_perfix}{DateTime.Now:yyyyMMddHHmmssffffff}",
                    refund_fee = (int)(req_data.refund_fee * 100),
                    refund_fee_type = req_data.refund_fee_type,
                    refund_reason = req_data.refund_reason,
                    total_fee = (int)(req_data.total_fee * 100)
                },
                order_client = new Order_Client
                {
                    terminal_type = TerminalType.Windows,
                    sdk_version = "1.0",
                    spbill_create_ip = HttpContext.GetIP()
                },
                pay_mch_key = new Pay_Mch_Key
                {
                    pay_platform = PayPlatform.微信支付,
                    out_mch_id = config["out_mch_id"],
                    out_sub_mch_id = req_data.out_sub_mch_id,
                    out_shop_id = req_data.out_shop_id,
                    is_sub_mch_admin = true,
                },
                AuthenKey = mch.data.authen_key
            };

            var result = await wx.QueryAsync(req);

            res.message = result.Description;
            res.statusCode = result.Status;
            res.data = result.Data;

            return res;
        }

        public Task<ApiResult<string>> DailyJob(DateTime? day) => cmsDaily.DailyJobAsync(day ?? DateTime.Now.AddDays(-1));
    }
}
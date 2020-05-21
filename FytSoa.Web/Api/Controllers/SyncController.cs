using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces.Cms;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.AspNetCore.Mvc;
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
        public SyncController(IWxCloudService wx, ICmsMerchantService merchantService)
        {
            this.wx = wx;
            this.merchantService = merchantService;
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
                res.Code = 500;
                res.Msg = "请选择子商户查询";
                return res;
            }

            req.AuthenKey = mch.data.authen_key;

            var data = await wx.QueryAsync(req);

            res.Msg = "success";
            res.Count = data.Data.total_count;
            if (data.Data.order_details != null)
            {
                res.Data = data.Data.order_details.Select(p => p.order);
            }

            return res;
        }

        public Task<WxResponse<QueryOrderListResponse>> OrderList()
        {
            var req = new QueryOrderListRequest
            {
                order_type = OrderType.全部单据,
                start_time = DateTime.Now.Date.AddDays(-3),
                end_time = DateTime.Now.Date,
                page_num = 1,
                page_size = 100,
                out_sub_mch_id = "sz01ELTR281OFpmdAp6J",
                AuthenKey = "lSCp1M5grGWFD7rJzaZaqixsvOhORp2P"
            };

            return wx.QueryAsync(req);
        }

        public async Task<IActionResult> ShopInfo(int id) => Ok(await wx.SyncShopInfo(id));

        public Task<WxResponse<RefundResponse>> Refund()
        {
            var req = new RefundRequest
            {
                refund_content = new Refund_Content
                {
                    out_trade_no = "sz0100lmnx12020051312422304jvek8",
                    out_refund_no = $"sz0100lmnx{DateTime.Now:yyyyMMddHHmmssffffff}",
                    refund_fee = 1,
                    refund_fee_type = "CNY",//需要和来源订单保持一至
                    refund_reason = "就是任性",
                    total_fee = 1
                },
                order_client = new Order_Client
                {
                    terminal_type = TerminalType.Windows,
                    sdk_version = "1.0",
                    spbill_create_ip = "171.221.202.167"
                },
                pay_mch_key = new Pay_Mch_Key
                {
                    //pay_platform = PayPlatform.默认,
                    out_mch_id = "sz013NzuonO6CMJd0rCB",
                    out_sub_mch_id = "sz01ELTR281OFpmdAp6J",
                    out_shop_id = "sz01qyoPJmd3j1hWmul4",
                    is_sub_mch_admin = true,
                    //authorization_name=
                },
                AuthenKey = "lSCp1M5grGWFD7rJzaZaqixsvOhORp2P",
            };

            return wx.QueryAsync(req);
        }

    }
}
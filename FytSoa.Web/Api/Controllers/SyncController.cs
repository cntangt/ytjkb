using FytSoa.Common;
using FytSoa.Core.Model.Cms;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SyncController : Controller
    {
        readonly IWxCloudService wx;
        readonly ICmsMerchantService merchantService;
        readonly ICmsSettlementService cmsDaily;
        readonly IConfiguration config;
        readonly IHttpClientFactory factory;

        public SyncController(IWxCloudService wx, ICmsMerchantService merchantService, ICmsSettlementService cmsDaily, IConfiguration config, IHttpClientFactory factory)
        {
            this.wx = wx;
            this.merchantService = merchantService;
            this.cmsDaily = cmsDaily;
            this.config = config;
            this.factory = factory;
        }

        public async Task<IActionResult> TradeExport(string q)
        {
            var req = JsonConvert.DeserializeObject<QueryOrderListRequest>(q);

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
                return Content("请选择导出商户");
            }

            req.AuthenKey = mch.data.authen_key;

            req.page_size = 50;

            var list = new List<TradeOrder>();

            for (req.page_num = 1; ; req.page_num++)
            {
                var d = await wx.QueryAsync(req);

                if (d.Status > 0 || d.Data == null || d.Data.order_details == null || d.Data.order_details.Length == 0)
                {
                    break;
                }

                list.AddRange(d.Data.order_details.Where(p => p.order != null).Select(p => p.order));
            }

            if (list.Count == 0)
            {
                return Content("没有数据");
            }

            var data = await list.ExcelBuilder()
                  .Col("支付渠道", p => p.sub_pay_platform)
                  .Col("流水单号", p => p.out_trade_no)
                  .Col("商户名称", p => mch.data.name)
                  .Col("门店名称", p => p.shop_name)
                  .Col("订单金额", p => p.total_fee / 100M, "¥#,##0.00", FuncType.SUM)
                  .Col("支付类型", p => p.trade_type)
                  .Col("支付状态", p => p.trade_state)
                  .Col("交易时间", p => p.time_end.ToLocalTime(), "yyyy年MM月dd日 HH:mm:ss")
                  .Col("设备号", p => p.device_id)
                  .Col("店员名称", p => p.staff_name)
                  .Col("手续费", p => p.poundage / 100M, "¥#,##0.00", FuncType.SUM)
                  .Write("交易订单");

            return File(
                data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"（交易订单）{mch.data.name}{req.start_time:yyyyMMddHHmmss}-{req.end_time:yyyyMMddHHmmss}.xlsx");
        }

        public async Task<IActionResult> RefundExport(string q)
        {
            var req = JsonConvert.DeserializeObject<QueryOrderListRequest>(q);

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
                return Content("请选择导出商户");
            }

            req.AuthenKey = mch.data.authen_key;

            req.page_size = 50;

            var list = new List<RefundOrder>();

            for (req.page_num = 1; ; req.page_num++)
            {
                var d = await wx.QueryAsync(req);

                if (d.Status > 0 || d.Data == null || d.Data.order_details == null || d.Data.order_details.Length == 0)
                {
                    break;
                }

                list.AddRange(d.Data.order_details.Where(p => p.refund_order != null).Select(p => p.refund_order));
            }

            if (list.Count == 0)
            {
                return Content("没有数据");
            }

            var data = await list.Write("退款订单",
                p => new EC("支付渠道", p.sub_pay_platform),
                p => new EC("交易单号", p.out_trade_no),
                p => new EC("退款单号", p.out_refund_no),
                p => new EC("商户名称", mch.data.name),
                p => new EC("门店名称", p.shop_name),
                p => new EC("订单金额", p.total_fee / 100M, "¥#,##0.00", sum: true),
                p => new EC("退款金额", p.refund_fee / 100M, "¥#,##0.00", sum: true),
                p => new EC("支付类型", p.trade_type),
                p => new EC("退款状态", p.refund_state),
                p => new EC("退款时间", p.last_update_time.ToLocalTime(), "yyyy年MM月dd日 HH:mm:ss"),
                p => new EC("设备号", p.device_id),
                p => new EC("店员名称", p.staff_name));

            return File(
                data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"（退款订单）{mch.data.name}{req.start_time:yyyyMMddHHmmss}-{req.end_time:yyyyMMddHHmmss}.xlsx");
        }

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
                res.Msg = "请选择商户查询";
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
                res.Msg = "请选择商户查询";
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
                res.message = "请选择商户查询";
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
                    total_fee = (int)(req_data.total_fee)
                },
                order_client = new Order_Client
                {
                    terminal_type = TerminalType.Windows,
                    sdk_version = "1.0",
                    spbill_create_ip = await this.ServerIP() //HttpContext.GetIP()
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

        public Task<ApiResult<string>> MonthlyJob() => cmsDaily.MonthlyJobAsync(DateTime.Now);

        [HttpPost]
        public async Task<PageResult<IEnumerable<CmsOrderOverview>>> DailyReport(QueryOrderListRequest req)
        {
            var res = new PageResult<IEnumerable<CmsOrderOverview>>();

            var mch = await merchantService.GetModelAsync(p => p.out_sub_mch_id == req.out_sub_mch_id);
            if (mch == null || mch.data == null || mch.data.id == 0)
            {
                res.Code = ApiEnum.Error;
                res.Msg = "请选择商户查询";
                return res;
            }

            req.createby = await HttpContext.LoginUserId();

            var data = await cmsDaily.GetShopDailyReport(req);

            if (data.statusCode != 200)
            {
                res.Msg = data.message;
                res.Code = ApiEnum.Error;

                return res;
            }

            if (data.data.Items == null)
            {
                res.Msg = "查询结果为空";
                res.Code = ApiEnum.Error;

                return res;
            }

            res.Msg = "success";
            res.Count = data.data.TotalItems;
            res.Data = data.data.Items;

            return res;
        }

        public async Task<IActionResult> DailyReportExport(string q)
        {
            var req = JsonConvert.DeserializeObject<QueryOrderListRequest>(q);

            var mch = await merchantService.GetModelAsync(p => p.out_sub_mch_id == req.out_sub_mch_id);
            if (mch == null || mch.data == null || mch.data.id == 0)
            {
                return Content("请选择导出商户");
            }

            req.createby = await HttpContext.LoginUserId();
            req.page_size = int.MaxValue;

            var res = await cmsDaily.GetShopDailyReport(req);

            if (res.data.Items.Count == 0)
            {
                return Content("没有数据");
            }

            var data = await res.data.Items.Write("门店日报表",
                p => new EC("门店名称", p.shop_name),
                p => new EC("ERP机构", p.erp_org),
                p => new EC("交易笔数", p.success_count, sum: true),
                p => new EC("交易金额", p.success_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("退款笔数", p.refund_create_count, sum: true),
                p => new EC("退款金额", p.refund_create_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("应收金额", p.pay_settle_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("优惠金额", p.discount_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("手续费", p.poundage / 100M, "¥#,##0.00", sum: true),
                p => new EC("入账金额", p.income_amount / 100M, "¥#,##0.00", sum: true));

            return File(
                data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "门店日报表.xlsx"
            );
        }

        [HttpPost]
        public async Task<IActionResult> TradeSummary(QueryOrderListRequest req)
        {
            if (!await HttpContext.IsSystem())
            {
                req.createby = await HttpContext.LoginUserId();
            }

            var res = await cmsDaily.GetAgentTradeSummary(req);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        public async Task<IActionResult> TradeSummaryExport(string q)
        {
            var req = JsonConvert.DeserializeObject<QueryOrderListRequest>(q);

            req.page_size = int.MaxValue;

            if (!await HttpContext.IsSystem())
            {
                req.createby = await HttpContext.LoginUserId();
            }

            var res = await cmsDaily.GetAgentTradeSummary(req);

            if (res.data.Items.Count == 0)
            {
                return Content("没有数据");
            }

            var data = await res.data.Items.Write("代理商交易统计",
                p => new EC("商户名称", p.shop_name),
                p => new EC("交易笔数", p.success_count, sum: true),
                p => new EC("交易金额", p.success_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("退款笔数", p.refund_create_count, sum: true),
                p => new EC("退款金额", p.refund_create_amount / 100M, "¥#,##0.00", sum: true),
                p => new EC("交易净额", p.pay_settle_amount / 100M, "¥#,##0.00", sum: true));

            return File(
                data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "代理商交易统计.xlsx"
            );
        }

        private async Task<string> ServerIP()
        {
            var ip = string.Empty;

            ip = config.GetValue("ServerIP", ip);

            if (string.IsNullOrEmpty(ip))
            {
                var http = factory.CreateClient();
                var msg = await http.GetAsync("http://whatismyip.akamai.com/");
                ip = await msg.Content.ReadAsStringAsync();
            }

            return ip;
        }
    }
}
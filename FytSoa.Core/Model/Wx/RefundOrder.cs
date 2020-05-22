using FytSoa.Service.DtoModel.Wx;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_refund_order")]
    public class RefundOrder
    {
        public int id { get; set; }
        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
        public string out_shop_id { get; set; }
        public string shop_name { get; set; }
        public string staff_id { get; set; }
        public string staff_name { get; set; }
        public string device_id { get; set; }
        public string device_name { get; set; }
        public string qrcode_id { get; set; }
        public string qrcode_name { get; set; }
        public SourceType source_type { get; set; }
        public string out_card_id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SubPayPlatform sub_pay_platform { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType trade_type { get; set; }
        public string out_trade_no { get; set; }
        public long total_fee { get; set; }
        public string out_refund_no { get; set; }
        public string refund_id { get; set; }
        public long refund_fee { get; set; }
        public long settlement_total_fee { get; set; }
        public string refund_fee_type { get; set; }
        public double poundage { get; set; }
        public long income_fee { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        public DateTime refund_start_processing_time { get; set; }
        public DateTime pay_last_update_time { get; set; }
        public string refund_reason { get; set; }
        public string client_refund_flow_id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public WxpayRefundOrderState wxpay_refund_state { get; set; }
        public long cash_refund_fee { get; set; }
        public string cash_refund_fee_type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AlipayRefundOrderState alipay_refund_state { get; set; }
        public long credits { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RecordRefundOrderState record_refund_state { get; set; }
        public bool is_deposit_mode { get; set; }
        public string out_freeze_no { get; set; }
        public long freeze_fee { get; set; }
    }
}

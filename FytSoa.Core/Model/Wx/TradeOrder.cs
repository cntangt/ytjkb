using FytSoa.Service.DtoModel.Wx;
using System;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_trade_order")]
    public class TradeOrder
    {
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
        public PayPlatform sub_pay_platform { get; set; }
        public TradeType trade_type { get; set; }
        public string out_trade_no { get; set; }
        public string transaction_id { get; set; }
        public long total_fee { get; set; }
        public FeeType fee_type { get; set; }
        public long cash_fee { get; set; }
        public FeeType cash_fee_type { get; set; }
        public long settlement_total_fee { get; set; }
        public long discount_fee { get; set; }
        public long recharge_bonus_fee { get; set; }
        public bool is_card_recharge { get; set; }
        public double poundage { get; set; }
        //public string coupon_infos { get; set; }
        public long remaining_settlement_fee { get; set; }
        public long income_fee { get; set; }
        public DateTime time_end { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        public string body { get; set; }
        public string detail { get; set; }
        public string client_flow_id { get; set; }
        public string remark { get; set; }
        public long refunded_fee { get; set; }
        public DateTime refund_create_time { get; set; }
        public DateTime refund_time { get; set; }
        public long refunded_settlement_fee { get; set; }
        public WxpayOrderState wxpay_current_trade_state { get; set; }
        public string attach { get; set; }
        public string bank_type { get; set; }
        public string goods_tag { get; set; }
        public string scene_info { get; set; }
        public AlipayRefundOrderState alipay_current_trade_state { get; set; }
        public long discountable_amount { get; set; }
        public long undiscountable_amount { get; set; }
        public string membership_number { get; set; }
        public string uid { get; set; }
        public long credits { get; set; }
        public RecordOrderState record_current_trade_state { get; set; }
        public string out_freeze_no { get; set; }
        public long freeze_fee { get; set; }
        public bool is_deposit_mode { get; set; }
        public bool is_confirm_unfreeze { get; set; }
        public long manual_unfreeze_fee { get; set; }
        public string freeze_id { get; set; }
    }
}

using System;

namespace FytSoa.Service.DtoModel.Wx
{
    public class QueryOrderListResponse
    {
        public int total_count { get; set; }
        public Order_Details[] order_details { get; set; }
        public string nonce_str { get; set; }
    }

    public class Order_Details
    {
        public Order order { get; set; }
        public Refund_Order refund_order { get; set; }
    }

    public class Order
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
        public long remaining_settlement_fee { get; set; }
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
        public bool is_deposit_mode { get; set; }
    }

    public class Refund_Order
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
        public SourceType source_type { get; set; }
        public PayPlatform sub_pay_platform { get; set; }
        public TradeType trade_type { get; set; }
        public string out_trade_no { get; set; }
        public long total_fee { get; set; }
        public string out_refund_no { get; set; }
        public string refund_id { get; set; }
        public long refund_fee { get; set; }
        public long settlement_refund_fee { get; set; }
        public string refund_fee_type { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        public DateTime refund_start_processing_time { get; set; }
        public DateTime pay_last_update_time { get; set; }
        public string refund_reason { get; set; }
        public string client_refund_flow_id { get; set; }
        public AlipayRefundOrderState alipay_refund_state { get; set; }
        public bool is_deposit_mode { get; set; }
    }

}

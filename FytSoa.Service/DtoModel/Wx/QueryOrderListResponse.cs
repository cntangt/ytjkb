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
        public int source_type { get; set; }
        public int sub_pay_platform { get; set; }
        public int trade_type { get; set; }
        public string out_trade_no { get; set; }
        public string transaction_id { get; set; }
        public int total_fee { get; set; }
        public string fee_type { get; set; }
        public int cash_fee { get; set; }
        public string cash_fee_type { get; set; }
        public int settlement_total_fee { get; set; }
        public int discount_fee { get; set; }
        public int recharge_bonus_fee { get; set; }
        public bool is_card_recharge { get; set; }
        public int remaining_settlement_fee { get; set; }
        public DateTime time_end { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        public string body { get; set; }
        public string detail { get; set; }
        public string client_flow_id { get; set; }
        public string remark { get; set; }
        public int refunded_fee { get; set; }
        public DateTime refund_create_time { get; set; }
        public DateTime refund_time { get; set; }
        public int refunded_settlement_fee { get; set; }
        public int wxpay_current_trade_state { get; set; }
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
        public int source_type { get; set; }
        public int sub_pay_platform { get; set; }
        public int trade_type { get; set; }
        public string out_trade_no { get; set; }
        public int total_fee { get; set; }
        public string out_refund_no { get; set; }
        public string refund_id { get; set; }
        public int refund_fee { get; set; }
        public int settlement_refund_fee { get; set; }
        public string refund_fee_type { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        public DateTime refund_start_processing_time { get; set; }
        public DateTime pay_last_update_time { get; set; }
        public string refund_reason { get; set; }
        public string client_refund_flow_id { get; set; }
        public int alipay_refund_state { get; set; }
        public bool is_deposit_mode { get; set; }
    }

}

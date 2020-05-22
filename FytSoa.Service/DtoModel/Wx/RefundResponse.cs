namespace FytSoa.Service.DtoModel.Wx
{
    public class RefundResponse
    {
        public Pay_Mch_Key pay_mch_key { get; set; }
        public Refund_Order_Content[] refund_order_content { get; set; }
        public string nonce_str { get; set; }
        public Order_Client[] order_client { get; set; }

        public class Pay_Mch_Key
        {
            public int pay_platform { get; set; }
            public string out_mch_id { get; set; }
            public string out_sub_mch_id { get; set; }
            public string out_shop_id { get; set; }
            public int sub_pay_platform { get; set; }
            public Wxpay_Pay_Mch_Key_Ext wxpay_pay_mch_key_ext { get; set; }
        }

        public class Wxpay_Pay_Mch_Key_Ext
        {
            public string app_id { get; set; }
            public string sub_app_id { get; set; }
        }

        public class Refund_Order_Content
        {
            public string out_refund_no { get; set; }
            public string refund_id { get; set; }
            public int trade_type { get; set; }
            public string out_trade_no { get; set; }
            public string nonce_str { get; set; }
            public int create_time { get; set; }
            public int last_update_time { get; set; }
            public bool is_transforming { get; set; }
            public int refund_success_time { get; set; }
            public string refund_reason { get; set; }
            public int total_fee { get; set; }
            public int refund_fee { get; set; }
            public string refund_fee_type { get; set; }
            public int exchange_rate { get; set; }
            public string client_refund_flow_id { get; set; }
            public int settlement_refund_fee { get; set; }
            public int trade_state { get; set; }
            public Wxpay_Refund_Order_Content_Ext wxpay_refund_order_content_ext { get; set; }
        }

        public class Wxpay_Refund_Order_Content_Ext
        {
            public int state { get; set; }
            public int cash_refund_fee { get; set; }
            public int coupon_refund_fee { get; set; }
            public int coupon_refund_count { get; set; }
            public string refund_account { get; set; }
            public string refund_channel { get; set; }
            public string refund_recv_account { get; set; }
        }

        public class Order_Client
        {
            public string shop_id { get; set; }
            public string device_id { get; set; }
            public string staff_id { get; set; }
            public string machine_no { get; set; }
            public int terminal_type { get; set; }
            public string sdk_version { get; set; }
            public string spbill_create_ip { get; set; }
            public int sub_terminal_type { get; set; }
        }
    }
}
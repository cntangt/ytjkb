namespace FytSoa.Service.DtoModel.Wx
{
    public class RefundRequest : WxRequest<RefundResponse>
    {
        public override string ApiName => "query_refund_order";
        public override string DataName => "query_refund_order";

        public string out_trade_no { get; set; }
        public string out_refund_no { get; set; }
        public Pay_Mch_Key pay_mch_key { get; set; }
        public Order_Client order_client { get; set; }

        public class Pay_Mch_Key
        {
            public int pay_platform { get; set; }
            /// <summary>
            /// *
            /// </summary>
            public int sub_pay_platform { get; set; }
            public string out_shop_id { get; set; }
            public string out_sub_mch_id { get; set; }
            public string out_mch_id { get; set; }
        }

        public class Order_Client
        {
            public string staff_id { get; set; }
            public string machine_no { get; set; }
            public TerminalType terminal_type { get; set; }
            public string sdk_version { get; set; }
            /// <summary>
            /// *
            /// </summary>
            public string device_id { get; set; }
            public string spbill_create_ip { get; set; }
        }
    }
}
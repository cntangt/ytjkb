namespace FytSoa.Service.DtoModel.Wx
{
    public class DoRefundRequest
    {
        public decimal refund_fee { get; set; }
        public string refund_reason { get; set; }
        public string out_trade_no { get; set; }
        public decimal total_fee { get; set; }
        public PayPlatform sub_pay_platform { get; set; }
        public string out_sub_mch_id { get; set; }
        public string out_shop_id { get; set; }
        public string refund_fee_type { get; set; }
    }
}

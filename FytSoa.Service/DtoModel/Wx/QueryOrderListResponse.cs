using FytSoa.Core.Model.Wx;

namespace FytSoa.Service.DtoModel.Wx
{
    public class QueryOrderListResponse
    {
        public int total_count { get; set; }
        public OrderDetails[] order_details { get; set; }
        public string nonce_str { get; set; }
    }

    public class OrderDetails
    {
        public TradeOrder order { get; set; }
        public RefundOrder refund_order { get; set; }
    }
}

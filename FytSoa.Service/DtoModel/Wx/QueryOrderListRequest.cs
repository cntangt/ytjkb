using System;

namespace FytSoa.Service.DtoModel.Wx
{
    public class QueryOrderListRequest : WxRequest<QueryOrderListResponse>
    {
        public override string ApiName => "query_order_list";
        public override string DataName => "query_order_list";

        public string out_sub_mch_id { get; set; }
        public OrderType order_type { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public int page_num { get; set; }
        public int page_size { get; set; }
    }
}

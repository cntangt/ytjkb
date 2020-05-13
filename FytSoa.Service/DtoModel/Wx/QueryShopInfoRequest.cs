namespace FytSoa.Service.DtoModel.Wx
{
    public class QueryShopInfoRequest : WxRequest<QueryShopInfoResponse>
    {
        public override string ApiName => "query_sub_mch_shop_info";
        public override string DataName => "query_shop_info";

        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
        public int page_num { get; set; }
        public int page_size { get; set; }
    }
}


namespace FytSoa.Service.DtoModel.Wx
{
    public class GoodsDto
    {
        public decimal? cost_price { get; set; }
        public string receipt_id { get; set; }
        public GoodsDetail[] goods_detail { get; set; }
    }

    public class GoodsDetail
    {
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public decimal? quantity { get; set; }
        public decimal? price { get; set; }
    }
}

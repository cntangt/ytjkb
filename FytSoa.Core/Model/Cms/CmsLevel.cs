namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Level")]
    public class CmsLevel
    {
        public int Id {get;set;}
        public string Name { get; set; }
        public decimal Alipay { get; set; }
        public decimal WxPay { get; set; }
        public decimal OtherPay { get; set; }
        public bool Status { get; set; }
    }
}

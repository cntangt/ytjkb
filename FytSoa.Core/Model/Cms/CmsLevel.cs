namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Level")]
    public class CmsLevel
    {
        public int Id {get;set;}
        public string Name { get; set; }
        public decimal Alipay { get; set; }
        public decimal Wxpay { get; set; }
        public decimal Otherpay { get; set; }
        public bool Status { get; set; }
    }
}

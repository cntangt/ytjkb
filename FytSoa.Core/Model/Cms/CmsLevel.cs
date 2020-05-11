namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Level")]
    public class CmsLevel
    {
        public int Id {get;set;}
        public string Name { get; set; }
        public int Alipay { get; set; }
        public int WxPay { get; set; }
        public int OtherPay { get; set; }
        public bool Status { get; set; }
    }
}

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Agent")]
    public class CmsAgent
    {
        public int id { get; set; }
        public string admin_guid { get; set; }
        public string name { get; set; }
        public int level_id { get; set; }
        public string tel { get; set; }
        public string business_area { get; set; }
        public int settle_type { get; set; }
        public string account_name { get; set; }
        public string account_no { get; set; }
        public string account_info { get; set; }
        public decimal wxpay { get; set; }
        public decimal alipay { get; set; }
        public decimal otherpay { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
    }
}

using System;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("cms_merchant")]
    public class CmsMerchant
    {
        public int id { get; set; }
        public string admin_guid { get; set; }
        public string agent_admin_guid { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public string appid { get; set; }
        public string out_mch_id { get; set; }
        public string sub_out_mch_id { get; set; }
        public string order_perfix { get; set; }
        public string authen_key { get; set; }
        public decimal wxpay { get; set; }
        public decimal alipay { get; set; }
        public decimal otherpay { get; set; }
        public decimal agent_alipay { get; set; }
        public decimal agent_wxpay { get; set; }
        public decimal agent_otherpay { get; set; }
        public DateTime create_time { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string admin_name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string agent_admin_name { get; set; }
    }
}

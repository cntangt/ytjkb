using Newtonsoft.Json;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace FytSoa.Core.Model.Sys
{
    [MySugarTable("admin_shop_rel")]
    public class AdminShopRel
    {
        [SugarColumn(IsIdentity = true)]
        public int id { get; set; }
        public string admin_guid { get; set; }
        public string out_shop_id { get; set; }
        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
    }
}
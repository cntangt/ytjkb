﻿using SqlSugar;
using System;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_shopinfo")]
    public class ShopInfo
    {
        public int id { get; set; }
        public string shop_id { get; set; }
        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
        public string shop_name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string coordinate_type { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string height { get; set; }
        public string phone { get; set; }
        public string out_shop_id { get; set; }
        public string out_shop_id_url { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public string fee_type { get; set; }
        public string erp_org { get; set; }

        [SugarColumn(IsIgnore = true)]
        public DeviceInfo[] device_infos { get; set; }

        [SugarColumn(IsIgnore = true)]
        public StaffInfo[] staff_infos { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string out_mch_name { get; set; }
    }
}

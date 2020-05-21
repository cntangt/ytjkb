﻿using Newtonsoft.Json;
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
        public string sub_out_mch_id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string shop_name { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool is_power
        {
            get
            {
                if (id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
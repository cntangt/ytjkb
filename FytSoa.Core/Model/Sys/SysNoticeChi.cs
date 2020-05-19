using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace FytSoa.Core.Model.Sys
{
    [MySugarTable("Sys_Notice_Chi")]
    public class SysNoticeChi
    {
        public int id { get; set; }
        public int notice_id { get; set; }
        public string admin_guid { get; set; }
        public bool read_status { get; set; }
        public DateTime? read_time { get; set; }
    }
}
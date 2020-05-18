using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace FytSoa.Core.Model.Sys
{
    [MySugarTable("Sys_Notice")]
    public class SysNotice
    {
        public int id { get; set; }
        public string title { get; set; }
        public int sort { get; set; }
        public string content { get; set; }
        public DateTime release_time { get; set; }
        public DateTime expiry_time { get; set; }
        public DateTime create_time { get; set; }
        public DateTime? update_time { get; set; }
        public bool status { get; set; }
        public bool delete { get; set; }
    }
}
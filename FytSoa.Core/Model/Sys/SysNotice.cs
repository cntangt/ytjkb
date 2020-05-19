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
        public string agent_ids { get; set; }
        public string merchant_ids { get; set; }
        public int sort { get; set; }
        public string content { get; set; }
        public DateTime? begin_time { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime create_time { get; set; }
        public DateTime? update_time { get; set; }
        public bool status { get; set; }
        public bool delete { get; set; }

        /// <summary>
        /// 返回代理商列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<NoticeToList> AgentList
        {
            get
            {
                var role = new List<NoticeToList>();

                if (!string.IsNullOrEmpty(agent_ids))
                {
                    role = JsonConvert.DeserializeObject<List<NoticeToList>>(agent_ids);
                }

                return role;
            }
        }

        /// <summary>
        /// 返回子商户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<NoticeToList> MerchantList
        {
            get
            {
                var role = new List<NoticeToList>();

                if (!string.IsNullOrEmpty(merchant_ids))
                {
                    role = JsonConvert.DeserializeObject<List<NoticeToList>>(merchant_ids);
                }

                return role;
            }
        }
    }

    public class NoticeToList
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
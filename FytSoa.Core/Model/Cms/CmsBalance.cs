using System;
using FytSoa.Service.DtoModel.Wx;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("agent_balance_fath")]
    public class CmsBalance
    {
        public string BillID { get; set; }
        public string BizDt { get; set; }
        public string agent_admin_guid { get; set; }
        public string agent_admin_name { get; set; }
        public decimal balance_amount { get; set; }
        public decimal rebate_amount { get; set; }
        public decimal modify_amount { get; set; }
        public decimal rebate_amount_rel { get; set; }
        public int statu { get; set; }
        public DateTime moditime { get; set; }
        public string Uptr { get; set; }
    }
}
using System;
using FytSoa.Service.DtoModel.Wx;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Agent")]
    public class CmsAgent
    {
        public int Id { get; set; }
        public string Admin_Guid { get; set; }
        public string Name { get; set; }
        public int Level_Id { get; set; }
        public string Tel { get; set; }
        public string Business_Area { get; set; }
        public SettleType Settle_Type { get; set; }
        public string Account_name { get; set; }
        public string Account_no { get; set; }
        public string Account_info { get; set; }
        public decimal Wxpay { get; set; }
        public decimal Alipay { get; set; }
        public decimal Otherpay { get; set; }
        public DateTime? Create_time { get; set; }
        public DateTime? Update_time { get; set; }
        public bool Status { get; set; }
        public bool Delete { get; set; }    
    }
}
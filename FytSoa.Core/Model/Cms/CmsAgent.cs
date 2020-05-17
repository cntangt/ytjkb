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
        public string Account_Name { get; set; }
        public string Account_No { get; set; }
        public string Account_Info { get; set; }
        public decimal Wxpay { get; set; }
        public decimal Alipay { get; set; }
        public decimal Otherpay { get; set; }
        public DateTime? Create_Time { get; set; }
        public DateTime? Update_Time { get; set; }
        public bool Status { get; set; }
        public bool Delete { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string LoginName { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Curr_Admin_Guid { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Level_Name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Settle_Name { get; set; }
    }
}
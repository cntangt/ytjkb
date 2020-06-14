using System;
using FytSoa.Service.DtoModel.Wx;
using Org.BouncyCastle.Asn1;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("agent_balance_chi")]
    public class CmsBalance_Chi
    {
        public string BillID { get; set; }
        public string out_sub_mch_id { get; set; }
        public string mch_admin_guid { get; set; }
        public string mch_name { get; set; }
        public SubPayPlatform sub_pay_platform { get; set; }
        public decimal success_amount { get; set; }
        public decimal refund_create_amount { get; set; }
        public decimal coupon_amount { get; set; }
        public decimal balance_amount { get; set; }
        public decimal rebate { get; set; }
        public decimal rebate_amount { get; set; }
        public decimal modify_amount { get; set; }
        public decimal rebate_amount_rel { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Settle_Name { get; set; }
    }
}
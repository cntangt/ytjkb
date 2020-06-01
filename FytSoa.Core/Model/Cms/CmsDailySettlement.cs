using FytSoa.Service.DtoModel.Wx;
using System;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("cms_daily_settlement")]
    public class CmsDailySettlement
    {
        public DateTime business_date { get; set; }
        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
        public string out_shop_id { get; set; }
        public string staff_id { get; set; }
        public string device_id { get; set; }
        public SubPayPlatform sub_pay_platform { get; set; }
        public string shop_name { get; set; }
        public string erp_org { get; set; }
        public long count_trade { get; set; }
        public long count_refund { get; set; }
        public long total_trade_fee { get; set; }
        public long total_refund_fee { get; set; }
        public long receivable_fee { get; set; }
        public long discount_fee { get; set; }
        public long poundage_fee { get; set; }
        public long settlement_fee { get; set; }
        public DateTime create_time { get; set; }
    }
}

using FytSoa.Service.DtoModel.Wx;
using System;

namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("cms_order_overview")]
    public class CmsOrderOverview
    {
        public DateTime business_date { get; set; }
        public string out_sub_mch_id { get; set; }
        public string out_shop_id { get; set; }
        public string staff_id { get; set; }
        public string device_id { get; set; }
        public string qrcode_id { get; set; }
        public SourceType source_type { get; set; }
        public SubPayPlatform sub_pay_platform { get; set; }
        public TradeType trade_type { get; set; }
        public string fee_type { get; set; }
        public long success_count { get; set; }
        public long success_amount { get; set; }
        public long settle_amount { get; set; }
        public long discount_amount { get; set; }
        public long pay_settle_amount { get; set; }
        public long refund_settle_amount { get; set; }
        public long refund_create_count { get; set; }
        public long refund_create_amount { get; set; }
        public long poundage { get; set; }
        public long income_amount { get; set; }
        public long order_refunded_amount { get; set; }
        public long sub_mch_recharge_coupon_count { get; set; }
        public long sub_mch_recharge_coupon_amount { get; set; }
        public long platform_recharge_coupon_count { get; set; }
        public long platform_recharge_coupon_amount { get; set; }
        public long others_recharge_coupon_count { get; set; }
        public long others_recharge_coupon_amount { get; set; }
        public long sub_mch_non_recharge_coupon_count { get; set; }
        public long sub_mch_non_recharge_coupon_amount { get; set; }
        public long platform_non_recharge_coupon_count { get; set; }
        public long platform_non_recharge_coupon_amount { get; set; }
        public long others_non_recharge_coupon_count { get; set; }
        public long others_non_recharge_coupon_amount { get; set; }
        public long freeze_count { get; set; }
        public long freeze_amount { get; set; }
        public long direct_unfreeze_count { get; set; }
        public long direct_unfreeze_amount { get; set; }
        public long consumed_unfreeze_amount { get; set; }
        public long auto_unfreeze_amount { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string shop_name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string erp_org { get; set; }
    }
}

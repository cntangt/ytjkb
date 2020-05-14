namespace FytSoa.Core.Model.Cms
{
    [MySugarTable("Cms_Admin_Merchant_Rel")]
    public class CmsAdminMerchantRel
    {
        public string Admin_Guid { get; set; }
        public string Out_Mch_Id { get; set; }
        public string Sub_Out_Mch_Id { get; set; }
    }
}

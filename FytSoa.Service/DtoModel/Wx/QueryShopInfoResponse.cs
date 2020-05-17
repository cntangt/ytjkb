using FytSoa.Core.Model.Wx;
using System;

namespace FytSoa.Service.DtoModel.Wx
{
    public class QueryShopInfoResponse
    {
        public ShopInfo[] shop_infos { get; set; }
        public int total_count { get; set; }
        public string nonce_str { get; set; }
    }

    public class Shop_Infos
    {
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string branch_name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public int coordinate_type { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string height { get; set; }
        public string phone { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public Device_Infos[] device_infos { get; set; }
        public Staff_Infos[] staff_infos { get; set; }
        public string out_shop_id { get; set; }
        public string out_shop_id_url { get; set; }
        public string fee_type { get; set; }
        public string goods_tag { get; set; }
        public bool receive_notify_after_duty { get; set; }
        public int pay_page_mode { get; set; }
        public int shop_type { get; set; }
        public Qrcode_Infos[] qrcode_infos { get; set; }
        public bool send_micro_pay_notify { get; set; }
        public string shop_group_id { get; set; }
        public bool disable_membership_card { get; set; }
        public Wxpay_Shop_Info_Ext wxpay_shop_info_ext { get; set; }
        public Alipay_Shop_Info_Ext alipay_shop_info_ext { get; set; }
    }

    public class Wxpay_Shop_Info_Ext
    {
        public string ext_sub_mch_id { get; set; }
    }

    public class Alipay_Shop_Info_Ext
    {
        public string ali_authorization_url { get; set; }
    }

    public class Device_Infos
    {
        public string device_id { get; set; }
        public int device_type { get; set; }
        public string remark { get; set; }
        public string device_name { get; set; }
        public string ext_setting { get; set; }
        public int device_shift_type { get; set; }
        public string sn_code { get; set; }
        public int bind_status { get; set; }
        public int bind_qrcode_source_type { get; set; }
        public string bind_qrcode_id { get; set; }
        public string bill_channel_id { get; set; }
        public string out_device_id { get; set; }
    }

    public class Staff_Infos
    {
        public string staff_id { get; set; }
        public string staff_name { get; set; }
        public string remark { get; set; }
        public bool shop_manager { get; set; }
        public bool receive_one_code_pay_notify { get; set; }
        public string mch_open_id { get; set; }
        public string cloud_pay_open_id { get; set; }
        public string trade_qr_code { get; set; }
        public bool refund_auth { get; set; }
        public string national_code { get; set; }
        public string mobile_phone { get; set; }
        public string out_staff_id { get; set; }
    }

    public class Qrcode_Infos
    {
        public string qrcode_id { get; set; }
        public string qrcode_name { get; set; }
        public int type { get; set; }
        public string staff_id { get; set; }
    }

}

using FytSoa.Service.DtoModel.Wx;
using System;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_shopinfo")]
    public class ShopInfo
    {
        public int id { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public int coordinate_type { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string height { get; set; }
        public string phone { get; set; }
        public string out_shop_id { get; set; }
        public string out_shop_id_url { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public DeviceInfo[] device_infos { get; set; }
        public StaffInfo[] staff_infos { get; set; }
        public string fee_type { get; set; }
    }
}

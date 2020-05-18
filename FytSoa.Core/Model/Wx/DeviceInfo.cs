using FytSoa.Service.DtoModel.Wx;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_deviceinfo")]
    public class DeviceInfo
    {
        public int id { get; set; }
        public string out_mch_id { get; set; }
        public string sub_out_mch_id { get; set; }
        public string out_shop_id { get; set; }
        public string device_id { get; set; }
        public string remark { get; set; }
        public string device_name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceShiftType device_shift_type { get; set; }
    }
}

using FytSoa.Service.DtoModel.Wx;

namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_deviceinfo")]
    public class DeviceInfo
    {
        public int id { get; set; }
        public string device_id { get; set; }
        public string remark { get; set; }
        public string device_name { get; set; }
        public DeviceShiftType device_shift_type { get; set; }
    }
}

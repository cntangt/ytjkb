﻿namespace FytSoa.Core.Model.Wx
{
    [MySugarTable("cms_staffinfo")]
    public class StaffInfo
    {
        public int id { get; set; }
        public string staff_name { get; set; }
        public string remark { get; set; }
        public bool shop_manager { get; set; }
        public bool receive_one_code_pay_notify { get; set; }
    }
}

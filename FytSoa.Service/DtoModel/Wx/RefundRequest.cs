using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace FytSoa.Service.DtoModel.Wx
{
    public class RefundRequest : WxRequest<RefundResponse>
    {
        public override string ApiName => "refund";
        public override string DataName => "refund";

        public Refund_Content refund_content { get; set; }
        public Pay_Mch_Key pay_mch_key { get; set; }
        public Order_Client order_client { get; set; }
    }

    public class Refund_Content
    {
        public string out_refund_no { get; set; }
        /// <summary>
        /// 商户订单号(云支付订单前缀 + 商户自定义部分)
        /// </summary>
        public string out_trade_no { get; set; }
        public string refund_fee_type { get; set; }
        public int total_fee { get; set; }
        public string refund_reason { get; set; }
        public int refund_fee { get; set; }
    }

    public class Pay_Mch_Key
    {
        public PayPlatform pay_platform { get; set; }
        public string out_mch_id { get; set; }
        public string out_sub_mch_id { get; set; }
        public string out_shop_id { get; set; }
        /// <summary>
        /// 操作者是否为商户管理员（此参数是为了校验退款人身份，操作退款人必须拥有退款权限。
        /// <para>true 代表操作者是商户管理员，该角色默认拥有退款权限。</para>
        /// 当为 false 时与 authorization_staff_id 配合使用校验退款权限）。
        /// </summary>
        public bool is_sub_mch_admin { get; set; }
        /// <summary>
        /// 如果 is_sub_mch_admin 为 false，则此项为必填，如果为 true，则此项不填。
        /// <para>1. 如果当前 staff_id 有退款权限则传自身的 staff_id。</para>
        /// 2. 如果是其他人授权则填授权者的 staff_id。
        /// <para>3. 机具调用传机具内配置的 staff_id。</para>
        /// </summary>
        public string authorization_staff_id { get; set; }
        /// <summary>
        /// 如果当前操作者有退款权限则传自身的 name，如果是其他人授权则填授权者的 name。
        /// <para>此字段最终会补充在返回结构的退款原因（refund_reason）字段中。</para>
        /// </summary>
        public string authorization_name { get; set; }
    }

    public class Order_Client
    {
        public TerminalType terminal_type { get; set; }
        public string sdk_version { get; set; } = "1.0";
        public string spbill_create_ip { get; set; }
        public string device_id { get; set; }
        public string sn_code { get; set; }
    }

}
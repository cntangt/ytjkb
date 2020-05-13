namespace FytSoa.Service.DtoModel.Wx
{
    public enum OrderType
    {
        支付订单 = 1,
        退款单 = 2,
        全部单据 = 3,
        冻结单 = 4,
        解冻单 = 5
    }

    public enum TerminalType
    {
        Windows = 1,
        Andriod = 2,
        iOS = 3,
        Linux = 4,
        其他 = 100
    }

    public enum FeeType
    {
        CNY
    }

    public enum SourceType
    {
        一码付 = 1,
        店员户外二维码收银 = 2,
        绑定二维码 = 3,
        定额二维码 = 4,
        会员卡充值 = 5,
        临时二维码 = 6,
        会员卡一码付 = 7
    }

    public enum TradeType
    {
        刷卡支付 = 1,
        扫码支付 = 2,
        公众号支付 = 3,
        App支付 = 4,
        声波支付 = 5,
        H5支付 = 6,
        一码付支付 = 8,
        小程序支付 = 9
    }

    public enum PayPlatform
    {
        普通微信支付 = 100,
        普通支付宝 = 200,
        代金券校园卡 = 300,
        银行卡记账 = 401,
        现金记账 = 402
    }

    public enum AlipayRefundOrderState
    {
        退款单初始态 = 1,
        退款单成功态 = 2,
        申请退款失败 = 3,
        退款中 = 4,
        订单不存在 = 5
    }

    public enum WxpayOrderState
    {
        订单初始态 = 1,
        刷卡支付_成功 = 2,
        统一下单_支付成功 = 3,
        已转入退款 = 4,
        刷卡支付_顾客停止支付 = 5,
        统一下单_待顾客支付 = 6,
        统一下单_订单已关闭 = 7,
        刷卡支付_已撤单 = 8,
        刷卡支付_用户支付中 = 9,
        刷卡支付_支付错误 = 10,
        /// <summary>
        /// 表示本地有_第三方支付平台没有的订单
        /// </summary>
        作废状态 = 11,
        订单受理中 = 12,
        押金解冻成功 = 13,
        押金消费成功 = 14
    }

    public enum WxpayRefundOrderState
    {
        退款单初始态 = 1,
        退款成功 = 2,
        退款失败 = 3,
        退款处理中 = 4,
        /// <summary>
        /// 退款到银行发现用户的卡作废或冻结了，导致原路退款银行卡失败，资金回流到子商户的现金帐号，需要子商户人工干预，通过线下或者财付通转账的方式进行退款
        /// </summary>
        转入代发 = 5,
        /// <summary>
        /// 表示本地有，第三方支付平台没有的订单
        /// </summary>
        作废状态 = 6
    }

    public enum RecordRefundOrderState
    {
        退款单初始态 = 1,
        退款单成功态 = 2
    }

    public enum RecordOrderState
    {
        订单初始态 = 1,
        成功 = 2,
        退款 = 3
    }

    public enum DeviceShiftType
    {
        移动收款机具 = 1, 
        云支付收银台 = 2, 
        智能POS = 3, 
        其它 = 4
    }
}

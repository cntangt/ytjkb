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
}

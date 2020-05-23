namespace System
{
    public static class StringExtension
    {
        public static string GetShopErpOrg(this string shopName)
        {
            if (string.IsNullOrEmpty(shopName))
            {
                return string.Empty;
            }

            if (shopName.Length <= 4)
            {
                return shopName;
            }

            return shopName.Substring(0, 4);
        }
    }
}

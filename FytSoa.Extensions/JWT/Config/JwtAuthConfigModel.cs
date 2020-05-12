namespace FytSoa.Extensions
{
    public class JwtAuthConfigModel : BaseConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public JwtAuthConfigModel()
        {
            JWTSecretKey = Configuration["JwtAuth:SecurityKey"];
            WebExp = double.Parse(Configuration["JwtAuth:WebExp"]);
            AppExp = double.Parse(Configuration["JwtAuth:AppExp"]);
            WxExp = double.Parse(Configuration["JwtAuth:WxExp"]);
            OtherExp = double.Parse(Configuration["JwtAuth:OtherExp"]);
            Issuer = Configuration["JwtAuth:Issuer"];
            Audience = Configuration["JwtAuth:Audience"];
        }
        /// <summary>
        /// 
        /// </summary>
        public string JWTSecretKey = "lyDqoSIQmyFcUhmmN4KBRGWWzm1ELC7owHVtStOu1YD7wYz";
        /// <summary>
        /// 
        /// </summary>
        public double WebExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double AppExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double WxExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double OtherExp = 12;

        public string Issuer = "jwt";

        public string Audience = "jwt";
    }
}

using Microsoft.AspNetCore.Authorization;

namespace FytSoa.Extensions
{
    /// <summary>
    /// 新增一个公司的认证策略
    /// </summary>
    public class CompanyAuthorizeAttribute: AuthorizeAttribute
    {
        public const string CompanyAuthenticationScheme = "CompanyAuthenticationScheme";

        public CompanyAuthorizeAttribute()
        {
            this.AuthenticationSchemes = CompanyAuthenticationScheme;
        }
    }
}

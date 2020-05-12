using Microsoft.AspNetCore.Authorization;

namespace FytSoa.Extensions
{
    public class JwtAuthorizeAttribute: AuthorizeAttribute
    {
        public const string JwtAuthenticationScheme = "JwtAuthenticationScheme";

        public JwtAuthorizeAttribute()
        {
            this.AuthenticationSchemes = JwtAuthenticationScheme;
        }
    }
}

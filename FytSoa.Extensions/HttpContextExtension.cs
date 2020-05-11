using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FytSoa.Extensions
{
    public static class HttpContextExtension
    {
        public async static Task<string> LoginUserId(this HttpContext context)
        {
            if (context == null)
            {
                return null;
            }

            var auth = await context.AuthenticateAsync();

            if (auth == null || auth.Principal == null || auth.Principal.Identities == null)
            {
                return null;
            }

            var identity = auth.Principal.Identities.FirstOrDefault(p => p.IsAuthenticated);

            if (identity == null)
            {
                return null;
            }

            var sid = identity.FindFirst(ClaimTypes.Sid);

            if (sid == null)
            {
                return null;
            }

            return sid.Value;
        }

        public async static Task<bool> IsSystem(this HttpContext context)
        {
            if (context == null)
            {
                return false;
            }

            var auth = await context.AuthenticateAsync();

            if (auth == null || auth.Principal == null || auth.Principal.Identities == null)
            {
                return false;
            }

            var identity = auth.Principal.Identities.FirstOrDefault(p => p.IsAuthenticated);

            if (identity == null)
            {
                return false;
            }

            var sid = identity.FindFirst(ClaimTypes.PrimarySid);

            if (sid == null)
            {
                return false;
            }

            return bool.TryParse(sid.Value, out bool isSystem) && isSystem;
        }
    }
}

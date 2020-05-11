using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class SiteExtension
    {
        public static async Task<string> SiteGuidAsync(this IDistributedCache cache)
        {
            var site = await cache.GetAsync<CmsSite>(KeyHelper.NOWSITE);

            if (site == null)
            {
                return "";
            }

            return site.Guid;
        }

        public static Task SiteSiteGuidAsync(this IDistributedCache cache, CmsSite site)
        {
            return cache.SetAsync(KeyHelper.NOWSITE, site);
        }
    }
}

using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages
{
    public class BasePageModel : PageModel
    {
        protected CmsSite CurrentSite;
        protected string SiteGuid;

        public BasePageModel(IDistributedCache cache)
        {
            Task.Run(async () =>
            {
                CurrentSite = await cache.GetAsync<CmsSite>(KeyHelper.NOWSITE);
                SiteGuid = CurrentSite?.Guid;
            }).Wait();
        }
    }
}

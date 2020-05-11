using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected CmsSite CurrentSite;
        protected string SiteGuid;


        public BaseController(IDistributedCache cache)
        {
            Task.Run(async () =>
            {
                CurrentSite = await cache.GetAsync<CmsSite>(KeyHelper.NOWSITE);
                SiteGuid = CurrentSite?.Guid;
            }).Wait();
        }
    }
}
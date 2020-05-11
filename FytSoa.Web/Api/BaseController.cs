using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace FytSoa.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string SiteGuid { get; private set; }


        public BaseController(IDistributedCache cache)
        {
            SiteGuid = cache.SiteGuidAsync().Result;
        }
    }
}
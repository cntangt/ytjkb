using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class CmsSiteController : ControllerBase
    {
        private readonly ICmsSiteService _siteService;
        private readonly IDistributedCache _cache;

        public CmsSiteController(ICmsSiteService siteService, IDistributedCache cache)
        {
            _siteService = siteService;
            _cache = cache;
        }

        /// <summary>
        /// 获得站点列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IActionResult> SaveList()
        {
            return Ok(await _siteService.GetListAsync(m => true, m => m.AddTime, DbOrderEnum.Asc));
        }

        /// <summary>
        /// 保存站点信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost("savesite")]
        public async Task<IActionResult> SaveSite([FromBody]CmsSite parm)
        {
            var res = new ApiResult<string>();

            if (!string.IsNullOrEmpty(parm?.Guid))
            {
                res = await _siteService.UpdateAsync(parm);
            }
            else
            {
                res.message = "请选择修改站点";
                res.success = false;

                return Ok(res);
            }

            await _cache.SiteSiteGuidAsync(parm);

            return Ok(res);
        }
    }
}
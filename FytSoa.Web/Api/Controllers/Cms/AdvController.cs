using FytSoa.Common;
using FytSoa.Core;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class AdvController : BaseController
    {
        private readonly ICmsAdvClassService _classService;
        private readonly ICmsAdvListService _listService;

        public AdvController(ICmsAdvClassService classService, ICmsAdvListService listService, IDistributedCache cache) : base(cache)
        {
            _classService = classService;
            _listService = listService;
        }

        #region 广告栏位管理

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("class/page")]
        public async Task<IActionResult> GetClassPages()
        {
            return Ok(await _classService.GetListAsync(m => m.SiteGuid == SiteGuid, m => m.Guid, DbOrderEnum.Asc));
        }

        /// <summary>
        /// 获得字典栏目Tree列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("class/add")]
        public async Task<IActionResult> AddClass([FromBody]CmsAdvClass parm)
        {
            parm.SiteGuid = SiteGuid;
            parm.Guid = Guid.NewGuid().ToString();
            return Ok(await _classService.AddAsync(parm));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("class/delete")]
        public async Task<IActionResult> DeleteClass([FromBody]ParmString obj)
        {
            return Ok(await _classService.DeleteAsync(obj.parm));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("class/edit")]
        public async Task<IActionResult> EditClass([FromBody]CmsAdvClass parm)
        {
            parm.SiteGuid = SiteGuid;
            return Ok(await _classService.UpdateAsync(parm));
        }
        #endregion

        #region 广告位管理

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list/page")]
        public async Task<IActionResult> GetAdvListPages([FromQuery]PageParm parm)
        {
            var res = await _listService.GetListAsync(m => m.ClassGuid == parm.key, m => m.Sort, DbOrderEnum.Desc);
            return Ok(new { code = 0, msg = "success", count = 1, res.data });
        }

        /// <summary>
        /// 获得字典栏目Tree列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list/add")]
        public async Task<IActionResult> AddAdvList([FromBody]CmsAdvList parm)
        {
            parm.Guid = Guid.NewGuid().ToString();
            return Ok(await _listService.AddAsync(parm));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("list/delete")]
        public async Task<IActionResult> DeleteAdvList([FromBody]ParmString obj)
        {
            return Ok(await _listService.DeleteAsync(obj.parm));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("list/edit")]
        public async Task<IActionResult> EditAdvList([FromBody]CmsAdvList parm)
        {
            return Ok(await _listService.UpdateAsync(parm));
        }
        #endregion
    }
}
﻿using FytSoa.Common;
using FytSoa.Core;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class DownloadController : BaseController
    {
        private readonly ICmsDownloadService _downloadService;
        public DownloadController(ICmsDownloadService downloadService, IDistributedCache cache) : base(cache)
        {
            _downloadService = downloadService;
        }

        #region 文章管理
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("getpages")]
        public IActionResult GetPages([FromQuery]PageParm parm)
        {
            parm.site = SiteGuid;
            var res = _downloadService.GetList(parm);
            return Ok(new { code = 0, msg = "success", count = res.TotalItems, data = res.Items });
        }


        /// <summary>
        /// 获得字典栏目Tree列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]CmsDownload parm)
        {
            parm.SiteGuid = SiteGuid;
            //处理文件类型
            parm.FileType= FileHelper.GetFileExt(parm.FileUrl);
            return Ok(await _downloadService.AddAsync(parm));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody]ParmString param)
        {
            return Ok(await _downloadService.DeleteAsync(param.parm));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody]CmsDownload parm)
        {
            parm.SiteGuid = SiteGuid;
            //处理文件类型
            parm.FileType = FileHelper.GetFileExt(parm.FileUrl);
            return Ok(await _downloadService.UpdateAsync(parm));
        }
        #endregion
    }
}
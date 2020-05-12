﻿using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class LevelController : ControllerBase
    {
        private readonly ICmsLevelService levelService;
        public LevelController(ICmsLevelService levelService) 
        {
            this.levelService = levelService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            var list = await levelService.GetListAsync();
            
            return Ok(new { code = 0, msg = "success", count = 1, data = list.data });
        }

        [HttpPost("add"), ApiAuthorize(Modules = "Level", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]CmsLevel parm)
        {
            return Ok(await levelService.AddAsync(parm));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            return Ok(await levelService.DeleteAsync(obj.parm));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Level", Methods = "Update", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Edit([FromBody]CmsLevel parm)
        {
            return Ok(await levelService.UpdateAsync(parm));
        }
    }
}
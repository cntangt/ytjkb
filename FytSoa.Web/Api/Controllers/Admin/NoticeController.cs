using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Sys;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class NoticeController : ControllerBase
    {
        private readonly ISysAdminService _adminService;
        private readonly ISysNoticeService _noticeService;
        public NoticeController(
            ISysAdminService adminService,
            ISysNoticeService noticeService)
        {
            _adminService = adminService;
            _noticeService = noticeService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            var res = await _noticeService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpPost("add"), ApiAuthorize(Modules = "Notice", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]SysNotice parm)
        {
            return Ok(await _noticeService.AddAsync(parm));
        }

        [HttpPost("delete"), ApiAuthorize(Modules = "Notice", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            return Ok(await _noticeService.DeleteAsync(obj.parm));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Notice", Methods = "Update", LogType = LogEnum.UPDATE)]
        public async Task<IActionResult> Edit([FromBody]SysNotice parm)
        {
            return Ok(await _noticeService.UpdateAsync(parm));
        }

        [HttpPost("bymodel")]
        public async Task<IActionResult> GetModelById([FromBody]ParmString parm)
        {
            var id = Convert.ToInt32(parm.parm);
            return Ok(await _noticeService.GetModelAsync(m => m.id == id));
        }

        [HttpPost("unreadquantity")]
        public async Task<IActionResult> GetUnreadQuantity()
        {
            return Ok(await _noticeService.GetUnreadQuantity(await HttpContext.LoginUserId()));
        }
    }
}
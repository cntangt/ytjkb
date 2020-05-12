using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Menu")]
    [JwtAuthorize(Roles = "Admin")]
    public class MenuController : ControllerBase
    {
        private readonly ISysMenuService _sysMenuService;
        private readonly ISysAuthorizeService _authorizeService;
        private readonly ISysPermissionsService _sysPermissionsService;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        public MenuController(
            ISysMenuService sysMenuService,
            ISysAuthorizeService authorizeService,
            ISysPermissionsService sysPermissionsService,
            IConfiguration config,
            IDistributedCache cache)
        {
            _sysMenuService = sysMenuService;
            _authorizeService = authorizeService;
            _sysPermissionsService = sysPermissionsService;
            _config = config;
            _cache = cache;
        }

        /// <summary>
        /// 根据菜单，获得当前菜单的所有功能权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("bycode")]
        public IActionResult GetCodeByMenu([FromQuery]MenuGetParm param)
        {
            var res = _authorizeService.GetCodeByMenu(param.role, param.menu);
            return Ok(new { code = 0, msg = "success", count = 1, res.data });
        }

        /// <summary>
        /// 获得组织结构Tree列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("gettree")]
        public async Task<IActionResult> GetListPage([FromBody]MenuTreeParm param)
        {
            var res = await _sysMenuService.GetListTreeAsync(param.roleGuid);
            return Ok(res.data);
        }

        /// <summary>
        /// 提供角色弹框授权返回客户端菜单列表和当前角色的列表
        /// 涉及到选中状态
        /// </summary>
        [HttpPost("menubyrole")]
        public async Task<IActionResult> GetMenuByRole([FromBody]MenuTreeParm param)
        {
            var menu = await _sysMenuService.GetListTreeAsync(param.roleGuid);
            var res = new MenuRoleDto()
            {
                menu = menu.data
            };
            return Ok(res);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            var res = await _sysMenuService.GetPagesAsync(parm);
            if (res.data?.Items?.Count > 0)
            {
                foreach (var item in res.data.Items)
                {
                    item.Name = Utils.LevelName(item.Name, item.Layer);
                }
            }
            return Ok(new { code = 0, msg = "success", count = res.data?.TotalItems, data = res.data?.Items });
        }

        /// <summary>
        /// 提供权限查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("authmenu")]
        public async Task<IActionResult> GetAuthMenuAsync()
        {
            var res = new ApiResult<List<SysMenuDto>>();

            var userGuid = await HttpContext.LoginUserId();

            res.data = await _cache.GetAsync<List<SysMenuDto>>(KeyHelper.ADMINMENU + "_" + userGuid);

            if (res.data == null)
            {
                res.statusCode = (int)ApiEnum.URLExpireError;
                res.message = "Session已过期，请重新登录";
            }

            return Ok(res);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("add"), ApiAuthorize(Modules = "Menu", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> AddMenu([FromBody]SysMenu model)
        {
            return Ok(await _sysMenuService.AddAsync(model, model.cbks));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete"), ApiAuthorize(Modules = "Menu", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> DeleteMenu([FromBody]ParmString obj)
        {
            var list = Utils.StrToListString(obj.parm);
            return Ok(await _sysMenuService.DeleteAsync(m => list.Contains(m.Guid)));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit"), ApiAuthorize(Modules = "Menu", Methods = "Update", LogType = LogEnum.UPDATE)]
        public async Task<IActionResult> EditMenu([FromBody]SysMenu model)
        {
            return Ok(await _sysMenuService.ModifyAsync(model, model.cbks));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        [HttpPost("sort")]
        public async Task<IActionResult> ColStor([FromBody]ParmStringSort obj)
        {
            return Ok(await _sysMenuService.ColSort(obj.p, obj.i, obj.o));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("authorizaion")]
        [ApiAuthorize(Modules = "Menu", Methods = "Update", LogType = LogEnum.STATUS)]
        public async Task<IActionResult> GetAuthorizaionMenu([FromBody]ParmString obj)
        {
            return Ok(await _sysMenuService.GetMenuByRole(obj.parm));
        }
    }
}
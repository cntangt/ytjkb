using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BotDetect.Web;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
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
    [Produces("application/json")]
    [Route("api/Admin")]
    [JwtAuthorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly ISysAdminService _adminService;
        private readonly ISysLogService _logService;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;
        private readonly ICmsShopService _shop;
        public AdminController(
            ISysAdminService adminService,
            ISysLogService logService,
            IConfiguration config,
            IDistributedCache cache,
            ICmsShopService shop) : base(cache)
        {
            _adminService = adminService;
            _logService = logService;
            _config = config;
            _cache = cache;
            _shop = shop;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            if (!await HttpContext.IsSystem())
            {
                parm.CreateBy = await HttpContext.LoginUserId();
            }

            var res = await _adminService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        /// <summary>
        /// 获取用户门店权限
        /// </summary>
        [HttpGet("getshops")]
        public async Task<IActionResult> GetShops(string admin_guid)
        {
            var res = await _adminService.GetShopsAsync(admin_guid);

            return Ok(new { data = new AdminShopRel { shopList = res.data } });
        }

        /// <summary>
        /// 保存用户门店权限
        /// </summary>
        [HttpPost("saveshops")]
        public async Task<IActionResult> SaveShops([FromBody]AdminShopRel parm)
        {
            return Ok(await _adminService.AddShopsAsync(parm));
        }

        /// <summary>
        /// 根据编号，查询用户信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost("bymodel")]
        public async Task<IActionResult> GetModelByGuid([FromBody]ParmString parm)
        {
            var res = await _adminService.GetModelAsync(m => m.Guid == parm.parm);
            if (!string.IsNullOrEmpty(res.data.Guid))
            {
                res.data.LoginPwd = DES3Encrypt.DecryptString(res.data.LoginPwd);
            }
            return Ok(res);
        }

        /// <summary>
        /// 获得字典栏目Tree列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("add"), ApiAuthorize(Modules = "Admin", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> AddAdmin([FromBody]SysAdmin parm)
        {
            parm.CreateBy = await HttpContext.LoginUserId();

            return Ok(await _adminService.AddAsync(parm));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete"), ApiAuthorize(Modules = "Admin", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> DeleteAdmin([FromBody]ParmString obj)
        {
            return Ok(await _adminService.DeleteAsync(obj.parm));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit"), ApiAuthorize(Modules = "Admin", Methods = "Update", LogType = LogEnum.UPDATE)]
        public async Task<IActionResult> EditAdmin([FromBody]SysAdmin parm)
        {
            return Ok(await _adminService.ModifyAsync(parm));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]SysAdminLogin parm)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.HttpRequestError };
            try
            {
                //获得公钥私钥，解密
                var rsaKey = await _cache.GetAsync<List<string>>($"LOGINKEY:{parm.lid}");
                if (rsaKey == null)
                {
                    res.message = "登录失败，请刷新浏览器再次登录";
                    return Ok(res);
                }
                //Ras解密密码
                var ras = new RSACrypt(rsaKey[0], rsaKey[1]);
                parm.password = ras.Decrypt(parm.password);

                //获得用户登录限制次数
                var configLoginCount = Convert.ToInt32(_config[KeyHelper.LOGINCOUNT]);
                //获得登录次数和过期时间
                var loginConfig = await _cache.GetAsync<SysAdminLoginConfig>(KeyHelper.LOGINCOUNT) ?? new SysAdminLoginConfig();
                if (loginConfig.Count != 0 && loginConfig.DelayMinute != null)
                {
                    //说明存在过期时间，需要判断
                    if (DateTime.Now <= loginConfig.DelayMinute)
                    {
                        res.message = "您的登录以超过设定次数，请稍后再次登录~";
                        return Ok(res);
                    }
                    else
                    {
                        //已经过了登录的预设时间，重置登录配置参数
                        loginConfig.Count = 0;
                        loginConfig.DelayMinute = null;
                    }
                }

                #region 验证码

                var captcha = new SimpleCaptcha();
                if (!captcha.Validate(parm.code, parm.cid))
                {
                    res.message = "验证码错误";
                    res.statusCode = (int)ApiEnum.ParameterError;
                    return Ok(res);
                }

                #endregion

                //查询登录结果
                var dbres = await _adminService.LoginAsync(parm);
                if (dbres.statusCode != 200)
                {
                    //增加登录次数
                    loginConfig.Count += 1;
                    //登录的次数大于配置的次数，则提示过期时间
                    if (loginConfig.Count == configLoginCount)
                    {
                        var configDelayMinute = Convert.ToInt32(_config[KeyHelper.LOGINDELAYMINUTE]);
                        //记录过期时间
                        loginConfig.DelayMinute = DateTime.Now.AddMinutes(configDelayMinute);
                        res.message = "登录次数超过" + configLoginCount + "次，请" + configDelayMinute + "分钟后再次登录";
                        return Ok(res);
                    }
                    //记录登录次数，保存到session
                    await _cache.SetAsync(KeyHelper.LOGINCOUNT, loginConfig);
                    //提示用户错误和登录次数信息
                    res.message = dbres.message + "　　您还剩余" + (configLoginCount - loginConfig.Count) + "登录次数";
                    return Ok(res);
                }

                var user = dbres.data.admin;
                var identity = new ClaimsPrincipal(
                 new ClaimsIdentity(new[]
                     {
                        new Claim(ClaimTypes.PrimarySid, user.IsSystem.ToString()),
                        new Claim(ClaimTypes.Sid, user.Guid),
                        new Claim(ClaimTypes.Role, "授权用户"),
                        new Claim(ClaimTypes.Name, user.TrueName),
                        new Claim(ClaimTypes.WindowsAccountName, user.LoginName),
                        new Claim(ClaimTypes.UserData, user.UpLoginDate.ToString())
                     }, CookieAuthenticationDefaults.AuthenticationScheme)
                );
                //如果保存用户类型是Session，则默认设置cookie退出浏览器 清空
                if (_config[KeyHelper.LOGINSAVEUSER] == "Session")
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                    {
                        AllowRefresh = false
                    });
                }
                else
                {
                    //根据配置保存浏览器用户信息，小时单位
                    var hours = int.Parse(_config[KeyHelper.LOGINCOOKIEEXPIRES]);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddHours(hours),
                        IsPersistent = true,
                        AllowRefresh = false
                    });
                }

                //把权限存到缓存里
                await _cache.SetAsync(KeyHelper.ADMINMENU + "_" + dbres.data.admin.Guid, dbres.data.menu);

                res.data = JwtHelper.IssueJWT(new TokenModel()
                {
                    Uid = user.Guid,
                    UserName = user.LoginName,
                    Role = "Admin",
                    TokenType = "Web"
                });
                await _cache.RemoveAsync($"LOGINKEY:{parm.lid}");
                await _cache.RemoveAsync(KeyHelper.LOGINCOUNT);

                #region 保存日志
                var agent = HttpContext.Request.Headers["User-Agent"];
                var log = new SysLog()
                {
                    Guid = Guid.NewGuid().ToString(),
                    Logged = DateTime.Now,
                    Logger = LogEnum.LOGIN.GetEnumText(),
                    Level = "Info",
                    Message = "登录：" + parm.loginname,
                    Callsite = "/fytadmin/login",
                    IP = HttpContext.GetIP(),
                    User = parm.loginname,
                    Browser = agent.ToString()
                };
                await _logService.AddAsync(log);
                #endregion
            }
            catch (CryptographicException)
            {
                res.message = "登录失败，请刷新浏览器重试";
                res.statusCode = (int)ApiEnum.Error;
                return Ok(res);
            }
            catch (Exception ex)
            {
                var agent = HttpContext.Request.Headers["User-Agent"];
                var log = new SysLog()
                {
                    Guid = Guid.NewGuid().ToString(),
                    Logged = DateTime.Now,
                    Logger = LogEnum.LOGIN.GetEnumText(),
                    Level = "Error",
                    Message = "登录失败！" + ex.Message,
                    Exception = ex.ToString(),
                    Callsite = "/fytadmin/login",
                    IP = HttpContext.GetIP(),
                    User = parm.loginname,
                    Browser = agent.ToString()
                };
                await _logService.AddAsync(log);

                res.message = "登录失败，请刷新浏览器重试";
                res.statusCode = (int)ApiEnum.Error;
                return Ok(res);
            }

            res.statusCode = (int)ApiEnum.Status;

            return Ok(res);
        }

        /// <summary>
        /// 切换站点
        /// </summary>
        /// <returns></returns>
        [HttpPost("rep/site")]
        public async Task<IActionResult> UpdateNowSite([FromBody]CmsSite parm)
        {
            var menuSaveType = _config[KeyHelper.LOGINAUTHORIZE];
            if (menuSaveType == "Redis")
            {
                await _cache.SetAsync(KeyHelper.NOWSITE, parm);
            }
            else
            {
                await _cache.SetAsync(KeyHelper.NOWSITE, parm);
            }
            return Ok(new ApiResult<string>());
        }

        /// <summary>
        /// 管理员退出
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout"), Log("Admin：LogOut", LogType = LogEnum.LOGOUT)]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new ApiResult<string>() { data = "/fytadmin/login/" });
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        [HttpPost("updatepwd"), Log("Admin:Update", LogType = LogEnum.UPDATE)]
        public async Task<IActionResult> UpdatePwd([FromBody]UpdatePwdDto parm)
        {
            parm.userId = await HttpContext.LoginUserId();
            return Ok(await _adminService.UpdatePwdAsync(parm));
        }
    }
}
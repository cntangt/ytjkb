using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class BalanceController : ControllerBase
    {
        private readonly ICmsBalanceService balanceService;
        public BalanceController(ICmsBalanceService balanceService, ICmsLevelService levelService)
        {
            this.balanceService = balanceService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            if (!await HttpContext.IsSystem())
            {
                parm.guid = await HttpContext.LoginUserId();
            }

            var res = await balanceService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpGet("gettotals")]
        public async Task<IActionResult> GetTotals([FromQuery]PageParm parm)
        {
            var list = await balanceService.GetTotalAmountAsync(parm.key);

            return Ok(new { code = 0, msg = "success", count = 1, data = list });
        }

        [HttpGet("getdetailpages")]
        public async Task<IActionResult> GetDetailPages([FromQuery]PageParm parm)
        {
            var res = await balanceService.GetDetailPageAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpPost("modify"), ApiAuthorize(Modules = "Balance", Methods = "Update", LogType = LogEnum.UPDATE)]
        public Task<ApiResult<string>> Modify([FromBody]CmsBalance_Chi parm) => balanceService.ModifyAmountAsync(parm);
    }
}
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
    public class AgentController : ControllerBase
    {
        private readonly ICmsAgentService agentService;
        private readonly ICmsLevelService levelService;
        public AgentController(ICmsAgentService agentService, ICmsLevelService levelService)
        {
            this.agentService = agentService;
            this.levelService = levelService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            var list = await agentService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = 1, data = list.data.Items });
        }

        [HttpPost("add"), ApiAuthorize(Modules = "Agent", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]CmsAgent parm)
        {
            parm.Curr_Admin_Guid = await HttpContext.LoginUserId();
            return Ok(await agentService.AddAsync(parm));
        }

        [HttpPost("delete"), ApiAuthorize(Modules = "Agent", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            var list = Utils.StrToListInt(obj.parm);
            return Ok(await agentService.DeleteAsync(p => list.Contains(p.Id)));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Agent", Methods = "Update", LogType = LogEnum.UPDATE)]
        public async Task<IActionResult> Edit([FromBody]CmsAgent parm)
        {
            return Ok(await agentService.UpdateAsync(parm));
        }

        [HttpPost("level")]
        public async Task<IActionResult> Level([FromBody]ParmString obj)
        {
            var id = Convert.ToInt32(obj.parm);
            return Ok((await levelService.GetModelAsync(t => t.Id == id)).data);
        }

        [HttpPost("updatestatus"), ApiAuthorize(Modules = "Agent", Methods = "Update", LogType = LogEnum.UPDATE)]
        public Task<ApiResult<string>> UpdateStatus([FromBody]CmsAgent parm) => agentService.UpdateStatusAsync(parm);
    }
}
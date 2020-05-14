using FytSoa.Common;
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
    public class AgentController : ControllerBase
    {
        private readonly ICmsAgentService agentService;
        public AgentController(ICmsAgentService agentService)
        {
            this.agentService = agentService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            var list = await agentService.GetListAsync();

            return Ok(new { code = 0, msg = "success", count = 1, data = list.data });
        }

        [HttpPost("add"), ApiAuthorize(Modules = "Level", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]CmsAgent parm)
        {
            return Ok(await agentService.AddAsync(parm));
        }

        [HttpPost("delete"), ApiAuthorize(Modules = "Level", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            var list = Utils.StrToListInt(obj.parm);
            return Ok(await agentService.DeleteAsync(p => list.Contains(p.id)));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Level", Methods = "Update", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Edit([FromBody]CmsAgent parm)
        {
            return Ok(await agentService.UpdateAsync(parm));
        }
    }
}
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class MerchantController : ControllerBase
    {
        private readonly ICmsMerchantService merchantService;
        private readonly ICmsAgentService agentService;
        public MerchantController(ICmsMerchantService merchantService,ICmsAgentService agentService)
        {
            this.merchantService = merchantService;
            this.agentService = agentService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            if (!await HttpContext.IsSystem())
            {
                parm.CreateBy = await HttpContext.LoginUserId();
            }

            var res = await merchantService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpPost("add"), ApiAuthorize(Modules = "Merchant", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]CmsMerchant parm)
        {
            parm.status = true;
            return Ok(await merchantService.AddAsync(parm));
        }

        [HttpPost("delete"), ApiAuthorize(Modules = "Merchant", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            var list = Utils.StrToListInt(obj.parm);
            return Ok(await merchantService.DeleteAsync(p => list.Contains(p.id)));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Merchant", Methods = "Update", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Edit([FromBody]CmsMerchant parm)
        {
            return Ok(await merchantService.UpdateAsync(parm));
        }

        [HttpPost("agentbyguid")]
        public async Task<IActionResult> GetAgentbyGuid([FromBody]ParmString obj)
        {
            var guid = obj.parm;
            return Ok((await agentService.GetModelAsync(t => t.Admin_Guid == guid)).data);
        }
    }
}
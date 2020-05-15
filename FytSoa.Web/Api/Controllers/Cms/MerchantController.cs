using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
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
        public MerchantController(ICmsMerchantService merchantService)
        {
            this.merchantService = merchantService;
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

        [HttpPost("add"), ApiAuthorize(Modules = "Level", Methods = "Add", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Add([FromBody]CmsLevel parm)
        {
            return null;
            //return Ok(await levelService.AddAsync(parm));
        }

        [HttpPost("delete"), ApiAuthorize(Modules = "Level", Methods = "Delete", LogType = LogEnum.DELETE)]
        public async Task<IActionResult> Delete([FromBody]ParmString obj)
        {
            var list = Utils.StrToListInt(obj.parm);
            return null;
            //return Ok(await levelService.DeleteAsync(p => list.Contains(p.Id)));
        }

        [HttpPost("edit"), ApiAuthorize(Modules = "Level", Methods = "Update", LogType = LogEnum.ADD)]
        public async Task<IActionResult> Edit([FromBody]CmsLevel parm)
        {
            return null;
            //return Ok(await levelService.UpdateAsync(parm));
        }
    }
}
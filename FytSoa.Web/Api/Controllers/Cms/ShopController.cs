using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers.Cms
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [JwtAuthorize(Roles = "Admin")]
    public class ShopController : ControllerBase
    {
        readonly ICmsShopService shopService;
        readonly ICmsDeviceService deviceService;
        readonly ICmsStaffService staffService;

        public ShopController(ICmsShopService shopService, ICmsDeviceService deviceService, ICmsStaffService staffService)
        {
            this.shopService = shopService;
            this.deviceService = deviceService;
            this.staffService = staffService;
        }

        [HttpGet("getpages")]
        public async Task<IActionResult> GetPages([FromQuery]PageParm parm)
        {
            if (!await HttpContext.IsSystem())
            {
                parm.CreateBy = await HttpContext.LoginUserId();
            }

            var res = await shopService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpGet("device")]
        public async Task<IActionResult> GetDevice([FromQuery]PageParm parm)
        {
            var res = await deviceService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff([FromQuery]PageParm parm)
        {
            var res = await staffService.GetPagesAsync(parm);

            return Ok(new { code = 0, msg = "success", count = res.data.TotalItems, data = res.data.Items });
        }

        [HttpPost("getbymch")]
        public async Task<IActionResult> GetByMerchant([FromBody]GetByMerchantRequest req)
        {
            var res = await shopService.GetByAdminGuidAsync(await HttpContext.LoginUserId(), req.out_sub_mch_id, req.key, 10);

            return Ok(res.Select(p => new { name = p.shop_name, value = p.out_shop_id }));
        }
    }
}
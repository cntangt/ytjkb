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
    public class ShopController : ControllerBase
    {
        private readonly ICmsShopService shopService;

        public ShopController(ICmsShopService shopService)
        {
            this.shopService = shopService;
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
    }
}
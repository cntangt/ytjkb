using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using FytSoa.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class ShopModel : PageModel
    {
        ICmsMerchantService merchantService;

        public ShopModel(ICmsMerchantService merchantService)
        {
            this.merchantService = merchantService;
        }

        public async Task OnGetAsync()
        {
            var parm = new PageParm
            {
                page = 1,
                limit = 10000
            };

            if (!await HttpContext.IsSystem())
            {
                parm.CreateBy = await HttpContext.LoginUserId();
            }

            var list = await merchantService.GetPagesAsync(parm);

            if (list.data != null && list.data.Items != null)
            {
                ViewData["merchants"] = list.data.Items.Select(p => new SelectListItem
                {
                    Text = p.name,
                    Value = p.sub_out_mch_id
                }).ToList();
            }
        }
    }
}
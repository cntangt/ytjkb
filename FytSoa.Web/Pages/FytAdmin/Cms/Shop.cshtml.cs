using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using FytSoa.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using FytSoa.Service.Interfaces;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class ShopModel : PageModel
    {
        ICmsMerchantService merchantService;
        ISysRoleService roleService;

        public ShopModel(ICmsMerchantService merchantService, ISysRoleService roleService)
        {
            this.merchantService = merchantService;
            this.roleService = roleService;
        }

        public bool Display = false;

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
                    Value = p.out_sub_mch_id
                }).ToList();
            }

            var role_info = await roleService.GetRoleByAdminGuid(await HttpContext.LoginUserId());

            if (role_info.isSystem || role_info.isAgent)
            {
                Display = true;
            }
        }
    }
}
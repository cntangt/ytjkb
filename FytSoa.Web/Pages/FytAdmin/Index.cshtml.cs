using FytSoa.Core.Model.Sys;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ISysMenuService _sysMenuService;

        public IndexModel(ISysMenuService sysMenuService)
        {
            _sysMenuService = sysMenuService;
        }

        [BindProperty]
        public List<SysMenu> list { get; set; }

        [BindProperty]
        public string adminGuid { get; set; }

        public async Task OnGetAsync()
        {
            var page = await _sysMenuService.GetPagesAsync(new Service.DtoModel.PageParm() { limit = 1000 });

            list = page.data.Items;

            adminGuid = User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.Sid).Value;
        }
    }
}
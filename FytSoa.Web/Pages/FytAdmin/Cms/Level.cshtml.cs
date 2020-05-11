using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class LevelModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
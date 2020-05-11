using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FytSoa.Web.Pages.FytAdmin.Sys
{
    [Authorize]
    public class LogModel : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}
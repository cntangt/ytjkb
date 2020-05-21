using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FytSoa.Web.Pages.FytAdmin.Sys
{
    public class AdminToShopModel : PageModel
    {
        [BindProperty]
        public string admin_guid { get; set; }

        public void OnGet(string guid)
        {
            admin_guid = guid;
        }
    }
}
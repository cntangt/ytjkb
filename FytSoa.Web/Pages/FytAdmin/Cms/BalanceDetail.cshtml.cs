using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FytSoa.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class BalanceDetailModel : PageModel
    {
        public bool Dispaly { get; set; }
        public string BtnName { get; set; }

        public async Task OnGet(string billid, int status)
        {
            var isSys = await HttpContext.IsSystem();

            if ((isSys && (status == 0 || status == 2)) || (!isSys && status == 1))
            {
                Dispaly = true;
            }
            else
            {
                Dispaly = false;
            }

            //状态对应的下一步操作
            if (status == 0)
            {
                BtnName = "审核";
            }
            else if (status == 1)
            {
                BtnName = "确认";
            }
            else if (status == 2)
            {
                BtnName = "结算";
            }
        }
    }
}
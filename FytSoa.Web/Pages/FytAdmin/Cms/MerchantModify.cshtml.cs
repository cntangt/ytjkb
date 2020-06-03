using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Extensions;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class MerchantModifyModel : PageModel
    {
        private readonly ICmsMerchantService merchantService;
        private readonly ICmsAgentService agentService;

        public MerchantModifyModel(ICmsMerchantService merchantService, ICmsAgentService agentService)
        {
            this.merchantService = merchantService;
            this.agentService = agentService;
        }

        [BindProperty]
        public CmsMerchant Merchant { get; set; }
        public List<CmsAgent> CmsAgents { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            Merchant = (await merchantService.GetModelAsync(m => m.id == id)).data;

            if (await HttpContext.IsSystem())
            {
                CmsAgents = (await agentService.GetListAsync()).data;
            }
            else
            {
                var guid = await HttpContext.LoginUserId();
                CmsAgents = (await agentService.GetListAsync(t => t.Admin_Guid == guid, t => t.Create_Time, Common.DbOrderEnum.Asc)).data;
            }
        }
    }
}
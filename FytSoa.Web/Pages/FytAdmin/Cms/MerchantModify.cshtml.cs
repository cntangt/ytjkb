using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class MerchantModifyModel : PageModel
    {
        private readonly ICmsMerchantService merchantService;

        public MerchantModifyModel(ICmsMerchantService merchantService)
        {
            this.merchantService = merchantService;
        }

        [BindProperty]
        public CmsMerchant Merchant { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            Merchant = (await merchantService.GetModelAsync(m => m.id == id)).data;
        }
    }
}
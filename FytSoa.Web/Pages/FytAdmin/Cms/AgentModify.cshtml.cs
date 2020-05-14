using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class AgentModifyModel : PageModel
    {
        private readonly ICmsAgentService _agent;

        public AgentModifyModel(ICmsAgentService agent)
        {
            _agent = agent;
        }

        [BindProperty]
        public CmsAgent Agent { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            Agent = (await _agent.GetModelAsync(m => m.id == id)).data;
        }
    }
}
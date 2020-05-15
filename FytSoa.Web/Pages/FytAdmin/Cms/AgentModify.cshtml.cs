using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class AgentModifyModel : PageModel
    {
        private readonly ICmsAgentService _agent;
        private readonly ICmsLevelService _level;

        public AgentModifyModel(ICmsAgentService agent, ICmsLevelService level)
        {
            _agent = agent;
            _level = level;
        }

        [BindProperty]
        public CmsAgent Agent { get; set; }
        public List<CmsLevel> Levels { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            Agent = (await _agent.GetModelAsync(m => m.Id == id)).data;
            Levels = (await _level.GetListAsync()).data;
        }
    }
}
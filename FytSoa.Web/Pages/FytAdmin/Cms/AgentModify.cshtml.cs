using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class AgentModifyModel : PageModel
    {
        private readonly ICmsAgentService _agent;
        private readonly ICmsLevelService _level;
        private readonly ISysAdminService _admin;

        public AgentModifyModel(ICmsAgentService agent, ICmsLevelService level,ISysAdminService admin)
        {
            _agent = agent;
            _admin = admin;
            _level = level;
        }

        [BindProperty]
        public CmsAgent Agent { get; set; }
        public List<CmsLevel> Levels { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            if (id != 0)
            {
                Agent = (await _agent.GetModelAsync(m => m.Id == id)).data;
                Agent.LoginName = (await _admin.GetModelAsync(m => m.Guid == Agent.Admin_Guid)).data.LoginName;
            }
            else
            {
                Agent = new CmsAgent();
            }

            Levels = (await _level.GetListAsync()).data;
        }
    }
}
using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class LevelModifyModel : PageModel
    {
        private readonly ICmsLevelService _level;

        public LevelModifyModel(ICmsLevelService level)
        {
            _level = level;
        }

        [BindProperty]
        public CmsLevel Level { get; set; }

        public async Task OnGetAsync(int id = 0)
        {
            Level = (await _level.GetModelAsync(m => m.Id == id)).data;
        }
    }
}
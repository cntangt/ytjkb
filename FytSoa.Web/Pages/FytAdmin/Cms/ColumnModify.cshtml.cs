using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class ColumnModifyModel : PageModel
    {
        private readonly ICmsColumnService _columnService;
        private readonly ICmsTemplateService _templateService;
        private IDistributedCache _cache;

        public ColumnModifyModel(ICmsColumnService columnService, ICmsTemplateService templateService, IDistributedCache cache)
        {
            _columnService = columnService;
            _templateService = templateService;
            _cache = cache;
        }

        [BindProperty]
        public CmsColumn Column { get; set; }

        public List<CmsTemplate> TempList { get; set; }

        public List<CmsColumn> ColumnList { get; set; }

        public async Task OnGet(int id = 0, int parent = 0)
        {
            Column = (await _columnService.GetModelAsync(m => m.Id == id)).data;

            if (Column.Id == 0 && parent != 0)
            {
                Column.ParentId = parent;
            }


            TempList = (await _templateService.GetListAsync(m => true, m => m.AddDate, DbOrderEnum.Asc)).data;

            var siteGuid = await _cache.SiteGuidAsync();

            var list = _columnService.RecursiveModule(_columnService.GetListAsync(m => m.SiteGuid == siteGuid, m => m.Sort, DbOrderEnum.Asc).Result.data);
            foreach (var item in list)
            {
                item.Title = Utils.LevelName(item.Title, item.ClassLayer);
            }
            ColumnList = list;
        }
    }
}
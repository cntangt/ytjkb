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
    public class ArticleModifyModel : PageModel
    {
        private readonly ICmsColumnService _columnService;
        private readonly ICmsArticleService _articleervice;
        private readonly IDistributedCache _cache;

        public ArticleModifyModel(ICmsColumnService columnService, ICmsArticleService articleervice, IDistributedCache cache)
        {
            _columnService = columnService;
            _articleervice = articleervice;
            _cache = cache;
        }

        [BindProperty]
        public CmsArticle Article { get; set; }

        public List<CmsColumn> ColumnList { get; set; }

        public async Task OnGetAsync(int id = 0, int column = 0)
        {
            Article = (await _articleervice.GetModelAsync(m => m.Id == id)).data;
            if (Article.Id == 0 && column != 0)
            {
                Article.ColumnId = column;
            }

            var siteGuid = await _cache.SiteGuidAsync();
            var columns = (await _columnService.GetListAsync(m => m.SiteGuid == siteGuid, m => m.Sort, DbOrderEnum.Asc)).data;

            var list = _columnService.RecursiveModule(columns);
            foreach (var item in list)
            {
                item.Title = Utils.LevelName(item.Title, item.ClassLayer);
            }

            ColumnList = list;
        }
    }
}
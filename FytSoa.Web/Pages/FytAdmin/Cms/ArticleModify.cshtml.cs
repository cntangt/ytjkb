using FytSoa.Common;
using FytSoa.Core;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class ArticleModifyModel : BasePageModel
    {
        private readonly ICmsColumnService _columnService;
        private readonly ICmsArticleService _articleervice;

        public ArticleModifyModel(ICmsColumnService columnService, ICmsArticleService articleervice, IDistributedCache cache) : base(cache)
        {
            _columnService = columnService;
            _articleervice = articleervice;
        }

        [BindProperty]
        public CmsArticle Article { get; set; }

        public List<CmsColumn> ColumnList { get; set; }

        public void OnGet(int id = 0, int column = 0)
        {
            Article = _articleervice.GetModelAsync(m => m.Id == id).Result.data;
            if (Article.Id == 0 && column != 0)
            {
                Article.ColumnId = column;
            }
            var list = _columnService.RecursiveModule(_columnService.GetListAsync(m => m.SiteGuid == SiteGuid, m => m.Sort, DbOrderEnum.Asc).Result.data);
            foreach (var item in list)
            {
                item.Title = Utils.LevelName(item.Title, item.ClassLayer);
            }
            ColumnList = list;
        }
    }
}
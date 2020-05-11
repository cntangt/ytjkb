using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    [Authorize]
    public class ColumnModifyModel : BasePageModel
    {
        private readonly ICmsColumnService _columnService;
        private readonly ICmsTemplateService _templateService;

        public ColumnModifyModel(ICmsColumnService columnService, ICmsTemplateService templateService, IDistributedCache cache) : base(cache)
        {
            _columnService = columnService;
            _templateService = templateService;
        }

        [BindProperty]
        public CmsColumn Column { get; set; }

        public List<CmsTemplate> TempList { get; set; }

        public List<CmsColumn> ColumnList { get; set; }

        public void OnGet(int id = 0, int parent = 0)
        {
            Column = _columnService.GetModelAsync(m => m.Id == id).Result.data;
            if (Column.Id == 0 && parent != 0)
            {
                Column.ParentId = parent;
            }
            TempList = _templateService.GetListAsync(m => true, m => m.AddDate, Common.DbOrderEnum.Asc).Result.data;

            var list = _columnService.RecursiveModule(_columnService.GetListAsync(m => m.SiteGuid == SiteGuid, m => m.Sort, DbOrderEnum.Asc).Result.data);
            foreach (var item in list)
            {
                item.Title = Utils.LevelName(item.Title, item.ClassLayer);
            }
            ColumnList = list;
        }
    }
}
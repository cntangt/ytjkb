using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Distributed;

namespace FytSoa.Api.Controllers.H5
{
    [Route("api/h5")]
    [Produces("application/json")]
    [EnableCors("Any")]
    public class IndexController : Controller
    {
        private readonly ICmsAdvListService _listService;
        private readonly ICmsArticleService _articleService;
        private readonly ICmsColumnService _columnService;
        private readonly ICmsSiteService _siteService;
        private readonly IDistributedCache _cache;

        public IndexController(ICmsAdvListService listService
            , ICmsArticleService articleService
            , ICmsColumnService columnService
            , ICmsSiteService siteService
            , IDistributedCache cache)
        {
            _listService = listService;
            _articleService = articleService;
            _columnService = columnService;
            _siteService = siteService;
            _cache = cache;
        }

        [HttpPost("index")]
        public async Task<JsonResult> GetIndex()
        {
            var site = await _cache.GetAsync(CacheKey.WEBCMSSITE, _siteService.GetModelAsync(m => m.Guid == "78756a6c-50c8-47a5-b898-5d6d24a20327"));
            var column = await _cache.GetAsync(CacheKey.WEBCMSCOLUMN, _columnService.GetListAsync(m => true, m => m.Sort, DbOrderEnum.Asc));

            //查询焦点图
            var banner = _listService.GetListAsync(m => m.ClassGuid == "e8b4325d-bdd8-448f-83be-034d66642b14" && m.Status, m => m.Sort, DbOrderEnum.Desc).Result.data.Select(m => new
            {
                linkUrl = m.LinkUrl,
                title = m.Title,
                imgUrl = m.ImgUrl
            }).ToList();

            //查询案例，按权重和日期排序
            var caseColumn = column.data.Where(m => m.ParentId == 1015).ToList();
            var Case = _articleService.WebGetList(new PageParm() { limit = 10, types = 1, where = "istop=1" }, caseColumn.Select(m => m.Id).ToList()).Items.Select(m => new
            {
                id = m.Id,
                title = m.Title,
                imgUrl = m.ImgUrl,
                tag = m.Tag
            }).ToList();

            //查询新闻，按权重和日期排序
            var articleColumn = column.data.Where(m => m.ParentId == 1016).ToList();
            var Article = _articleService.WebGetList(new PageParm() { limit = 4, types = 1 }, articleColumn.Select(m => m.Id).ToList()).Items.Select(m => new
            {
                id = m.Id,
                title = m.Title,
                imgUrl = m.ImgUrl,
                addDate = m.AddDate
            }).ToList();

            return Json(new
            {
                banner,
                cases = Case,
                article = Article,
                site = new { title = site.data.SeoTitle, key = site.data.SeoKey, desc = site.data.SeoDescribe }
                ,
                newcolumn = articleColumn.Select(m => new { m.Id, m.Title }).ToList()
                ,
                casecolumn = caseColumn.Select(m => new { m.Id, m.Title }).ToList()
            });
        }

        /// <summary>
        /// 根据ID查询案例/新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public JsonResult GetDetail(int id = 0)
        {
            var model = _articleService.GetModelAsync(m => m.Id == id).Result.data;
            if (model == null)
            {
                model = new CmsArticle();
            }
            return Json(new { model.Title, model.Summary, model.Content });
        }

        /// <summary>
        /// 根据栏目ID查询列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cid">栏目id</param>
        /// <param name="type">case=案例  news=新闻</param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<JsonResult> GetList(int page = 1, int cid = 0, string type = "case")
        {
            var _column = await _cache.GetAsync(CacheKey.WEBCMSCOLUMN, _columnService.GetListAsync(m => true, m => m.Sort, DbOrderEnum.Asc), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            });

            var where = cid == 0 ? "" : "columnId=" + cid;
            if (type == "case")
            {
                var caseColumn = _column.data.Where(m => m.ParentId == 1015).Select(m => m.Id).ToList();
                var query = _articleService.WebGetList(new PageParm() { limit = 20, page = page, types = 1, where = where }, caseColumn);
                var list = query.Items.Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    imgUrl = m.ImgUrl,
                    tag = m.Tag
                }).ToList();
                return Json(new { data = list, total = query.TotalPages });
            }
            if (type == "news")
            {
                var column = _column.data.Where(m => m.ParentId == 1016).Select(m => m.Id).ToList();
                var query = _articleService.WebGetList(new PageParm() { limit = 20, page = page, types = 1, where = where }, column);
                var list = query.Items.Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    imgUrl = m.ImgUrl,
                    addDate = m.AddDate
                }).ToList();
                return Json(new { data = list, total = query.TotalPages });
            }
            return Json(new { data = new { }, total = 0 });
        }
    }
}
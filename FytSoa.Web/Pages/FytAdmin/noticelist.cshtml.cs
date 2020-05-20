using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin.Cms
{
    public class NoticeListModel : PageModel
    {
        private readonly ISysNoticeService _notice;

        public NoticeListModel(ISysNoticeService notice)
        {
            _notice = notice;
        }

        [BindProperty]
        public List<SysNoticeChi> NoticeList { get; set; }

        public async Task OnGetAsync()
        {
            NoticeList = (await _notice.GetNoticeList(await HttpContext.LoginUserId())).data.OrderBy(t => t.read_status).ToList();
        }
    }
}
using FytSoa.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Web.Pages.FytAdmin
{

    public class LoginModel : PageModel
    {
        IDistributedCache _cache;

        public LoginModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        [BindProperty]
        public List<string> RsaKey { get; set; }
        public Guid lid { get; set; }
        public async Task OnGetAsync()
        {
            var auth = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (auth.Status.ToString() != "Faulted")
            {
                RedirectToPage("Index");
            }

            RsaKey = RSACrypt.GetKey();
            lid = Guid.NewGuid();

            //获得公钥和私钥
            await _cache.SetAsync($"LOGINKEY:{lid}", RsaKey, expirationMinute: 30);
        }

    }
}
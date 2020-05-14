using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FytSoa.Web.Api.Controllers.Cms
{
    public class AgentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
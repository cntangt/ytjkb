using FytSoa.Common;
using FytSoa.Core.Model.Wx;
using FytSoa.Extensions;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Cms;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : Controller
    {
        readonly IWxCloudService wx;
        readonly ICmsMerchantService merchantService;
        readonly ICmsSettlementService settlementService;
        readonly IConfiguration config;

        public ChartController(IWxCloudService wx, ICmsMerchantService merchantService, ICmsSettlementService settlementService, IConfiguration config)
        {
            this.wx = wx;
            this.merchantService = merchantService;
            this.settlementService = settlementService;
            this.config = config;
        }

        public async Task<IndexModel> Info(int day = 7)
        {
            if (day > 30)
            {
                day = 30;
            }

            var admin_guid = await HttpContext.LoginUserId();
            var start = DateTime.Now.AddDays(-day - 1).Date;
            var end = DateTime.Now.Date;

            return new IndexModel
            {
                PlatformInfos = await settlementService.GetPlatformInfoAsync(admin_guid, start, end),
                Trends = await settlementService.GetTrendAsync(admin_guid, start, end)
            };
        }
    }
}
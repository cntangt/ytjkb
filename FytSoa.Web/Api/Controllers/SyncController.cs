using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SyncController : Controller
    {
        readonly IWxCloudService wx;

        public SyncController(IWxCloudService wx)
        {
            this.wx = wx;
        }

        public async Task OrderList()
        {
            var req = new QueryOrderListRequest
            {
                order_type = OrderType.全部单据,
                start_time = DateTime.Now.Date.AddDays(-1),
                end_time = DateTime.Now.Date,
                page_num = 1,
                page_size = 10,
                out_sub_mch_id = "sz01ELTR281OFpmdAp6J",
                AuthenKey = "lSCp1M5grGWFD7rJzaZaqixsvOhORp2P"
            };

            var res = await wx.QueryAsync(req);
        }

    }
}
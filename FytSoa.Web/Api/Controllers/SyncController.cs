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

        public Task<WxResponse<QueryOrderListResponse>> OrderList()
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

            return wx.QueryAsync(req);
        }

        public Task<WxResponse<QueryShopInfoResponse>> ShopInfo()
        {
            var req = new QueryShopInfoRequest
            {
                page_num = 1,
                page_size = 10,
                out_sub_mch_id = "sz01ELTR281OFpmdAp6J",
                out_mch_id = "sz013NzuonO6CMJd0rCB",
                AuthenKey = "lSCp1M5grGWFD7rJzaZaqixsvOhORp2P"
            };

            return wx.QueryAsync(req);
        }

    }
}
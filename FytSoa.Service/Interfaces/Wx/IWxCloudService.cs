using FytSoa.Common;
using FytSoa.Service.DtoModel.Wx;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces.Wx
{
    public interface IWxCloudService
    {
        Task<WxResponse<T>> QueryAsync<T>(WxRequest<T> req);

        Task<string> SyncShopInfo(int id);
    }
}

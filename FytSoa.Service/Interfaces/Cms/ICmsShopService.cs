using FytSoa.Core.Model.Wx;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsShopService : IBaseService<ShopInfo>
    {
        Task<IEnumerable<ShopInfo>> GetByAdminGuidAsync(string admin_guid, string out_sub_mch_id, string key, int limit);
    }
}
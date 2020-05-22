using FytSoa.Core.Model.Wx;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsStaffService : IBaseService<StaffInfo>
    {
        Task<IEnumerable<StaffInfo>> GetByShop(string admin_guid, string key, string out_sub_mch_id, string out_shop_id, int limit);
    }
}
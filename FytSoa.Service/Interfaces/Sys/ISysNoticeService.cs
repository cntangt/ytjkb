using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ISysNoticeService : IBaseService<SysNotice>
    {
        Task<ApiResult<string>> GetUnreadQuantity(string admin_guid);
    }
}

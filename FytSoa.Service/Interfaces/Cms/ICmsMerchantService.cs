using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces.Cms
{
    public interface ICmsMerchantService : IBaseService<CmsMerchant>
    {
        Task<ApiResult<string>> UpdateStatusAsync(CmsMerchant parm);
    }
}

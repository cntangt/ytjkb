using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsBalanceService : IBaseService<CmsBalance>
    {
        Task<IEnumerable<CmsBalance_Chi>> GetTotalAmountAsync(string billId);

        Task<ApiResult<Page<CmsBalance_Chi>>> GetDetailPageAsync(PageParm parm);

        Task<ApiResult<string>> ModifyAmountAsync(CmsBalance_Chi parm);
    }
}
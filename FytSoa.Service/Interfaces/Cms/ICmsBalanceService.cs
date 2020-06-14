using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsBalanceService : IBaseService<CmsBalance>
    {
        Task<IEnumerable<CmsBalance_Chi>> GetTotalAmountAsync(string billId);
    }
}
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsSettlementService : IBaseService<CmsDailySettlement>
    {
        Task<ApiResult<string>> DailyJobAsync(DateTime day);
        Task<ApiResult<string>> MonthlyJobAsync(DateTime day);
    }
}
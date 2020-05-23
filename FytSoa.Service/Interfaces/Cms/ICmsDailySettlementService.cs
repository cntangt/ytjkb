using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsDailySettlementService : IBaseService<CmsDailySettlement>
    {
        Task<ApiResult<string>> DailyJobAsync(DateTime day);
    }
}
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel.Cms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsSettlementService : IBaseService<CmsDailySettlement>
    {
        Task<ApiResult<string>> DailyJobAsync(DateTime day);
        Task<ApiResult<string>> MonthlyJobAsync(DateTime day);
        Task<IEnumerable<PlatformInfo>> GetPlatformInfoAsync(string admin_guid, DateTime start, DateTime end);
    }
}
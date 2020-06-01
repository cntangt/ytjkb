using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel.Cms;
using FytSoa.Service.DtoModel.Wx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsSettlementService : IBaseService<CmsOrderOverview>
    {
        Task<ApiResult<string>> DailyJobAsync(DateTime day);
        Task<ApiResult<string>> MonthlyJobAsync(DateTime day);
        Task<IEnumerable<PlatformInfo>> GetPlatformInfoAsync(string admin_guid, DateTime start, DateTime end);
        Task<IEnumerable<TrendInfo>> GetTrendAsync(string admin_guid, DateTime start, DateTime end);
        Task<ApiResult<Page<CmsOrderOverview>>> GetShopDailyReport(QueryOrderListRequest parm);
        Task<ApiResult<Page<CmsOrderOverview>>> GetAgentTradeSummary(QueryOrderListRequest parm);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Service.DtoModel;

namespace FytSoa.Service.Interfaces
{
	public interface ICmsAgentService : IBaseService<CmsAgent>
	{
        /// <summary>
        /// 获得列表
        /// </summary>
        Task<ApiResult<List<AgentDto>>> GetAgentListAsync(bool Async = true);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        Task<ApiResult<string>> AddAgentAsync(CmsAgent parm, bool Async = true);
    }
}
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsAgentService : IBaseService<CmsAgent>
    {
        Task<ApiResult<string>> UpdateStatusAsync(CmsAgent agent);
    }
}
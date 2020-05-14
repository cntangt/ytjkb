using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    public class CmsAgentService : BaseService<CmsAgent>, ICmsAgentService
    {
        public CmsAgentService(IConfiguration config) : base(config)
        {
        }
    }
}
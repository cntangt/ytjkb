using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    public class CmsLevelService : BaseService<CmsLevel>, ICmsLevelService
    {
        public CmsLevelService(IConfiguration config) : base(config)
        {
        }
    }
}
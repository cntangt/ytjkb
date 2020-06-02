using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsSiteService : BaseService<CmsSite>, ICmsSiteService
    {
        public CmsSiteService(IConfiguration config) : base(config)
        {
        }

        public Task<CmsSite> DefaultAsync() => Db.Queryable<CmsSite>().FirstAsync();
    }
}

using FytSoa.Core.Model.Cms;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    public interface ICmsSiteService : IBaseService<CmsSite>
    {
        Task<CmsSite> DefaultAsync();
    }
}

using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    /// <summary>
    /// 角色业务接口
    /// </summary>
    public interface ISysRoleService : IBaseService<SysRole>
    {
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Page<SysRole>>> GetPagesAsync(PageParm parm);

        Task<ApiResult<Page<SysRoleDto>>> GetPagesToRoleAsync(PageParm parm);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync(SysRole parm);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ModifyAsync(SysRole parm);

        Task<(bool isSystem, bool isAgent, bool isSubAdmin)> GetRoleByAdminGuid(string admin_guid);
    }
}

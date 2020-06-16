using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Sys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces
{
    /// <summary>
    /// 管理员接口
    /// </summary>
    public interface ISysAdminService : IBaseService<SysAdmin>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        Task<ApiResult<SysAdminMenuDto>> LoginAsync(SysAdminLogin parm);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Page<SysAdmin>>> GetPagesAsync(PageParm parm);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync(SysAdmin parm);

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync(string parm);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ModifyAsync(SysAdmin parm);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> UpdatePwdAsync(UpdatePwdDto parm);

        /// <summary>
        /// 获取用户门店权限
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<List<ShopInfo>>> GetShopsAsync(string admin_guid);

        /// <summary>
        /// 添加用户门店权限
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddShopsAsync(AdminShopRel parm);

        /// <summary>
        /// 更新登录次数
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> UpdateLoginSum(string admin_guid);
    }
}

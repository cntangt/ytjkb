using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    /// <summary>
    /// 角色功能实现
    /// </summary>
    public class SysRoleService : BaseService<SysRole>, ISysRoleService
    {
        public SysRoleService(IConfiguration config) : base(config)
        {
        }

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> AddAsync(SysRole parm)
        {
            var res = new ApiResult<string>() { data = "1", statusCode = 200 };
            try
            {
                parm.Guid = Guid.NewGuid().ToString();
                parm.EditTime = DateTime.Now;
                parm.AddTime = DateTime.Now;
                //if (parm.Level == 1)
                //{
                //    //根据部门ID查询部门
                //    var organizeModel = SysOrganizeDb.GetById(parm.DepartmentGuid);
                //    parm.DepartmentGroup = organizeModel.ParentGuidList;
                //}
                await Db.Insertable(parm).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
            }
            return res;
        }


        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Page<SysRole>>> GetPagesAsync(PageParm parm)
        {
            var res = new ApiResult<Page<SysRole>>();
            try
            {
                var groups = await Db.Queryable<SysRole>()
                    .Where(p => p.Level == 0)
                    .ToListAsync();

                var list = await Db.Queryable<SysRole>()
                        .Where(p => p.Level > 0)
                        .WhereIF(!string.IsNullOrEmpty(parm.CreateBy), p => p.CreateBy == parm.CreateBy || p.IsPublic)
                        .OrderBy(m => m.Sort, OrderByType.Desc)
                        .OrderBy(m => m.AddTime, OrderByType.Desc)
                        .ToPageAsync(parm.page, parm.limit);

                list.Items.ForEach(p =>
                {
                    var group = groups.FirstOrDefault(g => g.Guid == p.ParentGuid);
                    if (group != null)
                    {
                        p.DepartmentGroup = group.Name;
                    }
                });

                res.data = list;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Page<SysRoleDto>>> GetPagesToRoleAsync(PageParm parm)
        {
            var res = new ApiResult<Page<SysRoleDto>>();
            try
            {
                var reslist = await Db.Queryable<SysRole>()
                        .Where(p => p.Level > 0)
                        .WhereIF(!string.IsNullOrEmpty(parm.CreateBy), p => p.CreateBy == parm.CreateBy)
                        .OrderBy(m => m.Sort, OrderByType.Desc)
                        .OrderBy(m => m.AddTime, OrderByType.Desc)
                        .Select(it => new SysRoleDto()
                        {
                            guid = it.Guid,
                            name = it.Name,
                            DepartmentGroup = it.DepartmentGroup,
                            ParentGuid = it.ParentGuid,
                            sort = it.Sort,
                            level = it.Level,
                            codes = it.Codes,
                        })
                        .ToPageAsync(parm.page, parm.limit);

                var ps = await Db.Queryable<SysPermissions>().Where(m => m.AdminGuid == parm.guid && m.Types == 2).ToListAsync();

                reslist.Items.ForEach(p =>
                {
                    p.status = ps.Any(q => p.guid == q.RoleGuid);
                });

                res.data = reslist;
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        public async Task<(bool isSystem, bool isAgent, bool isSubAdmin)> GetRoleByAdminGuid(string admin_guid)
        {
            var roles = await Db.Queryable<SysAdmin, SysPermissions, SysRole>((admin, perm, role) => new JoinQueryInfos(
                 JoinType.Left, admin.Guid == perm.AdminGuid,
                 JoinType.Left, perm.RoleGuid == role.Guid))
              .Where((admin, perm, role) => admin.Guid == admin_guid)
              .Select((admin, perm, role) => role)
              .ToListAsync();

            var isSystem = roles.Any(p => p.IsSystem);
            var isAgent = roles.Any(p => p.Guid == "72171cf0-934d-4934-8e27-ee4f47e9985e");
            var is_sub_admin = roles.Any(p => p.Guid == "8dc9b479-216d-415a-9fba-85caedd6c4df");

            return (isSystem, isAgent, is_sub_admin);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> ModifyAsync(SysRole parm)
        {
            var res = new ApiResult<string>() { statusCode = 200 };
            try
            {
                parm.EditTime = DateTime.Now;
                //if (parm.Level == 1)
                //{
                //    //根据部门ID查询部门组
                //    var organizeModel = SysOrganizeDb.GetById(parm.DepartmentGuid);
                //    parm.DepartmentGroup = organizeModel.ParentGuidList;
                //}
                await Db.Updateable(parm).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
    }
}

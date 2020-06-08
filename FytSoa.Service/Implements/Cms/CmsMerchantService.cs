using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace FytSoa.Service.Implements
{
    public class CmsMerchantService : BaseService<CmsMerchant>, ICmsMerchantService
    {
        readonly ISysAdminService sysAdmin;
        readonly ISysPermissionsService sysPermissions;

        public override async Task<ApiResult<CmsMerchant>> GetModelAsync(Expression<Func<CmsMerchant, bool>> where, bool Async = true)
        {
            var data = await Db.Queryable<CmsMerchant>().FirstAsync(where);

            if (data == null)
            {
                data = new CmsMerchant();
            }
            else
            {
                var admin = await Db.Queryable<SysAdmin>().FirstAsync(p => p.Guid == data.admin_guid);
                data.admin_name = admin?.LoginName;

                var agent = await Db.Queryable<SysAdmin>().FirstAsync(p => p.Guid == data.agent_admin_guid);
                data.agent_admin_name = agent?.TrueName;
            }

            return new ApiResult<CmsMerchant>
            {
                statusCode = 200,
                data = data
            };
        }

        public CmsMerchantService(ISysAdminService sysAdmin, ISysPermissionsService sysPermissions, IConfiguration config) : base(config)
        {
            this.sysAdmin = sysAdmin;
            this.sysPermissions = sysPermissions;
        }

        public override async Task<ApiResult<Page<CmsMerchant>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<CmsMerchant>>();

            var query = Db.Queryable<CmsMerchant>()
                .WhereIF(!string.IsNullOrEmpty(parm.CreateBy), p => p.agent_admin_guid == parm.CreateBy)
                .WhereIF(!string.IsNullOrEmpty(parm.key), p => p.name.Contains(parm.key) || p.tel.Contains(parm.key) || p.contact.Contains(parm.key) || p.out_sub_mch_id.Contains(parm.key));

            res.data = await query.OrderBy(p => p.create_time, SqlSugar.OrderByType.Desc).ToPageAsync(parm.page, parm.limit);

            if (res.data.Items.Count > 0)
            {
                var ids1 = res.data.Items.Select(p => p.admin_guid).ToArray();
                var ids2 = res.data.Items.Select(p => p.agent_admin_guid).ToArray();

                var admins = await Db.Queryable<SysAdmin>().In(p => p.Guid, ids1).ToListAsync();
                var agents = await Db.Queryable<CmsAgent>().In(p => p.Admin_Guid, ids2).ToListAsync();

                if (admins.Count > 0 || agents.Count > 0)
                {
                    res.data.Items.ForEach(p =>
                    {
                        var admin = admins.FirstOrDefault(a => a.Guid == p.admin_guid);
                        if (admin != null)
                        {
                            p.admin_name = admin.LoginName;
                        }

                        var agent = agents.FirstOrDefault(a => a.Admin_Guid == p.agent_admin_guid);
                        if (agent != null)
                        {
                            p.agent_admin_name = agent.Name;
                        }
                    });
                }
            }

            return res;
        }

        public override async Task<ApiResult<string>> AddAsync(CmsMerchant parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var query = Db.Queryable<CmsMerchant>();

                if (await query.AnyAsync(p => p.out_sub_mch_id == parm.out_sub_mch_id))
                {
                    throw new Exception($"商户子账号【{parm.out_sub_mch_id}】已经存在");
                }

                if (await query.AnyAsync(p => p.name == parm.name))
                {
                    throw new Exception($"商户名称【{parm.name}】已经存在");
                }

                var admin_guid = Guid.NewGuid().ToString();

                var roles = await Db.Queryable<SysRole>()
                    .Where(p => p.Guid == "8dc9b479-216d-415a-9fba-85caedd6c4df")
                    .Select(p => new AdminToRoleList
                    {
                        guid = p.Guid,
                        name = p.Name
                    })
                    .ToListAsync();

                using var tran = new TransactionScope();

                var adminRes = await sysAdmin.AddAsync(new SysAdmin
                {
                    AddDate = DateTime.Now,
                    CreateBy = parm.agent_admin_guid,
                    DepartmentGuid = null,
                    DepartmentGuidList = null,
                    DepartmentName = null,
                    Email = parm.email,
                    Guid = admin_guid,
                    HeadPic = null,
                    IsSystem = false,
                    LoginDate = null,
                    LoginName = parm.admin_name,
                    LoginPwd = "123456",
                    LoginSum = 0,
                    Mobile = parm.tel,
                    Number = null,
                    RoleGuid = JsonConvert.SerializeObject(roles),
                    Sex = "-",
                    Status = true,
                    Summary = null,
                    TrueName = parm.name,
                    UpLoginDate = null,
                });

                if (adminRes.statusCode != (int)ApiEnum.Status)
                {
                    throw new Exception(adminRes.message);
                }

                var authenRes = await sysPermissions.ToRoleAsync(new SysPermissions
                {
                    AdminGuid = admin_guid,
                    RoleGuid = "8dc9b479-216d-415a-9fba-85caedd6c4df",
                    status = true,
                    Types = 2
                }, true);

                if (authenRes.statusCode != (int)ApiEnum.Status)
                {
                    throw new Exception(authenRes.message);
                }
                //var relRes = await Db.Insertable(new CmsAdminMerchantRel
                //{
                //    Admin_Guid = admin_guid,
                //    out_sub_mch_id = parm.out_sub_mch_id,
                //    Out_Mch_Id = parm.out_mch_id
                //}).ExecuteCommandAsync();

                //if (relRes <= 0)
                //{
                //    throw new Exception("添加商户代理商关系失败");
                //}

                parm.create_time = DateTime.Now;
                parm.admin_guid = admin_guid;

                var dbres = await Db.Insertable(parm).ExecuteCommandAsync();

                res.data = dbres.ToString();
                res.statusCode = (int)ApiEnum.Status;

                tran.Complete();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            return res;
        }

        public override async Task<ApiResult<string>> UpdateAsync(CmsMerchant parm, bool Async = true)
        {

            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var query = Db.Queryable<CmsMerchant>();

                if (await query.AnyAsync(p => p.out_sub_mch_id == parm.out_sub_mch_id && p.id != parm.id))
                {
                    throw new Exception($"商户子账号【{parm.out_sub_mch_id}】已经存在");
                }
                if (await query.AnyAsync(p => p.name == parm.name && p.id != parm.id))
                {
                    throw new Exception($"商户名称【{parm.name}】已经存在");
                }

                var count = await Db.Updateable(parm).IgnoreColumns(p => new
                {
                    p.create_time,
                    p.admin_guid,
                    p.status
                }).ExecuteCommandAsync();

                if (count <= 0)
                {
                    throw new Exception("更新商户信息失败");
                }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            res.statusCode = (int)ApiEnum.Status;
            res.message = "更新商户信息成功";

            return res;
        }

        public async Task<ApiResult<string>> UpdateStatusAsync(CmsMerchant parm)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var count = await Db.Updateable(parm).UpdateColumns(p => new { p.status }).ExecuteCommandAsync();

                if (count <= 0)
                {
                    throw new Exception("更新状态失败");
                }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            res.statusCode = (int)ApiEnum.Status;
            res.message = "更新状态成功";

            return res;
        }
    }
}
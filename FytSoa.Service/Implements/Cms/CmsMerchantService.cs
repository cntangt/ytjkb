using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FytSoa.Service.Implements
{
    public class CmsMerchantService : BaseService<CmsMerchant>, ICmsMerchantService
    {
        readonly ISysAdminService sysAdmin;
        readonly ISysPermissionsService sysPermissions;

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
                .WhereIF(!string.IsNullOrEmpty(parm.key), p => p.name.Contains(parm.key) || p.contact.Contains(parm.key) || p.sub_out_mch_id.Contains(parm.key));

            res.data = await query.ToPageAsync(parm.page, parm.limit);

            if (res.data.Items.Count > 0)
            {
                var ids1 = res.data.Items.Select(p => p.admin_guid);
                var ids2 = res.data.Items.Select(p => p.agent_admin_guid);

                var ids = ids1.Concat(ids2).ToArray();

                var admins = await Db.Queryable<SysAdmin>().In(p => p.Guid, ids).ToListAsync();
                if (admins.Count > 0)
                {
                    res.data.Items.ForEach(p =>
                    {
                        var admin = admins.FirstOrDefault(a => a.Guid == p.admin_guid);
                        if (admin != null)
                        {
                            p.admin_name = admin.LoginName;
                        }

                        var agent = admins.FirstOrDefault(a => a.Guid == p.agent_admin_guid);
                        if (agent != null)
                        {
                            p.agent_admin_name = agent.TrueName;
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
                var count = 0;
                var query = Db.Queryable<CmsMerchant>();

                count = await query.CountAsync(p => p.sub_out_mch_id == parm.sub_out_mch_id);
                if (count > 0)
                {
                    throw new Exception($"商户子账号【{parm.sub_out_mch_id}】已经存在");
                }

                count = await query.CountAsync(p => p.name == parm.name);
                if (count > 0)
                {
                    throw new Exception($"商户名称【{parm.name}】已经存在");
                }

                var admin_guid = Guid.NewGuid().ToString();

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
                    RoleGuid = null,
                    Sex = "-",
                    Status = true,
                    Summary = null,
                    TrueName = parm.name,
                    UpLoginDate = null
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
                //    Sub_Out_Mch_Id = parm.sub_out_mch_id,
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
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Transactions;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace FytSoa.Service.Implements
{
    public class CmsAgentService : BaseService<CmsAgent>, ICmsAgentService
    {
        readonly ISysAdminService sysAdmin;
        readonly ISysPermissionsService sysPermissions;

        public CmsAgentService(ISysAdminService sysAdmin, ISysPermissionsService sysPermissions, IConfiguration config) : base(config)
        {
            this.sysAdmin = sysAdmin;
            this.sysPermissions = sysPermissions;
        }

        public override async Task<ApiResult<Page<CmsAgent>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<CmsAgent>>();

            var query = Db.Queryable<CmsAgent, CmsLevel, SysAdmin>((a, b, c) => new object[] {
                        JoinType.Left, a.Level_Id == b.Id,
                        JoinType.Left,a.Admin_Guid == c.Guid }).Select((a, b, c) => new CmsAgent
                        {
                            Id = a.Id,
                            Tel = a.Tel,
                            Name = a.Name,
                            Level_Name = b.Name,
                            Level_Id = a.Level_Id,
                            Admin_Guid = a.Admin_Guid,
                            Business_Area = a.Business_Area,
                            Settle_Type = a.Settle_Type,
                            Account_No = a.Account_No,
                            Account_Name = a.Account_Name,
                            Account_Info = a.Account_Info,
                            Wxpay = a.Wxpay,
                            Alipay = a.Alipay,
                            Otherpay = a.Otherpay,
                            Delete = a.Delete,
                            Status = a.Status,
                            Create_Time = a.Create_Time,
                            Update_Time = a.Update_Time,
                            LoginName = c.LoginName
                        })
                        .WhereIF(!string.IsNullOrEmpty(parm.key), p => p.Name.Contains(parm.key))
                        .OrderBy(a => a.Id, OrderByType.Desc);

            res.data = await query.ToPageAsync(parm.page, parm.limit);

            if (res.data.Items.Count > 0)
            {
                res.data.Items.ForEach(p =>
                {
                    p.Settle_Name = Enum.GetName(typeof(SettleType), p.Settle_Type);
                });
            }

            return res;
        }

        public override async Task<ApiResult<string>> AddAsync(CmsAgent parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var count = 0;
                var query = Db.Queryable<CmsAgent>();

                count = await query.CountAsync(p => p.Name == parm.Name);
                if (count > 0)
                {
                    throw new Exception($"代理商名称【{parm.Name}】已经存在");
                }

                count = (await sysAdmin.CountAsync(t => t.LoginName == parm.LoginName)).data.Count;
                if (count > 0)
                {
                    throw new Exception($"该登录账号【{parm.Name}】已经存在");
                }

                var admin_guid = Guid.NewGuid().ToString();

                using var tran = new TransactionScope();

                var adminRes = await sysAdmin.AddAsync(new SysAdmin
                {
                    AddDate = DateTime.Now,
                    CreateBy = parm.Curr_Admin_Guid,
                    DepartmentGuid = null,
                    DepartmentGuidList = null,
                    DepartmentName = null,
                    Email = null,
                    Guid = admin_guid,
                    HeadPic = null,
                    IsSystem = false,
                    LoginDate = null,
                    LoginName = parm.LoginName,
                    LoginPwd = DES3Encrypt.EncryptString("123456"),
                    LoginSum = 0,
                    Mobile = parm.Tel,
                    Number = null,
                    RoleGuid = null,
                    Sex = "-",
                    Status = true,
                    Summary = null,
                    TrueName = parm.Name,
                    UpLoginDate = null,
                });

                if (adminRes.statusCode != (int)ApiEnum.Status)
                {
                    throw new Exception(adminRes.message);
                }

                var authenRes = await sysPermissions.ToRoleAsync(new SysPermissions
                {
                    AdminGuid = admin_guid,
                    RoleGuid = "72171cf0-934d-4934-8e27-ee4f47e9985e",
                    status = true,
                    Types = 2
                }, true);

                if (authenRes.statusCode != (int)ApiEnum.Status)
                {
                    throw new Exception(authenRes.message);
                }

                parm.Create_Time = DateTime.Now;
                parm.Admin_Guid = admin_guid;

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

        public override async Task<ApiResult<string>> UpdateAsync(CmsAgent parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var query = Db.Queryable<CmsAgent>();

                if (await query.AnyAsync(p => p.Name == parm.Name && p.Id != parm.Id))
                {
                    throw new Exception($"代理商名称【{parm.Name}】已经存在");
                }

                parm.Update_Time = DateTime.Now;
                var count = await Db.Updateable(parm).IgnoreColumns(p => new
                {
                    p.Admin_Guid,
                    p.Create_Time
                }).ExecuteCommandAsync();

                if (count <= 0)
                {
                    throw new Exception("更新代理商信息失败");
                }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            res.statusCode = (int)ApiEnum.Status;
            res.message = "更新代理商信息成功";

            return res;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace FytSoa.Service.Implements
{
    public class CmsAgentService : BaseService<CmsAgent>, ICmsAgentService
    {
        public CmsAgentService(IConfiguration config) : base(config)
        {
        }

        public async Task<ApiResult<string>> AddAgentAsync(CmsAgent parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var _db = Db;
                var dbres = 0;
                _db.BeginTran();
                try
                {
                    //添加代理商
                    dbres = Async ? await _db.Insertable<CmsAgent>(parm).ExecuteCommandAsync() : _db.Insertable<CmsAgent>(parm).ExecuteCommand();
                    //生成登陆账号
                    var sysAdmin = new SysAdmin
                    {
                        AddDate = DateTime.Now,
                        RoleGuid = "72171cf0-934d-4934-8e27-ee4f47e9985e",
                        LoginName = "",
                        Mobile = parm.Tel,
                        LoginPwd = "123456",
                        Status = true,
                        IsSystem = false,
                        Guid = ""
                    };
                    dbres = Async ? await _db.Insertable<SysAdmin>(sysAdmin).ExecuteCommandAsync() : _db.Insertable<SysAdmin>(sysAdmin).ExecuteCommand();
                    //提交事务
                    _db.CommitTran();

                    res.data = dbres.ToString();
                    res.statusCode = (int)ApiEnum.Status;
                }
                catch (Exception ex)
                {
                    _db.RollbackTran();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        public async Task<ApiResult<List<AgentDto>>> GetAgentListAsync(bool Async = true)
        {
            var res = new ApiResult<List<AgentDto>>();
            try
            {
                var query = Db.Queryable<CmsAgent, CmsLevel>((a, b) => new object[] {
                        JoinType.Left, a.Level_Id == b.Id }).Select((a, b) => new AgentDto
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
                            Update_Time = a.Update_Time
                        }).OrderBy(t => t.Id, OrderByType.Desc);
                res.data = Async ? await query.ToListAsync() : query.ToList();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
    }
}
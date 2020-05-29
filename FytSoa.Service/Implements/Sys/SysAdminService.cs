﻿using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Sys;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FytSoa.Service.Implements
{
    public class SysAdminService : BaseService<SysAdmin>, ISysAdminService
    {
        public SysAdminService(IConfiguration config) : base(config)
        {
        }

        #region  用户登录和授权菜单查询
        /// <summary>
        /// 用户登录实现
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<SysAdminMenuDto>> LoginAsync(SysAdminLogin parm)
        {
            var res = new ApiResult<SysAdminMenuDto>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var adminModel = new SysAdminMenuDto();
                parm.password = DES3Encrypt.EncryptString(parm.password);
                var model = await Db.Queryable<SysAdmin>()
                        .Where(m => m.LoginName == parm.loginname).FirstAsync();
                if (model == null)
                {
                    res.message = "账号错误";
                    return res;
                }
                if (!model.LoginPwd.Equals(parm.password))
                {
                    res.message = "密码错误~";
                    return res;
                }
                if (!model.Status)
                {
                    res.message = "登录账号被冻结，请联系管理员~";
                    return res;
                }
                adminModel.menu = GetMenuByAdmin(model.Guid);
                if (adminModel == null)
                {
                    res.message = "当前账号没有授权功能模块，无法登录~";
                    return res;
                }

                //修改登录时间
                model.LoginDate = DateTime.Now;
                model.UpLoginDate = model.LoginDate;
                model.LoginSum = model.LoginSum + 1;
                SysAdminDb.Update(model);

                var roleList = await Db.Queryable<SysRole>().Where(m => m.IsSystem).Select(m => m.Guid).ToListAsync();

                model.IsSystem = roleList.Intersect(model.RoleList.Select(p => p.guid)).Any();

                res.statusCode = (int)ApiEnum.Status;
                adminModel.admin = model;
                res.data = adminModel;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
        /// <summary>
        /// 根据登录账号，返回菜单信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        List<SysMenuDto> GetMenuByAdmin(string admin)
        {
            try
            {
                return Db.Queryable<SysMenu, SysPermissions, SysPermissions>((sm, sp, rolesp) => new JoinQueryInfos(JoinType.Left, sm.Guid == sp.MenuGuid, JoinType.Left, sp.RoleGuid == rolesp.RoleGuid))
                .Where((sm, sp, rolesp) => sp.Types == 1 && sm.Status && rolesp.Types == 2 && rolesp.AdminGuid == admin)
                .OrderBy((sm, sp) => sm.Sort)
                .Select((sm, sp) => new SysMenuDto()
                {
                    guid = sm.Guid,
                    parentGuid = sm.ParentGuid,
                    parentName = sm.ParentName,
                    name = sm.Name,
                    nameCode = sm.NameCode,
                    parentGuidList = sm.ParentGuidList,
                    layer = sm.Layer,
                    urls = sm.Urls,
                    icon = sm.Icon,
                    sort = sm.Sort,
                    btnJson = sp.BtnFunJson
                })
                .Mapper((it, cache) =>
                {
                    var codeList = cache.Get(list =>
                    {
                        return Db.Queryable<SysCode>().Where(m => m.ParentGuid == "a88fa4d3-3658-4449-8f4a-7f438964d716")
                            .Select(m => new SysCodeDto()
                            {
                                guid = m.Guid,
                                name = m.Name,
                                codeType = m.CodeType
                            })
                            .ToList();
                    });
                    if (!string.IsNullOrEmpty(it.btnJson))
                    {
                        it.btnFun = codeList.Where(m => it.btnJson.Contains(m.guid)).ToList();
                    }
                })
                .ToList()
                .GroupBy(p => p.guid)
                .Select(p =>
                {
                    var dto = p.First();
                    dto.btnFun = p.Where(f => f.btnFun != null).SelectMany(f => f.btnFun).CurDistinct(f => f.guid).ToList();
                    return dto;
                }).ToList();
            }
            catch// (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> AddAsync(SysAdmin parm)
        {
            var res = new ApiResult<string>
            {
                statusCode = (int)ApiEnum.ParameterError
            };

            try
            {
                //判断用吗是否存在
                var isExisteName = await Db.Queryable<SysAdmin>().AnyAsync(m => m.LoginName == parm.LoginName);
                if (isExisteName)
                {
                    res.message = $"用户名【{parm.LoginName}】已经存在";
                    return res;
                }

                parm.LoginPwd = DES3Encrypt.EncryptString(parm.LoginPwd);

                if (string.IsNullOrEmpty(parm.Guid))
                {
                    parm.Guid = Guid.NewGuid().ToString();
                }
                parm.AddDate = DateTime.Now;

                var succ = SysAdminDb.Insert(parm);

                //var rel = await Db.Queryable<CmsAdminMerchantRel>().Where(p => p.Admin_Guid == parm.CreateBy).FirstAsync();
                //if (rel != null)
                //{
                //    CmsAdminMerchantRelDb.Insert(new CmsAdminMerchantRel
                //    {
                //        Admin_Guid = parm.Guid,
                //        Out_Mch_Id = rel.Out_Mch_Id,
                //        out_sub_mch_id = rel.out_sub_mch_id
                //    });
                //}

                res.statusCode = (int)ApiEnum.Status;
                res.data = parm.Guid;

            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            return res;
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync(string parm)
        {
            var list = Utils.StrToListString(parm);
            var isok = await Db.Deleteable<SysAdmin>().Where(m => list.Contains(m.Guid)).ExecuteCommandAsync();
            //删除授权
            if (isok > 1)
            {
                await Db.Deleteable<SysPermissions>().Where(m => list.Contains(m.MenuGuid) && m.Types == 2).ExecuteCommandAsync();
            }
            var res = new ApiResult<string>
            {
                statusCode = isok > 0 ? 200 : 500,
                data = isok > 0 ? "1" : "0",
                message = isok > 0 ? "删除成功~" : "删除失败~"
            };
            return res;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Page<SysAdmin>>> GetPagesAsync(PageParm parm)
        {
            var res = new ApiResult<Page<SysAdmin>>();
            try
            {
                var adminGuidList = new List<string>();
                //判断是否根据角色查询用户信息
                if (!string.IsNullOrEmpty(parm.guid))
                {
                    adminGuidList = await Db.Queryable<SysPermissions>()
                        .Where(m => m.RoleGuid == parm.guid && m.Types == 2)
                        .Select(m => m.AdminGuid).ToListAsync();
                }
                //查询角色
                var roleList = await Db.Queryable<SysRole>().Where(m => m.IsSystem).Select(m => m.Guid).ToListAsync();
                res.data = await Db.Queryable<SysAdmin>()
                        .WhereIF(!string.IsNullOrEmpty(parm.CreateBy), p => p.CreateBy == parm.CreateBy)
                        .WhereIF(!string.IsNullOrEmpty(parm.key), m => m.DepartmentGuidList.Contains(parm.key))
                        .WhereIF(!string.IsNullOrEmpty(parm.guid), m => adminGuidList.Contains(m.Guid))
                        .OrderBy(m => m.AddDate).ToPageAsync(parm.page, parm.limit);
                foreach (var item in res.data.Items)
                {
                    foreach (var row in item.RoleList)
                    {
                        item.IsSystem |= roleList.Contains(row.guid);
                    }
                }
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
        /// 修改菜单
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> ModifyAsync(SysAdmin parm)
        {
            var res = new ApiResult<string>
            {
                statusCode = (int)ApiEnum.Error
            };
            try
            {
                //修改，判断用户是否和其它的重复
                var isExisteName = await Db.Queryable<SysAdmin>().AnyAsync(m => m.LoginName == parm.LoginName && m.Guid != parm.Guid);
                if (isExisteName)
                {
                    res.message = "用户名已存在，请更换~";
                    res.statusCode = (int)ApiEnum.ParameterError;
                    return await Task.Run(() => res);
                }

                parm.LoginPwd = DES3Encrypt.EncryptString(parm.LoginPwd);
                //if (!string.IsNullOrEmpty(parm.DepartmentGuid))
                //{
                //    // 说明有父级  根据父级，查询对应的模型
                //    var model = SysOrganizeDb.GetById(parm.DepartmentGuid);
                //    parm.DepartmentGuidList = model.ParentGuidList;
                //}
                //查询授权表，type=2 更新新的权限值
                //删除
                var authority = await Db.Deleteable<SysPermissions>().Where(m => m.AdminGuid == parm.Guid && m.Types == 2).ExecuteCommandAsync();
                //添加新的
                var authorityList = new List<SysPermissions>();
                foreach (var item in parm.RoleList)
                {
                    authorityList.Add(new SysPermissions()
                    {
                        RoleGuid = item.guid,
                        AdminGuid = parm.Guid,
                        Types = 2
                    });
                }
                await Db.Insertable(authorityList).ExecuteCommandAsync();

                var dbres = await Db.Updateable<SysAdmin>().SetColumns(m => new SysAdmin()
                {
                    LoginName = parm.LoginName,
                    LoginPwd = parm.LoginPwd,
                    RoleGuid = parm.RoleGuid,
                    //DepartmentName = parm.DepartmentName,
                    //DepartmentGuid = parm.DepartmentGuid,
                    //DepartmentGuidList = parm.DepartmentGuidList,
                    TrueName = parm.TrueName,
                    //Number = parm.Number,
                    Sex = parm.Sex,
                    Mobile = parm.Mobile,
                    //Email = parm.Email,
                    Status = parm.Status
                }).Where(m => m.Guid == parm.Guid).ExecuteCommandAsync();
                if (dbres > 0)
                {
                    res.statusCode = (int)ApiEnum.Status;
                    res.message = "更新成功！";
                }
                else
                {
                    res.message = "更新失败！";
                }

            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public async Task<ApiResult<string>> UpdatePwdAsync(UpdatePwdDto parm)
        {
            var res = new ApiResult<string>
            {
                statusCode = (int)ApiEnum.Error
            };
            try
            {
                if (string.IsNullOrEmpty(parm.userId))
                {
                    res.message = "当前登录用户已过期";
                    res.statusCode = (int)ApiEnum.LoginExpireError;
                    return await Task.Run(() => res);
                }
                if (parm.new_pwd != parm.con_pwd)
                {
                    res.message = "两次密码输入不一致";
                    res.statusCode = (int)ApiEnum.ParameterError;
                    return await Task.Run(() => res);
                }

                var model = await Db.Queryable<SysAdmin>().Where(t => t.Guid == parm.userId).FirstAsync();
                if (model.LoginPwd != DES3Encrypt.EncryptString(parm.old_pwd))
                {
                    res.message = "原密码错误";
                    res.statusCode = (int)ApiEnum.ParameterError;
                    return await Task.Run(() => res);
                }

                model.LoginPwd = DES3Encrypt.EncryptString(parm.new_pwd);
                var dbres = await Db.Updateable(model).UpdateColumns(t => new { t.LoginPwd }).ExecuteCommandAsync();

                if (dbres > 0)
                {
                    res.statusCode = (int)ApiEnum.Status;
                    res.message = "更新成功！";
                }
                else
                {
                    res.message = "更新失败！";
                }
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 获取用户门店权限
        /// </summary>
        public async Task<ApiResult<List<ShopInfo>>> GetShopsAsync(string admin_guid)
        {
            var res = new ApiResult<List<ShopInfo>>();
            try
            {
                res.data = await Db.Queryable<ShopInfo, AdminShopRel>((a, b) => new object[] {
                    JoinType.Inner,a.out_shop_id == b.out_shop_id })
                        .Where((a, b) => b.admin_guid == admin_guid)
                            .Select((a, b) => a).ToListAsync();
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
        /// 添加用户门店权限
        /// </summary>
        public async Task<ApiResult<string>> AddShopsAsync(AdminShopRel parm)
        {
            var res = new ApiResult<string> { statusCode = (int)ApiEnum.ParameterError };

            try
            {
                using var tran = new TransactionScope();

                await Db.Deleteable<AdminShopRel>().Where(t => t.admin_guid == parm.admin_guid).ExecuteCommandAsync();

                await Db.Insertable(parm.shopList.Select(t => new AdminShopRel
                {
                    admin_guid = parm.admin_guid,
                    out_shop_id = t.out_shop_id,
                    out_mch_id = t.out_mch_id,
                    out_sub_mch_id = t.out_sub_mch_id
                }).ToList()).ExecuteCommandAsync();

                tran.Complete();

                res.statusCode = (int)ApiEnum.Status;
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
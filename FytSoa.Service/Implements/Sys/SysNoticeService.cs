using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FytSoa.Service.Implements
{
    public class SysNoticeService : BaseService<SysNotice>, ISysNoticeService
    {
        readonly ICmsAgentService agent;
        readonly ICmsMerchantService merchant;
        public SysNoticeService(ICmsAgentService agent, ICmsMerchantService merchant, IConfiguration config) : base(config)
        {
            this.agent = agent;
            this.merchant = merchant;
        }

        public override async Task<ApiResult<Page<SysNotice>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<SysNotice>>();
            try
            {
                res.data = await Db.Queryable<SysNotice>()
                        .OrderBy(t => t.sort, SqlSugar.OrderByType.Desc)
                        .ToPageAsync(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        public override async Task<ApiResult<string>> AddAsync(SysNotice parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error, message = "发布公告信息失败" };

            try
            {
                using var tran = new TransactionScope();

                parm.create_time = DateTime.Now;
                parm.id = await Db.Insertable(parm).ExecuteReturnIdentityAsync();

                var noticeList = new List<SysNoticeChi>();

                #region 代理商用户

                if (parm.AgentList.Count > 0)
                {
                    var agent_ids = parm.AgentList.Select(t => t.id).ToArray();
                    var agentList = await Db.Queryable<CmsAgent>().In(t => t.Id, agent_ids).ToListAsync();

                    foreach (var item in agentList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.Admin_Guid,
                        });
                    }
                }

                #endregion

                #region 商户用户

                if (parm.MerchantList.Count > 0)
                {
                    var merchant_ids = parm.MerchantList.Select(t => t.id).ToArray();
                    var merchantList = await Db.Queryable<CmsMerchant>().In(t => t.id, merchant_ids).ToListAsync();

                    foreach (var item in merchantList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.admin_guid,
                        });
                    }

                    var admins_guids = merchantList.Select(t => t.admin_guid).ToArray();
                    var adminList = await Db.Queryable<SysAdmin>().In(t => t.CreateBy, admins_guids).ToListAsync();

                    foreach (var item in adminList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.Guid,
                        });
                    }
                }

                #endregion

                await Db.Insertable(noticeList).ExecuteCommandAsync();

                tran.Complete();

                res.statusCode = (int)ApiEnum.Status;
                res.message = "发布公告信息成功";
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            return res;
        }

        public override async Task<ApiResult<string>> UpdateAsync(SysNotice parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error, message = "修改公告信息失败" };

            try
            {
                using var tran = new TransactionScope();

                parm.update_time = DateTime.Now;
                await Db.Updateable(parm).ExecuteCommandAsync();

                var noticeList = new List<SysNoticeChi>();

                #region 代理商用户

                if (parm.AgentList.Count > 0)
                {
                    var agent_ids = parm.AgentList.Select(t => t.id).ToArray();
                    var agentList = await Db.Queryable<CmsAgent>().In(t => t.Id, agent_ids).ToListAsync();

                    foreach (var item in agentList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.Admin_Guid,
                        });
                    }
                }

                #endregion

                #region 商户用户

                if (parm.MerchantList.Count > 0)
                {
                    var merchant_ids = parm.MerchantList.Select(t => t.id).ToArray();
                    var merchantList = await Db.Queryable<CmsMerchant>().In(t => t.id, merchant_ids).ToListAsync();

                    foreach (var item in merchantList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.admin_guid,
                        });
                    }

                    var admins_guids = merchantList.Select(t => t.admin_guid).ToArray();
                    var adminList = await Db.Queryable<SysAdmin>().In(t => t.CreateBy, admins_guids).ToListAsync();

                    foreach (var item in adminList)
                    {
                        noticeList.Add(new SysNoticeChi
                        {
                            notice_id = parm.id,
                            admin_guid = item.Guid,
                        });
                    }
                }

                #endregion

                var chiList1 = Db.Queryable<SysNoticeChi>().Where(t => t.notice_id == parm.id).ToList();
                var chiList2 = new List<SysNoticeChi>(chiList1);

                chiList1.RemoveAll(t => noticeList.Select(t => t.admin_guid).ToArray().Contains(t.admin_guid));
                await Db.Deleteable(chiList1).ExecuteCommandAsync();

                noticeList.RemoveAll(t => chiList2.Select(t => t.admin_guid).ToArray().Contains(t.admin_guid));
                await Db.Insertable(noticeList).ExecuteCommandAsync();

                tran.Complete();

                res.statusCode = (int)ApiEnum.Status;
                res.message = "修改公告信息成功";
            }
            catch (Exception ex)
            {
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            return res;
        }

        public async Task<ApiResult<string>> GetUnreadQuantity(string admin_guid)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Status, data = "0" };

            try
            {
                var count = await Db.Queryable<SysNotice, SysNoticeChi>((a, b) => new object[] {
                        JoinType.Inner, a.id == b.notice_id }).
                            Where((a, b) => b.admin_guid == admin_guid && b.read_status == false && SqlFunc.Between(DateTime.Now, a.begin_time, a.end_time)).CountAsync();

                res.data = count.ToString();
            }
            catch (Exception ex)
            {
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            return res;
        }

        public async Task<ApiResult<List<SysNoticeChi>>> GetNoticeList(string admin_guid)
        {
            var res = new ApiResult<List<SysNoticeChi>>();

            try
            {
                res.data = await Db.Queryable<SysNotice, SysNoticeChi>((a, b) => new object[] {
                        JoinType.Inner, a.id == b.notice_id }).
                            Where((a, b) => b.admin_guid == admin_guid && SqlFunc.Between(DateTime.Now, a.begin_time, a.end_time)).Select((a, b) => new SysNoticeChi
                            {
                                title = a.title,
                                id = b.id,
                                read_status = b.read_status
                            }).ToListAsync();
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

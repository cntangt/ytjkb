using FytSoa.Common;
using FytSoa.Core.Model.Sys;
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
    public class SysNoticeService : BaseService<SysNotice>, ISysNoticeService
    {
        public SysNoticeService(IConfiguration config) : base(config)
        {
        }

        public override async Task<ApiResult<string>> AddAsync(SysNotice parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                using var tran = new TransactionScope();

                parm.create_time = DateTime.Now;
                var count = await Db.Insertable(parm).ExecuteCommandAsync();

                if (count <= 0)
                {
                    throw new Exception("发布公告信息失败");
                }

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

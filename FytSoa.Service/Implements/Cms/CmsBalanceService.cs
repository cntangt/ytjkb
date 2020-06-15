using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FytSoa.Service.Implements
{
    public class CmsBalanceService : BaseService<CmsBalance>, ICmsBalanceService
    {
        public CmsBalanceService(IConfiguration config) : base(config)
        {
        }

        public override async Task<ApiResult<Page<CmsBalance>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<CmsBalance>>();

            var start_date = parm.start_date.Value.ToString("yyyyMM");
            var end_date = parm.end_date.Value.ToString("yyyyMM");

            var query = Db.Queryable<CmsBalance>()
                        .WhereIF(!string.IsNullOrEmpty(parm.guid), a => a.agent_admin_guid == parm.guid && a.statu > 0)
                        .Where(t => SqlFunc.Between(t.BizDt, start_date, end_date))
                        .OrderBy(a => a.BizDt, OrderByType.Desc);

            res.data = await query.ToPageAsync(parm.page, parm.limit);

            return res;
        }

        public async Task<IEnumerable<CmsBalance_Chi>> GetTotalAmountAsync(string billId)
        {
            var data = await Db.Queryable<CmsBalance_Chi>().Where(t => t.BillID == billId)
                .GroupBy(t => t.sub_pay_platform)
                .Select(t => new CmsBalance_Chi
                {
                    sub_pay_platform = t.sub_pay_platform,
                    balance_amount = SqlFunc.AggregateSum(t.balance_amount),
                    rebate_amount = SqlFunc.AggregateSum(t.rebate_amount),
                    modify_amount = SqlFunc.AggregateSum(t.modify_amount),
                    rebate_amount_rel = SqlFunc.AggregateSum(t.rebate_amount_rel),
                }).ToListAsync();

            if (data.Count > 0)
            {
                data.ForEach(p =>
                {
                    p.Settle_Name = Enum.GetName(typeof(SubPayPlatform), p.sub_pay_platform);
                });
            }

            return data;
        }

        public async Task<ApiResult<Page<CmsBalance_Chi>>> GetDetailPageAsync(PageParm parm)
        {
            var res = new ApiResult<Page<CmsBalance_Chi>>();

            var data = await Db.Queryable<CmsBalance_Chi>().Where(t => t.BillID == parm.key).ToPageAsync(parm.page, parm.limit);

            if (data.Items.Count > 0)
            {
                data.Items.ForEach(p =>
                {
                    p.Settle_Name = Enum.GetName(typeof(SubPayPlatform), p.sub_pay_platform);
                });
            }

            res.data = data;

            return res;
        }

        public async Task<ApiResult<string>> ModifyAmountAsync(CmsBalance_Chi parm)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                using var tran = new TransactionScope();

                var chi = await Db.Queryable<CmsBalance_Chi>().Where(t => t.BillID == parm.BillID &&
                                                                            t.out_sub_mch_id == parm.out_sub_mch_id &&
                                                                            t.sub_pay_platform == parm.sub_pay_platform).SingleAsync();

                var fath = await Db.Queryable<CmsBalance>().Where(t => t.BillID == parm.BillID).SingleAsync();

                parm.rebate_amount_rel = chi.rebate_amount_rel - chi.modify_amount + parm.modify_amount;
                await Db.Updateable(parm).UpdateColumns(p => new { p.modify_amount, p.rebate_amount_rel }).ExecuteCommandAsync();

                await Db.Updateable(new CmsBalance
                {
                    BillID = fath.BillID,
                    modify_amount = fath.modify_amount - chi.modify_amount + parm.modify_amount,
                    rebate_amount_rel = fath.rebate_amount_rel - chi.modify_amount + parm.modify_amount,
                    moditime = DateTime.Now
                }).UpdateColumns(t => new { t.modify_amount, t.rebate_amount_rel, t.moditime }).ExecuteCommandAsync();

                tran.Complete();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            res.statusCode = (int)ApiEnum.Status;
            res.message = "调整金额成功";

            return res;
        }

        public async Task<ApiResult<string>> ConfirmBillAsync(CmsBalance parm)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Error };

            try
            {
                var model = await Db.Queryable<CmsBalance>().Where(t => t.BillID == parm.BillID).SingleAsync();

                model.statu += 1;
                model.moditime = DateTime.Now;

                await Db.Updateable(model).UpdateColumns(t => new { t.statu, t.moditime }).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }

            res.statusCode = (int)ApiEnum.Status;
            res.message = "操作成功";

            return res;
        }
    }
}
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
    }
}
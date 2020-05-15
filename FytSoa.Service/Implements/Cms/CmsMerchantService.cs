using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces.Cms;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsMerchantService : BaseService<CmsMerchant>, ICmsMerchantService
    {
        public CmsMerchantService(IConfiguration config) : base(config)
        {
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
    }
}
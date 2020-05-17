using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsShopService : BaseService<ShopInfo>, ICmsShopService
    {
        public CmsShopService(IConfiguration config) : base(config)
        {
        }

        public override async Task<ApiResult<Page<ShopInfo>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<ShopInfo>>();

            var query = Db.Queryable<ShopInfo>();

            if (!string.IsNullOrEmpty(parm.CreateBy) || !string.IsNullOrEmpty(parm.key))
            {
                var mchs = Db.Queryable<CmsMerchant>()
                    .WhereIF(!string.IsNullOrEmpty(parm.key), p => p.sub_out_mch_id == parm.key)
                    .WhereIF(!string.IsNullOrEmpty(parm.CreateBy), p => p.agent_admin_guid == parm.CreateBy)
                    .Select(p => p.sub_out_mch_id)
                    .ToList();

                if (mchs.Count == 0)
                {
                    mchs.Add("-");
                }

                query.In(p => p.sub_out_mch_id, mchs);
            }

            var data = await query.ToPageAsync(parm.page, parm.limit);

            if (data.Items.Count > 0)
            {
                var ids = data.Items.Select(p => p.sub_out_mch_id).ToArray();

                var mchs = await Db.Queryable<CmsMerchant>().In(p => p.sub_out_mch_id, ids).ToListAsync();
                if (mchs.Count > 0)
                {
                    data.Items.ForEach(shop =>
                    {
                        var mch = mchs.FirstOrDefault(m => shop.sub_out_mch_id == m.sub_out_mch_id);
                        if (mch != null)
                        {
                            shop.out_mch_name = mch.name;
                        }
                    });
                }
            }

            res.data = data;
            res.statusCode = (int)ApiEnum.Status;

            return res;
        }
    }
}
using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsShopService : BaseService<ShopInfo>, ICmsShopService
    {
        readonly ISysRoleService roleService;

        public CmsShopService(ISysRoleService roleService, IConfiguration config) : base(config)
        {
            this.roleService = roleService;
        }

        public async Task<IEnumerable<ShopInfo>> GetByAdminGuidAsync(string admin_guid, string out_sub_mch_id, string key, int limit)
        {
            var role_info = await roleService.GetRoleByAdminGuid(admin_guid);

            var query = Db.Queryable<ShopInfo, CmsMerchant, AdminShopRel>((shop, mch, rel) =>
                new JoinQueryInfos(JoinType.Left, shop.out_sub_mch_id == mch.out_sub_mch_id, JoinType.Left, shop.out_shop_id == rel.out_shop_id));

            if (role_info.isSystem)
            {
                //管理员不需要过滤
            }
            else if (role_info.isAgent)
            {
                query.Where((shop, mch, rel) => mch.agent_admin_guid == admin_guid);
            }
            else if (role_info.isSubAdmin)
            {
                query.Where((shop, mch, rel) => mch.admin_guid == admin_guid);
            }
            else // 普通员工
            {
                query.Where((shop, mch, rel) => rel.admin_guid == admin_guid);
            }

            query
                .WhereIF(!string.IsNullOrEmpty(key), (shop, mch, rel) => shop.shop_name.Contains(key))
                .WhereIF(!string.IsNullOrEmpty(out_sub_mch_id), (shop, mch, rel) => shop.out_sub_mch_id == out_sub_mch_id);

            return await query.ToPageListAsync(1, limit);
        }

        public override async Task<ApiResult<Page<ShopInfo>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var role_info = await roleService.GetRoleByAdminGuid(parm.CreateBy);

            var res = new ApiResult<Page<ShopInfo>>();

            var query = Db.Queryable<ShopInfo, CmsMerchant, AdminShopRel>((shop, mch, rel) =>
                new JoinQueryInfos(JoinType.Left, shop.out_sub_mch_id == mch.out_sub_mch_id, JoinType.Left, shop.out_shop_id == rel.out_shop_id));
            if (role_info.isSystem)
            {
                //管理员不需要过滤
            }
            else if (role_info.isAgent)
            {
                query.Where((shop, mch, rel) => mch.agent_admin_guid == parm.CreateBy);
            }
            else if (role_info.isSubAdmin)
            {
                query.Where((shop, mch, rel) => mch.admin_guid == parm.CreateBy);
            }
            else // 普通员工
            {
                query.Where((shop, mch, rel) => rel.admin_guid == parm.CreateBy);
            }

            query.WhereIF(!string.IsNullOrEmpty(parm.key), (shop, mch, rel) => shop.out_sub_mch_id == parm.key);

            var data = await query.ToPageAsync(parm.page, parm.limit);

            if (data.Items.Count > 0)
            {
                var ids = data.Items.Select(p => p.out_sub_mch_id).ToArray();

                var mchs = await Db.Queryable<CmsMerchant>().In(p => p.out_sub_mch_id, ids).ToListAsync();
                if (mchs.Count > 0)
                {
                    data.Items.ForEach(shop =>
                    {
                        var mch = mchs.FirstOrDefault(m => shop.out_sub_mch_id == m.out_sub_mch_id);
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

        public override async Task<ApiResult<ShopInfo>> GetModelAsync(Expression<Func<ShopInfo, bool>> where, bool Async = true)
        {
            var shop = await base.GetModelAsync(where, Async);

            if (shop.data != null && shop.data.id > 0)
            {
                var mch = await Db.Queryable<CmsMerchant>().FirstAsync(p => p.out_sub_mch_id == shop.data.out_sub_mch_id);

                if (mch != null && mch.id > 0)
                {
                    shop.data.out_mch_name = mch.name;
                }
            }

            return shop;
        }
    }
}
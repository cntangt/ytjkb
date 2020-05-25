using FytSoa.Common;
using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Sys;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsStaffService : BaseService<StaffInfo>, ICmsStaffService
    {
        readonly ISysRoleService roleService;
        public CmsStaffService(ISysRoleService roleService, IConfiguration config) : base(config)
        {
            this.roleService = roleService;
        }

        public override async Task<ApiResult<Page<StaffInfo>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var data = await Db.Queryable<StaffInfo>().Where(p => p.out_shop_id == parm.key).ToPageAsync(parm.page, parm.limit);

            return new ApiResult<Page<StaffInfo>>
            {
                data = data,
                statusCode = (int)ApiEnum.Status
            };
        }

        public async Task<IEnumerable<StaffInfo>> GetByShop(string admin_guid, string key, string out_sub_mch_id, string out_shop_id, int limit)
        {
            var role_info = await roleService.GetRoleByAdminGuid(admin_guid);

            var query = Db.Queryable<StaffInfo, CmsMerchant, AdminShopRel>((staff, mch, rel) =>
                new JoinQueryInfos(JoinType.Left, staff.out_sub_mch_id == mch.out_sub_mch_id, JoinType.Left, staff.out_shop_id == rel.out_shop_id));

            if (role_info.isSystem)
            {
                //管理员不需要过滤
            }
            else if (role_info.isAgent)
            {
                query.Where((staff, mch, rel) => mch.agent_admin_guid == admin_guid);
            }
            else if (role_info.isSubAdmin)
            {
                query.Where((staff, mch, rel) => mch.admin_guid == admin_guid);
            }
            else // 普通员工
            {
                query.Where((staff, mch, rel) => rel.admin_guid == admin_guid);
            }

            query
                .WhereIF(!string.IsNullOrEmpty(key), (staff, mch, rel) => staff.staff_name.Contains(key))
                .WhereIF(!string.IsNullOrEmpty(out_sub_mch_id), (staff, mch, rel) => staff.out_sub_mch_id == out_sub_mch_id)
                .WhereIF(!string.IsNullOrEmpty(out_shop_id), (staff, mch, rel) => staff.out_shop_id == out_shop_id);

            return await query.ToPageListAsync(1, limit);
        }
    }
}
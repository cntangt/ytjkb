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
        public CmsStaffService(IConfiguration config) : base(config)
        {
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
            var roles = await Db.Queryable<SysAdmin, SysPermissions, SysRole>((admin, perm, role) => new JoinQueryInfos(
                JoinType.Left, admin.Guid == perm.AdminGuid,
                JoinType.Left, perm.RoleGuid == role.Guid))
             .Where((admin, perm, role) => admin.Guid == admin_guid)
             .Select((admin, perm, role) => role)
             .ToListAsync();

            var isSystem = roles.Any(p => p.IsSystem);
            var isAgent = roles.Any(p => p.Guid == "72171cf0-934d-4934-8e27-ee4f47e9985e");
            var is_sub_admin = roles.Any(p => p.Guid == "8dc9b479-216d-415a-9fba-85caedd6c4df");

            var query = Db.Queryable<StaffInfo, CmsMerchant, AdminShopRel>((staff, mch, rel) =>
                new JoinQueryInfos(JoinType.Left, staff.out_sub_mch_id == mch.out_sub_mch_id, JoinType.Left, staff.out_shop_id == rel.out_shop_id));

            if (isSystem)
            {
                //管理员不需要过滤
            }
            else if (isAgent)
            {
                query.Where((staff, mch, rel) => mch.agent_admin_guid == admin_guid);
            }
            else if (is_sub_admin)
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
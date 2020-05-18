using FytSoa.Common;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
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
    }
}
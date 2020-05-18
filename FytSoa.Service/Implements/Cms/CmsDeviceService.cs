using FytSoa.Common;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class CmsDeviceService : BaseService<DeviceInfo>, ICmsDeviceService
    {
        public CmsDeviceService(IConfiguration config) : base(config)
        {
        }

        public override async Task<ApiResult<Page<DeviceInfo>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var data = await Db.Queryable<DeviceInfo>().Where(p => p.out_shop_id == parm.key).ToPageAsync(parm.page, parm.limit);

            return new ApiResult<Page<DeviceInfo>>
            {
                data = data,
                statusCode = (int)ApiEnum.Status
            };
        }
    }
}
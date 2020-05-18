using FytSoa.Core.Model.Wx;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    public class CmsStaffService : BaseService<StaffInfo>, ICmsStaffService
    {
        public CmsStaffService(IConfiguration config) : base(config)
        {
        }
    }
}
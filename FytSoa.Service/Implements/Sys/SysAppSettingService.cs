using FytSoa.Core.Model.Sys;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    public class SysAppSettingService : BaseService<SysAppSetting>, ISysAppSettingService
    {
        public SysAppSettingService(IConfiguration config) : base(config)
        {
        }
    }
}

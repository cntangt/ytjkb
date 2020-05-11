using FytSoa.Core.Model.Wx;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：Wx_setting服务接口实现
    */
    public class WxSettingService : BaseService<WxSetting>, IWxSettingService
    {
        public WxSettingService(IConfiguration config) : base(config)
        {
        }
    }
}
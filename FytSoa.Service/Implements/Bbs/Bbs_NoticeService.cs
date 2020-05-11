using FytSoa.Core.Model.Bbs;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：Bbs_notice服务接口实现
    */
    public class Bbs_NoticeService : BaseService<Bbs_Notice>, IBbs_NoticeService
    {
        public Bbs_NoticeService(IConfiguration config) : base(config)
        {
        }
    }
}
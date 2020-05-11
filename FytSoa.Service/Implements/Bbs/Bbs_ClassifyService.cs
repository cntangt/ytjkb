using FytSoa.Core.Model.Bbs;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：Bbs_classify服务接口实现
    */
    public class Bbs_ClassifyService : BaseService<Bbs_Classify>, IBbs_ClassifyService
    {
        public Bbs_ClassifyService(IConfiguration config) : base(config)
        {
        }
    }
}
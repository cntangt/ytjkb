﻿using FytSoa.Core.Model.Cms;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：CmsVote服务接口实现
    * 版权所有：北京飞易腾科技有限公司
    * 企业官网：http://www.feiyit.com
    */
    public class CmsVoteService : BaseService<CmsVote>, ICmsVoteService
    {
        public CmsVoteService(IConfiguration config) : base(config)
        {
        }
    }
}
﻿using FytSoa.Core.Model.Bbs;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：Bbs_comment服务接口实现
    */
    public class Bbs_CommentService : BaseService<Bbs_Comment>, IBbs_CommentService
    {
        public Bbs_CommentService(IConfiguration config) : base(config)
        {
        }
    }
}
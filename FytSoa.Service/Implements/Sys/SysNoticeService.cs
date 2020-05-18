using FytSoa.Common;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.DtoModel.Sys;
using FytSoa.Service.Extensions;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class SysNoticeService : BaseService<SysNotice>, ISysNoticeService
    {
        public SysNoticeService(IConfiguration config) : base(config)
        {
        }
    }
}

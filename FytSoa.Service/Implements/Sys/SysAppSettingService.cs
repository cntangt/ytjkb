using FytSoa.Common;
using FytSoa.Core;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.DtoModel;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements
{
    public class SysAppSettingService : BaseService<SysAppSetting>, ISysAppSettingService
    {
        public SysAppSettingService(IConfiguration config) : base(config)
        {
        }
    }
}

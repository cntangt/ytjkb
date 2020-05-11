using System;
using FytSoa.Core.Model.Form;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    public class FormTableService : BaseService<FormTable>, IFormTableService
    {
        public FormTableService(IConfiguration config) : base(config)
        {
        }
    }
}

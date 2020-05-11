using FytSoa.Core.Model.Member;
using FytSoa.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FytSoa.Service.Implements
{
    /*!
    * 文件名称：Member_group服务接口实现
    */
    public class Member_GroupService : BaseService<Member_Group>, IMember_GroupService
    {
        public Member_GroupService(IConfiguration config) : base(config)
        {
        }
    }
}
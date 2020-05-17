
namespace FytSoa.Service.DtoModel.Sys
{
    public class UpdatePwdDto
    {
        public string userId { get; set; }
        public string old_pwd { get; set; }
        public string new_pwd { get; set; }
        public string con_pwd { get; set; }
    }
}

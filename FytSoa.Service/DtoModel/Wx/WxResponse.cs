namespace FytSoa.Service.DtoModel.Wx
{
    public class WxResponse<T>
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public T Data { get; set; }
    }
}

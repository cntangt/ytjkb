namespace FytSoa.Service.DtoModel
{
	public class PageResult<T>
	{
		public int Code { get; set; }
		public string Msg { get; set; }
		public T Data { get; set; }
		public long Count { get; set; }
	}
}

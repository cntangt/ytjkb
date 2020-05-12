using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements.Wx
{
    public class WxCloudService : IWxCloudService
    {
        const string domain = "https://pay.qcloud.com/cpay/";

        readonly IConfiguration config;
        readonly IHttpClientFactory factory;

        public WxCloudService(IConfiguration config, IHttpClientFactory factory)
        {
            this.config = config;
            this.factory = factory;
        }

        public async Task<WxResponse<T>> QueryAsync<T>(WxRequest<T> req)
        {
            var result = new WxResponse<T>();

            var req_content = JsonConvert.SerializeObject(req);

            var req_data = new
            {
                authen_info = new
                {
                    a = new
                    {
                        authen_type = 1,
                        authen_code = SHA256(req_content)
                    }
                },
                request_content = req_content
            };

            var client = factory.CreateClient();
            var data = new StringContent(JsonConvert.SerializeObject(req_data), Encoding.UTF8, "application/json");
            var msg = await client.PostAsync($"{domain}{req.ApiName}", data);
            var json = await msg.Content.ReadAsStringAsync();
            var res = JObject.Parse(json);

            var res_content = res["response_content"];

            if (res_content == null)
            {
                result.Status = -1;
                result.Description = "请求结果为空";
                return result;
            }
            if ((int)res_content["status"] == 0)
            {
                result.Data = res_content[req.ApiName].ToObject<T>();
            }

            result.Status = (int)res_content["status"];
            result.Description = res_content["description"].ToString();

            return result;
        }

        private string SHA256(string content)
        {
            var key = Encoding.UTF8.GetBytes(config["AUTHEN_KEY"]);
            var payload = Encoding.UTF8.GetBytes(content);
            var hmacsha256 = new HMACSHA256(key);
            var bs = hmacsha256.ComputeHash(payload, 0, payload.Length);
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

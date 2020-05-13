using Newtonsoft.Json;
using System;

namespace FytSoa.Service.DtoModel.Wx
{
    public abstract class WxRequest<T>
    {
        [JsonIgnore]
        public abstract string ApiName { get; }

        [JsonIgnore]
        public abstract string DataName { get; }

        [JsonIgnore]
        public byte[] Cert { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string AuthenKey { get; set; }

        public string nonce_str => Guid.NewGuid().ToString("n");

    }
}

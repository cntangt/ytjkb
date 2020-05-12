using System;
using System.Text.Json.Serialization;

namespace FytSoa.Service.DtoModel.Wx
{
    public abstract class WxRequest<T>
    {
        [JsonIgnore]
        public abstract string ApiName { get; }

        public string nonce_str => Guid.NewGuid().ToString("n");
    }
}

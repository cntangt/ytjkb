using FytSoa.Core.Model.Cms;
using FytSoa.Core.Model.Wx;
using FytSoa.Service.DtoModel.Wx;
using FytSoa.Service.Interfaces.Wx;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements.Wx
{
    public class WxCloudService : BaseService<object>, IWxCloudService
    {
        const string domain = "https://pay.qcloud.com/cpay/";

        readonly IConfiguration config;
        readonly IHttpClientFactory factory;

        public WxCloudService(IConfiguration config, IHttpClientFactory factory) : base(config)
        {
            this.config = config;
            this.factory = factory;
        }

        public async Task<WxResponse<T>> QueryAsync<T>(WxRequest<T> req)
        {
            var result = new WxResponse<T>();

            var req_content = JsonConvert.SerializeObject(req, jss);

            var req_data = new
            {
                authen_info = new
                {
                    a = new
                    {
                        authen_type = 1,
                        authen_code = SHA256(req_content, req.AuthenKey)
                    }
                },
                request_content = req_content
            };

            var client = factory.CreateClient();
            var data = new StringContent(JsonConvert.SerializeObject(req_data, jss), Encoding.UTF8, "application/json");
            var msg = await client.PostAsync($"{domain}{req.ApiName}", data);
            var json = await msg.Content.ReadAsStringAsync();
            var res = JObject.Parse(json);

            if (res["response_content"] == null)
            {
                result.Status = -1;
                result.Description = "请求结果为空";
                return result;
            }

            var res_content = JObject.Parse(res["response_content"].ToString());

            if ((int)res_content["status"] == 0)
            {
                result.Data = JsonConvert.DeserializeObject<T>(res_content[req.DataName].ToString(), jss);
            }

            result.Status = (int)res_content["status"];
            result.Description = res_content["description"].ToString();

            return result;
        }

        private string SHA256(string content, string authenKey)
        {
            var key = Encoding.UTF8.GetBytes(authenKey);
            var payload = Encoding.UTF8.GetBytes(content);
            var hmacsha256 = new HMACSHA256(key);
            var bs = hmacsha256.ComputeHash(payload, 0, payload.Length);
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        private string out_mch_id => config["out_mch_id"];

        private JsonSerializerSettings jss
        {
            get
            {
                var setting = new JsonSerializerSettings();

                setting.NullValueHandling = NullValueHandling.Ignore;

                setting.Converters.Add(new UnixDateTimeConverter());

                setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;

                setting.ContractResolver = new EmptyValueContractResolver();

                return setting;
            }
        }

        #region 详细同步业务


        public async Task<string> SyncShopInfo(int id)
        {
            var mch = await Db.Queryable<CmsMerchant>().FirstAsync(p => p.id == id);

            try
            {
                await Db.Deleteable<ShopInfo>(p => p.out_sub_mch_id == mch.out_sub_mch_id).ExecuteCommandAsync();
                await Db.Deleteable<DeviceInfo>(p => p.out_sub_mch_id == mch.out_sub_mch_id).ExecuteCommandAsync();
                await Db.Deleteable<StaffInfo>(p => p.out_sub_mch_id == mch.out_sub_mch_id).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            int countShop = 0, countDevice = 0, countStaff = 0;

            for (var page = 1; ; page++)
            {
                var req = new QueryShopInfoRequest
                {
                    page_num = page,
                    page_size = 10,
                    out_mch_id = out_mch_id,
                    out_sub_mch_id = mch.out_sub_mch_id,
                    AuthenKey = mch.authen_key
                };

                var res = await QueryAsync(req);

                if (res.Status != 0)
                {
                    return res.Description;
                }

                if (res.Status == 0 && res.Data != null && res.Data.shop_infos != null && res.Data.shop_infos.Length > 0)
                {
                    try
                    {
                        var shops = res.Data.shop_infos.Select(shop =>
                        {
                            shop.out_mch_id = out_mch_id;
                            shop.out_sub_mch_id = mch.out_sub_mch_id;
                            shop.erp_org = shop.shop_name.GetShopErpOrg();

                            return shop;
                        }).ToArray();
                        countShop += await Db.Insertable(shops).ExecuteCommandAsync();

                        var devices = shops.Where(p => p.device_infos != null).SelectMany(shop =>
                        {
                            return shop.device_infos.Select(device =>
                            {
                                device.out_shop_id = shop.out_shop_id;
                                device.out_mch_id = shop.out_shop_id;
                                device.out_sub_mch_id = shop.out_sub_mch_id;

                                return device;
                            });
                        }).ToArray();
                        countDevice += await Db.Insertable(devices).ExecuteCommandAsync();

                        var staffs = shops.Where(p => p.staff_infos != null).SelectMany(shop =>
                        {
                            return shop.staff_infos.Select(staff =>
                            {
                                staff.out_shop_id = shop.out_shop_id;
                                staff.out_mch_id = shop.out_mch_id;
                                staff.out_sub_mch_id = shop.out_sub_mch_id;

                                return staff;
                            });
                        }).ToArray();
                        countStaff += await Db.Insertable(staffs).ExecuteCommandAsync();
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }
                else
                {
                    break;
                }
            }

            return $"同步门店信息完成，门店：{countShop}，设备：{countDevice}，店员：{countStaff}";
        }

        #endregion
    }

    public class EmptyValueContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jp = base.CreateProperty(member, memberSerialization);

            if (memberSerialization == MemberSerialization.OptOut && jp.PropertyType == typeof(string))
            {
                jp.ShouldSerialize = instance =>
                {
                    var val = instance.GetType().GetProperty(member.Name).GetValue(instance);

                    return val != null && !string.IsNullOrEmpty(val.ToString());
                };
            }

            return jp;
        }
    }
}

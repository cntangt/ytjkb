using FytSoa.Extensions;
using FytSoa.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace FytSoa.Web
{
    public class Startup
    {
        private readonly IConfiguration config;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            config = configuration;

            BaseConfigModel.SetBaseConfig(config, env.ContentRootPath, env.WebRootPath);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //自定注册
            AddAssembly(services, "FytSoa.Service");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => o.LoginPath = new PathString("/fytadmin/login"))
                .AddJwtBearer(JwtAuthorizeAttribute.JwtAuthenticationScheme, o =>
                {
                    var jwtConfig = new JwtAuthConfigModel();
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidateLifetime = true,//是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                        ClockSkew = TimeSpan.FromSeconds(30),//注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                        ValidAudience = jwtConfig.Audience,//Audience
                        ValidIssuer = jwtConfig.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtAuth:SecurityKey"]))//拿到SecurityKey
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // 如果过期，则把<是否过期>添加到，返回头信息中
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("App", policy => policy.RequireRole("App").Build());
                    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                    options.AddPolicy("AdminOrApp", policy => policy.RequireRole("Admin,App").Build());
                })
                .AddDistributedRedisCache(p => p.Configuration = config["Cache:Configuration"])
                .AddSingleton(HtmlEncoder.Create(UnicodeRanges.All))
                .AddSingleton(GetScheduler())
                .AddResponseCompression()
                .AddHttpClient()
                .AddCors(c =>
                {
                    c.AddPolicy("Any", policy =>
                    {
                        policy.SetIsOriginAllowed(p => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
                })
                .AddMvc(p =>
                {
                    p.EnableEndpointRouting = false;
                })
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
#else
            app.UseExceptionHandler("/Error");
#endif
            app.UseStatusCodePagesWithReExecute("/Error");

            // 解决Ubuntu Nginx 代理不能获取IP问题
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //将日志记录到数据库
            LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            LogManager.Configuration.Variables["connectionString"] = config["DBConnection:MySqlConnectionString"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //避免日志中的中文输出乱码

            app.UseAuthentication(); // 认证
            app.UseResponseCompression(); //性能压缩
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors("Any");
            app.UseMvc();
        }

        public void AddAssembly(IServiceCollection services, string assemblyName)
        {
            if (!string.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    if (interfaceType.Length == 1)
                    {
                        services.AddTransient(interfaceType[0], item);
                    }
                    if (interfaceType.Length > 1)
                    {
                        services.AddTransient(interfaceType[1], item);
                    }
                }
            }
        }

        private SchedulerCenter GetScheduler()
        {
            string dbProviderName = config.GetSection("Quartz")["dbProviderName"];
            string connectionString = config.GetSection("Quartz")["connectionString"];
            string driverDelegateType;
            switch (dbProviderName)
            {
                case "SQLite-Microsoft":
                case "SQLite":
                    driverDelegateType = typeof(SQLiteDelegate).AssemblyQualifiedName; break;
                case "MySql":
                    driverDelegateType = typeof(MySQLDelegate).AssemblyQualifiedName; break;
                case "OracleODPManaged":
                    driverDelegateType = typeof(OracleDelegate).AssemblyQualifiedName; break;
                case "SQLServer":
                case "SQLServerMOT":
                    driverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName; break;
                case "Npgsql":
                    driverDelegateType = typeof(PostgreSQLDelegate).AssemblyQualifiedName; break;
                case "Firebird":
                    driverDelegateType = typeof(FirebirdDelegate).AssemblyQualifiedName; break;
                default:
                    throw new System.Exception("dbProviderName unreasonable");
            }
            SchedulerCenter schedulerCenter = SchedulerCenter.Instance;
            schedulerCenter.Setting(new DbProvider(dbProviderName, connectionString), driverDelegateType);
            return schedulerCenter;
        }
    }
}

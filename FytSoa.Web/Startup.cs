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
            //�Զ�ע��
            AddAssembly(services, "FytSoa.Service");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => o.LoginPath = new PathString("/fytadmin/login"))
                .AddJwtBearer(JwtAuthorizeAttribute.JwtAuthenticationScheme, o =>
                {
                    var jwtConfig = new JwtAuthConfigModel();
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤Issuer
                        ValidateAudience = true,//�Ƿ���֤Audience
                        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                        ValidateLifetime = true,//�Ƿ���֤��ʱ  ������exp��nbfʱ��Ч ͬʱ����ClockSkew 
                        ClockSkew = TimeSpan.FromSeconds(30),//ע�����ǻ������ʱ�䣬�ܵ���Чʱ��������ʱ�����jwt�Ĺ���ʱ�䣬��������ã�Ĭ����5����
                        ValidAudience = jwtConfig.Audience,//Audience
                        ValidIssuer = jwtConfig.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtAuth:SecurityKey"]))//�õ�SecurityKey
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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

            // ���Ubuntu Nginx �����ܻ�ȡIP����
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //����־��¼�����ݿ�
            LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            LogManager.Configuration.Variables["connectionString"] = config["DBConnection:MySqlConnectionString"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //������־�е������������

            app.UseAuthentication(); // ��֤
            app.UseResponseCompression(); //����ѹ��
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

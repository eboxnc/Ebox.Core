
using Autofac;
using Autofac.Extras.DynamicProxy;
using Ebox.Core.Common.Helpers;
using Ebox.Core.Data;
using Ebox.Core.Extensions.ServiceExtensions;
using Ebox.Core.Identity;
using Ebox.Core.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Ebox.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));
            services.AddControllers();
            #region swagger配置
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ebox.Core",
                    Version = "v1",
                    Description = "框架说明文档",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                    {
                        Name = "Ebox.Core",
                        Email = "Ebox.Core@xxx.com",
                        Url = "https://www.baidu.com"
                    }
                });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Ebox.Core.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);

                var xmlModelPath = Path.Combine(basePath, "Ebox.Core.Model.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlModelPath);

                //foreach (var file in Directory.GetFiles(basePath, "*.xml"))
                //{
                //    c.IncludeXmlComments(file);
                //}

                ///在一个Controller中使用相同(类似)参数的方法
                c.CustomSchemaIds(type => type.ToString());
                c.SchemaGeneratorOptions.DiscriminatorNameSelector = (t) => t.Name;
                //c.DocInclusionPredicate((docName, apiDescription) =>
                //{
                //    return string.Equals(docName, apiDescription.GroupName, StringComparison.OrdinalIgnoreCase);
                //});

                #region identity4
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    In = ParameterLocation.Header,
                //    Name = "Bearer",
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Password = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri(Configuration.GetValue<string>("appSettings:Authority") + "/connect/authorize"),
                //            TokenUrl = new Uri(Configuration.GetValue<string>("appSettings:Authority") + "/connect/token"),
                //            Scopes = new Dictionary<string, string> { { "Ebox_scope", "Ebox API Scope" } },
                //        }
                //    }
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        }, new List<string>() }
                //});
                #endregion
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.OperationFilter<AddResponseHeadersFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization", //请求头中token名称
                    Description = "Bearer+空格"
                });


            });

            #endregion

            services.AddAuthorization();

            #region identity4
            //services.AddIdentityServer().AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddInMemoryApiResources(Config.GetApis())
            //    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            //.AddProfileService<UserProfileService>();
            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = Configuration.GetValue<string>("appSettings:Authority");
            //        options.RequireHttpsMetadata = false;
            //        options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(0);//滑动时间
            //        options.TokenValidationParameters.ValidateAudience = false;
            //        options.TokenValidationParameters.LifetimeValidator = (notBefore, expires, token, para) => expires != null ? expires > DateTime.UtcNow : true;
            //    });
            #endregion

            services.AddSingleton<JwtHelper>();
            services.AddSqlsugarSetup(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                var audienceConfig = Configuration.GetSection("JwtConfig");
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,//验证密钥
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"])),

                    ValidateIssuer = true,//验证发行人
                    ValidIssuer = audienceConfig["Issuer"],

                    ValidateAudience = true, //验证订阅人
                    ValidAudience = audienceConfig["Audience"],

                    RequireExpirationTime = true,//验证过期时间
                    ValidateLifetime = true, //验证生命周期

                    ClockSkew = TimeSpan.Zero //缓冲过期时间
                };

                x.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException)) //
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        },
                    OnMessageReceived = context =>
                    {
                        var bearerToken = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(bearerToken))
                        {
                            var helper = context.HttpContext.RequestServices.GetService<JwtHelper>();
                            context.Token = helper.GetNoBearerToken(bearerToken);
                            //if (helper.IsInvalidToken(context.Token))
                            //{                             
                            //    var result = JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
                            //    context.Response.ContentType = "application/json";
                            //    //验证失败返回401
                            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            //    context.Response.WriteAsync(result);

                            //    return Task.FromResult(0);
                            //}
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        //终止默认的返回结果(必须有)
                        context.HandleResponse();
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
                        context.Response.ContentType = "application/json";
                        //验证失败返回401
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.WriteAsync(result);
                        return Task.FromResult(0);
                    },
                    OnTokenValidated = context =>
                    {
                        var token = ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)context.SecurityToken).RawData;
                        var helper = context.HttpContext.RequestServices.GetService<JwtHelper>();
                        context.Response.Headers.Add("n-token", helper.RenewToken(token));
                        if (helper.IsInvalidToken(token))
                        {
                            var result = JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
                            context.Response.ContentType = "application/json";
                            //验证失败返回401
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.WriteAsync(result);

                            return Task.FromResult(0);
                        }
                        return Task.CompletedTask;
                    }
                };
            });


            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("appSettings:Authority"));
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ebox.Core v1");
                    c.OAuthClientId(Config.ClientId);
                    c.OAuthClientSecret(Config.ClientSecret);
                });
            }

            app.UseRouting();
            //    app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region 自动注入服务
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var assemblysServices = Assembly.Load("Ebox.Core.Interface");
            var assemblysServices1 = Assembly.Load("Ebox.Core.Data");
            builder.RegisterAssemblyTypes(assemblysServices, assemblysServices1)
                .InstancePerDependency()//瞬时单例
               .AsImplementedInterfaces()////自动以其实现的所有接口类型暴露（包括IDisposable接口）
               .EnableInterfaceInterceptors(); //引用Autofac.Extras.DynamicProxy;


            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>))
                .InstancePerDependency()
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors();

        }
        #endregion
    }
}

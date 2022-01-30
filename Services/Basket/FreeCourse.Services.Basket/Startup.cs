using FreeCource.Shared.Services;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket
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
            services.AddHttpContextAccessor();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));
            //Sistem ayaða kaldýðýnda Redis ile baðlantý kurulsun
            //
            services.AddSingleton<RedisService>(sp =>
            {
                //ServiceProvider DI eklemiþ olduðumuz sýnýflarý alalým demek.
                //OptionPattern kullanýlýyor.
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;//appsettings deki RedisSettings deðerleri alýp class aktarýr
                var redis = new RedisService(redisSettings.Host, redisSettings.Port);// sistem ayaða kaldýðýnda RedisService'i çalýþtýrýyoruz.
                redis.Connect();
                return redis;
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Basket", Version = "v1" });
            });
            //JWT ile bir kullanýcýnýn request yapmasýný ve payloadda sub key word olmasýný bekliyoruz.
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //Gelen JWT sub key'ini mapleme diyoruz. çünkü sub yerine uzun bir url ile geliyor
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityServerURL"];
                options.Audience = "resource_basket";
                options.RequireHttpsMetadata = false;
            });
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Basket v1"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Iamport.RestApi;
using Iamport.RestApi.Apis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using Sample.AspNetCore.Repositories;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Sample.AspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.local.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 리포지터리 등록
            // 샘플이라 단순한 in-memory static 저장소를 사용합니다.
            services.AddSingleton<CheckoutRepository>();
            services.AddSingleton<PaymentRepository>();

            // 아임포트 서비스 등록
            services.AddSingleton(serviceProvider =>
            {
                var options =
                    serviceProvider
                    .GetService<IOptions<IamportHttpClientOptions>>();
                return options.Value;
            });
            services.AddSingleton<IIamportClient, IamportHttpClient>();
            services.AddSingleton<IPaymentsApi, PaymentsApi>();
            services.AddSingleton<ISubscribeApi, SubscribeApi>();
            services.Configure<IamportHttpClientOptions>(
                Configuration.GetSection("iamport"));

            // Add framework services.
            services.AddMvc();
            // 다음 설정을 하지 않으면 View 렌더링시 Unicode 문자열이 &#1234; 형식으로 출력됩니다.
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using ConsulServiceRegistration.ConsulServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace ConsulServiceRegistration
{
    public class ConsulRegistrationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddHealthChecks();
            context.Services.AddConsul(configuration);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            //// 获取主机生命周期管理接口
            //var lifetime = context.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
            //// 获取服务配置项
            //var serviceOptions = context.ServiceProvider.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;

            app.UseConsul();

        }
    }
}

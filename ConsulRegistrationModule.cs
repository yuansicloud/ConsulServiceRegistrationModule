using ConsulServiceRegistration.ConsulServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
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

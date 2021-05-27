using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServiceRegistration
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Hosting;
    using Consul;

    namespace ConsulServiceRegistration
    {
        // Consul服务注册
        public static class ConsulRegistrationExtensions
        {
            public static void AddConsul(this IServiceCollection service, IConfiguration configuration)
            {
                //读取配置文件
                service.Configure<ConsulServiceOptions>(configuration.GetSection(nameof(ConsulServiceOptions)));
            }
            public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
            {
                // 获取主机生命周期管理接口
                var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

                // 获取服务配置项
                var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;


                // 健康检查
                app.UseHealthChecks(serviceOptions.HealthCheck);

                // 服务Id，唯一
                serviceOptions.ServiceId ??= Guid.NewGuid().ToString("N");
                var consulClient = new ConsulClient(configuration =>
                {
                    // 服务注册的地址，集群中任意一个地址
                    configuration.Address = new Uri(serviceOptions.ConsulAddress);
                });
                // 获取当前服务地址和端口，这里自动获取，也可以配置
                var features = app.Properties["server.Features"] as FeatureCollection;
                Uri uri = null;
                var address = features.Get<IServerAddressesFeature>().Addresses?.FirstOrDefault();
                if (address == null)
                {
                    // 方便使用命令启用多个服务
                    uri = new Uri(serviceOptions.BaseUrl);
                }
                else
                {
                    uri = new Uri(address);
                }
                // 节点服务注册对象
                var registration = new AgentServiceRegistration()
                {
                    ID = serviceOptions.ServiceId,
                    Name = serviceOptions.ServiceName,
                    Address = uri.Host,
                    Port = uri.Port,
                    Check = new AgentServiceCheck
                    {
                        // 超时时间
                        Timeout = TimeSpan.FromSeconds(5),
                        // 服务停止多久后注销服务
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        // 健康检查地址
                        HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceOptions.HealthCheck}",
                        // 检查健康时间间隔
                        Interval = TimeSpan.FromSeconds(10)
                    }
                };
                // 注册服务
                consulClient.Agent.ServiceRegister(registration).Wait();
                // 应用程序终止时，注销服务
                lifetime.ApplicationStopping.Register(() => {
                    consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
                });

                return app;
            }
        }
    }
}

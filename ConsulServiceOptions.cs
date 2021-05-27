using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServiceRegistration
{
    public class ConsulServiceOptions
    {
        // 服务注册地址（Consul的地址）
        public string ConsulAddress { get; set; }
        // 服务Id
        public string ServiceId { get; set; }
        // 服务名称
        public string ServiceName { get; set; }
        // 健康检查地址
        public string HealthCheck { get; set; }

        /// <summary>
        /// 本机地址
        /// </summary>
        public string BaseUrl { get; set; }
    }
}

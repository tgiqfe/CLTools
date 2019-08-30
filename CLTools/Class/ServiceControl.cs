using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace CLTools.Class
{
    class ServiceControl
    {
        /// <summary>
        /// サービス名からServiceControllerを取得
        /// </summary>
        /// <param name="serviceName">ServiceName, DisplayNameどちらでも可。ServiceName優先</param>
        /// <returns>ServiceControllerインスタンス。指定した名前に一致したサービスが無い場合はnull</returns>
        public static ServiceController GetServiceController(string serviceName)
        {
            ServiceController retSV = ServiceController.GetServices().FirstOrDefault(
                x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            if (retSV == null)
            {
                retSV = ServiceController.GetServices().FirstOrDefault(
                    x => x.DisplayName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }
            return retSV;
        }
    }
}

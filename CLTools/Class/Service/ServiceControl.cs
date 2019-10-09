using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace CLTools
{
    class ServiceControl
    {
        /// <summary>
        /// サービス名からServiceControllerを取得
        /// </summary>
        /// <param name="serviceName">ServiceName, DisplayNameどちらでも可。ServiceName優先</param>
        /// <returns>ServiceControllerインスタンス。指定した名前に一致したサービスが無い場合はnull</returns>
        /*
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
        */



        public static ServiceController[] GetServiceController(string serviceName)
        {
            if (serviceName.Contains("*"))
            {
                string patternString = Regex.Replace(serviceName, ".",
                    x =>
                    {
                        string y = x.Value;
                        if (y.Equals("?")) { return "."; }
                        else if (y.Equals("*")) { return ".*"; }
                        else { return Regex.Escape(y); }
                    });
                if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
                if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
                Regex regPattern = new Regex(patternString, RegexOptions.IgnoreCase);

                return ServiceController.GetServices().Where(x =>
                    regPattern.IsMatch(x.ServiceName) || regPattern.IsMatch(x.DisplayName)).ToArray();
            }
            else
            {
                ServiceController retSV = ServiceController.GetServices().FirstOrDefault(
                    x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                if (retSV == null)
                {
                    retSV = ServiceController.GetServices().FirstOrDefault(
                        x => x.DisplayName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                }
                return new ServiceController[] { retSV };
            }
        }

        public static ServiceController[] GetServiceController(string serviceName, bool ignoreServiceName, bool ignoreDiplayName)
        {
            if (serviceName.Contains("*"))
            {
                string patternString = Regex.Replace(serviceName, ".",
                x =>
                {
                    string y = x.Value;
                    if (y.Equals("?")) { return "."; }
                    else if (y.Equals("*")) { return ".*"; }
                    else { return Regex.Escape(y); }
                });
                if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
                if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
                Regex regPattern = new Regex(patternString, RegexOptions.IgnoreCase);
                if (ignoreServiceName && !ignoreDiplayName)
                {
                    return ServiceController.GetServices().Where(x =>
                        regPattern.IsMatch(x.DisplayName)).ToArray();
                }
                else if (!ignoreServiceName && ignoreDiplayName)
                {
                    return ServiceController.GetServices().Where(x =>
                           regPattern.IsMatch(x.ServiceName)).ToArray();
                }
                else if (!ignoreServiceName && !ignoreDiplayName)
                {
                    return ServiceController.GetServices().Where(x =>
                        regPattern.IsMatch(x.ServiceName) || regPattern.IsMatch(x.DisplayName)).ToArray();
                }
                return new ServiceController[] { null };
            }
            else
            {
                ServiceController retSV = null;
                if (!ignoreServiceName)
                {
                    retSV = ServiceController.GetServices().FirstOrDefault(
                    x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                }
                if (retSV == null && !ignoreDiplayName)
                {
                    retSV = ServiceController.GetServices().FirstOrDefault(
                        x => x.DisplayName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                }
                return new ServiceController[] { retSV };
            }
        }
    }
}

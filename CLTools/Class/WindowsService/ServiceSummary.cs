using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;

namespace CLTools.Class
{
    public class ServiceSummary
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string StartupType { get; set; }
        public string[] ServicesDependedOn { get; set; }
        public string[] DependentServices { get; set; }

        public ServiceSummary() { }
        public ServiceSummary(string name)
        {
            ServiceController sc = ServiceControl.GetServiceController(name);
            this.Name = sc.ServiceName;
            this.DisplayName = sc.DisplayName;

            this.StartupType = sc.StartType.ToString();
            if (sc.StartType == ServiceStartMode.Automatic)
            {
                int delayedAutoStart = (int)Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + sc.ServiceName,
                    "DelayedAutostart", 0);
                if (delayedAutoStart > 0)
                {
                    this.StartupType = "DelayedAutomatic";
                }
            }

            this.ServicesDependedOn = sc.ServicesDependedOn.Select(x => x.ServiceName).ToArray();
            this.DependentServices = sc.DependentServices.Select(x => x.ServiceName).ToArray();

        }

    }
}

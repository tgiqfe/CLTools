﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.ServiceProcess;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsLifecycle.Stop, "WindowsService")]
    public class StopCLService : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0), Alias("ServiceName")]
        public string Name { get; set; }
        [Parameter(Position = 1)]
        public SwitchParameter RunAsync { get; set; }
        [Parameter]
        public SwitchParameter IgnoreServiceName { get; set; }
        [Parameter]
        public SwitchParameter IgnoreDisplayName { get; set; }

        protected override void ProcessRecord()
        {
            List<ServiceSummary> scList = new List<ServiceSummary>();
            foreach (ServiceController sc in ServiceControl.GetServiceController(
                Name, IgnoreServiceName, IgnoreDisplayName))
            {
                scList.Add(new ServiceSummary(sc));
                if (sc.CanStop)
                {
                    sc.Stop();
                    if (!RunAsync)
                    {
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                }
            }
            WriteObject(scList);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.ServiceProcess;
using CLTools.Class;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsLifecycle.Start, "WindowsService")]
    public class StartWindowsService : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }
        [Parameter(Position = 1)]
        public SwitchParameter RunAsync { get; set; }

        protected override void ProcessRecord()
        {
            ServiceController sc = ServiceControl.GetServiceController(Name);
            if(sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();
                if (!RunAsync)
                {
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            WriteObject(new ServiceSummary(sc));
        }
    }
}

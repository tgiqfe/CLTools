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
    [Cmdlet(VerbsLifecycle.Stop, "WindowsService")]
    public class StopWindowsService : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }
        [Parameter(Position = 1)]
        public SwitchParameter RunAsync { get; set; }

        protected override void ProcessRecord()
        {
            ServiceController sc = ServiceControl.GetServiceController(Name);
            if (sc.CanStop)
            {
                sc.Stop();
                if (!RunAsync)
                {
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            WriteObject(new ServiceSummary(sc));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLTools.Class;
using System.Management.Automation;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "WindowsService")]
    public class GetCLService : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0), Alias("ServiceName")]
        public string Name { get; set; }
        [Parameter]
        public SwitchParameter IgnoreServiceName { get; set; }
        [Parameter]
        public SwitchParameter IgnoreDisplayName { get; set; }

        protected override void ProcessRecord()
        {
            List<ServiceSummary> scList = new List<ServiceSummary>(
                ServiceControl.GetServiceController(Name, IgnoreServiceName, IgnoreDisplayName).
                    Select(x => new ServiceSummary(x)));
            WriteObject(scList);
        }
    }
}

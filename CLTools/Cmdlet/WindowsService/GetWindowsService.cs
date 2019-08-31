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
    public class GetWindowsService : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new ServiceSummary(ServiceControl.GetServiceController(Name)));
        }
    }
}

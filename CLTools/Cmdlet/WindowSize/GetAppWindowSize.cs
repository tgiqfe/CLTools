using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;

namespace CLTools.Cmdlet.WindowSize
{
    [Cmdlet(VerbsCommon.Get, "AppWindowSize")]
    public class GetAppWindowSize : PSCmdlet
    {
        [Parameter, Alias("Name")]
        public string ApplicationName { get; set; }

        protected override void ProcessRecord()
        {
            
        }
    }
}

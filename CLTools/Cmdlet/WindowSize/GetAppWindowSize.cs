using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;
using CLTools.Class.WindowSize;

namespace CLTools.Cmdlet.WindowSize
{
    [Cmdlet(VerbsCommon.Get, "AppWindowSize")]
    public class GetAppWindowSize : PSCmdlet
    {
        [Parameter, Alias("Name")]
        public string ApplicationName { get; set; }

        protected override void ProcessRecord()
        {
            var summaryList = new List<AppWindowSizeSummary>();

            Process[] procs = string.IsNullOrEmpty(ApplicationName) ?
                Process.GetProcesses() :
                Process.GetProcessesByName(ApplicationName);
            foreach(Process proc in procs)
            {
                var summary = new AppWindowSizeSummary(proc);
                if (summary.IsWindowProcess)
                {
                    summaryList.Add(new AppWindowSizeSummary(proc));
                }
            }

            WriteObject(summaryList);
        }
    }
}

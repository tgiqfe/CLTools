using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using CLTools.Class;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "SoundVolume")]
    public class GetCLVolume : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new VolumeSummary());
        }
    }
}

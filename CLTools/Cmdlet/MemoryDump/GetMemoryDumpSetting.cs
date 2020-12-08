using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace CLTools.Cmdlet.MemoryDump
{
    [Cmdlet(VerbsCommon.Get, "MemoryDumpSetting")]
    public class GetMemoryDumpSetting : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var setting = new Class.MemoryDump.MemoryDumpSetting();
            setting.Load();

            WriteObject(setting);
        }
    }
}

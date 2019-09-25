using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using CLTools.Class;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "CLUser")]
    public class GetCLUser : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            UserSummary user = new UserSummary(Name);
            WriteObject(user);
        }
    }
}

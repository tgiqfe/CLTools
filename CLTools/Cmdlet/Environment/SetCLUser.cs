using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "CLUser")]
    public class SetCLUser : PSCmdlet
    {
        [Parameter(Mandatory=true)]
        public string Name { get; set; }
        [Parameter]
        public bool Enabled { get; set; }

        protected override void ProcessRecord()
        {
            UserSummary user = new UserSummary(Name);

        }

    }
}

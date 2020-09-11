using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management;

namespace CLTools.Cmdlet.Environment
{
    [Cmdlet(VerbsCommon.Set, "LocalUser")]
    public class SetLocalUser : PSCmdlet
    {
        [Parameter(Mandatory=true, Position = 0)]
        public string Name { get; set; }
        [Parameter]
        public bool? Enabled { get; set; }

        protected override void ProcessRecord()
        {
            if(Enabled != null)
            {
                ManagementObject mo = new ManagementClass("Win32_UserAccount").
                    GetInstances().
                    OfType<ManagementObject>().
                    FirstOrDefault(x => x["Name"].ToString().Equals(Name, StringComparison.OrdinalIgnoreCase));
                if (mo != null)
                {
                    mo.SetPropertyValue("Disabled", !(bool)Enabled);
                    mo.Put();
                }
            }
        }

    }
}

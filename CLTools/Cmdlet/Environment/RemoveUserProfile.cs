using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Remove, "UserProfile")]
    public class RemoveUserProfile : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            UserSummary user = new UserSummary(Name);
            ManagementObject mo = new ManagementClass("Win32_UserProfile").
                GetInstances().
                OfType<ManagementObject>().
                FirstOrDefault(x => x["SID"].ToString().Equals(user.SID, StringComparison.OrdinalIgnoreCase));
            if (mo != null)
            {
                mo.Delete();
            }
        }
    }
}

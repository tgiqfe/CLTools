using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using CLTools.Class.Environment;
using System.Management;

namespace CLTools.Cmdlet.Environment
{
    [Cmdlet(VerbsCommon.Get, "CLLocalUser")]
    public class GetCLLocalUser : PSCmdlet
    {
        [Parameter]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(Name))
            {
                //  Name 未指定の場合、全ローカルユーザーを取得
                var summaryList = new List<UserSummary>();
                foreach (ManagementObject mo in new ManagementClass("Win32_UserAccount").
                    GetInstances().
                    OfType<ManagementObject>().
                    Where(x => (bool)x["LocalAccount"] && x["Name"] is string))
                {
                    summaryList.Add(new UserSummary(mo["Name"].ToString()));
                }
                WriteObject(summaryList);
            }
            else
            {
                //  指定したNameのユーザーを取得 (一応ドメインユーザーにも対応)
                UserSummary user = new UserSummary(Name);
                WriteObject(user);
            }
        }
    }
}

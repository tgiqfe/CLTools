using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsCommon.Get, "LocalGPO")]
    public class GetLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter Machine { get; set; }
        [Parameter]
        public SwitchParameter User { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }
        [Parameter]
        public SwitchParameter ToSetCmdlet { get; set; }
        [Parameter]
        public SwitchParameter ToNewCmdlet { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            Class.GPO.GroupPolicy gp = new Class.GPO.GroupPolicy();

            Action<List<Class.GPO.GroupPolicyObject>> returnObject = (gpoList) =>
            {
                var cmdletList = new List<string>();

                if (ToSetCmdlet)
                {
                    foreach (Class.GPO.GroupPolicyObject gpo in gpoList)
                    {
                        cmdletList.Add(string.Format(
                            "Set-LocalGPO -Path \"{0}\" -Name \"{1}\" -Value {2} -Type {3}",
                                gpo.Path,
                                gpo.Name,
                                (gpo.Type == Class.GPO.RegistryControl.REG_DWORD || gpo.Type == Class.GPO.RegistryControl.REG_QWORD) ? gpo.Value : "\"" + gpo.Value + "\"",
                                gpo.Type));
                    }
                    WriteObject(string.Join("\r\n", cmdletList));
                }
                else if (ToNewCmdlet)
                {
                    foreach (Class.GPO.GroupPolicyObject gpo in gpoList)
                    {
                        cmdletList.Add(string.Format(
                            "New-LocalGPO -Path \"{0}\" -Name \"{1}\" -Value {2} -Type {3}",
                                gpo.Path,
                                gpo.Name,
                                (gpo.Type == Class.GPO.RegistryControl.REG_DWORD || gpo.Type == Class.GPO.RegistryControl.REG_QWORD) ?
                                    gpo.Value :
                                    "\"" + gpo.Value + "\"",
                                gpo.Type));
                    }
                    WriteObject(string.Join("\r\n", cmdletList));
                }
                else
                {
                    WriteObject(gpoList.ToArray());
                }
            };

            if (!string.IsNullOrEmpty(TargetPolFile))
            {
                //  指定のPolファイルから読み込み
                if (File.Exists(TargetPolFile))
                {
                    gp.SetMachine(Class.GPO.PolFile.Create(TargetPolFile));
                    returnObject(gp.Machine);
                }
            }
            else if (Machine)
            {
                //  コンピュータの構成
                if (File.Exists(Class.GPO.Item.MACHINE_POL_PATH))
                {
                    gp.SetMachine(Class.GPO.PolFile.Create(Class.GPO.Item.MACHINE_POL_PATH));
                    returnObject(gp.Machine);
                }
            }
            else if (User)
            {
                //  ユーザーの構成
                if (File.Exists(Class.GPO.Item.USER_POL_PATH))
                {
                    gp.SetUser(Class.GPO.PolFile.Create(Class.GPO.Item.USER_POL_PATH));
                    returnObject(gp.User);
                }
            }
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            System.Environment.CurrentDirectory = _currentDirectory;
        }
    }
}

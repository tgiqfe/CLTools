using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using CLTools.Class.GPO;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsDiagnostic.Test, "LocalGPO")]
    public class TestLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter Machine { get; set; }
        [Parameter]
        public SwitchParameter User { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }
        [Parameter, Alias("GPO")]
        public GroupPolicyObject[] GroupPolicyObject { get; set; }
        [Parameter, Alias("Path")]
        public string RegistryPath { get; set; }
        [Parameter, Alias("Name")]
        public string RegistryName { get; set; }
        [Parameter, Alias("Type"), ValidateSet("REG_SZ", "REGBINARY", "REG_DWORD", "REG_QWORD", "REG_MULTI_SZ", "REG_EXPAND_SZ", "REG_NONE")]
        public string RegistryType { get; set; }
        [Parameter, Alias("Value")]
        public string RegistryValue { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            if ((this.GroupPolicyObject == null || this.GroupPolicyObject.Length == 0) &&
                !string.IsNullOrEmpty(RegistryPath) &&
                !string.IsNullOrEmpty(RegistryName) &&
                !string.IsNullOrEmpty(RegistryType) &&
                !string.IsNullOrEmpty(RegistryValue))
            {
                this.GroupPolicyObject = new GroupPolicyObject[1]
                {
                    new GroupPolicyObject()
                    {
                        Path = RegistryPath,
                        Name = RegistryName,
                        Type = RegistryType,
                        Value = RegistryValue,
                    }
                };
            }

            GroupPolicy gp = new GroupPolicy();

            Func<List<GroupPolicyObject>, bool> checkContain = (gpoList) =>
            {
                if(gpoList.Count > 0)
                {
                    bool result = true;
                    foreach (GroupPolicyObject paramGPO in this.GroupPolicyObject)
                    {
                        result &= gpoList.Any(x =>
                            x.Path.Equals(paramGPO.Path, StringComparison.OrdinalIgnoreCase) &&
                            x.Name.Equals(paramGPO.Name, StringComparison.OrdinalIgnoreCase) &&
                            x.Value == paramGPO.Value &&
                            x.Type == paramGPO.Type);
                    }
                    return result;
                }
                else
                {
                    return false;
                }
            };

            if (!string.IsNullOrEmpty(TargetPolFile))
            {
                if (File.Exists(TargetPolFile))
                {
                    gp.SetMachine(PolFile.Create(TargetPolFile));
                    WriteObject(checkContain(gp.Machine));
                }
            }
            else if (Machine)
            {
                if (File.Exists(Item.MACHINE_POL_PATH))
                {
                    gp.SetMachine(PolFile.Create(Item.MACHINE_POL_PATH));
                    WriteObject(checkContain(gp.Machine));
                }
            }
            else if (User)
            {
                if (File.Exists(Item.USER_POL_PATH))
                {
                    gp.SetUser(PolFile.Create(Item.USER_POL_PATH));
                    WriteObject(checkContain(gp.User));
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

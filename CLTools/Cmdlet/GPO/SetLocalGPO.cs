using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsCommon.Set, "LocalGPO")]
    public class SetLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter Machine { get; set; }
        [Parameter]
        public SwitchParameter User { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }
        [Parameter, Alias("GPO")]
        public Class.GPO.GroupPolicyObject[] GroupPolicyObject { get; set; }
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
            Functions.CheckAdmin();

            if ((this.GroupPolicyObject == null || this.GroupPolicyObject.Length == 0) &&
                !string.IsNullOrEmpty(RegistryPath) &&
                !string.IsNullOrEmpty(RegistryName) &&
                !string.IsNullOrEmpty(RegistryType) &&
                !string.IsNullOrEmpty(RegistryValue))
            {
                this.GroupPolicyObject = new Class.GPO.GroupPolicyObject[1]
                {
                    new Class.GPO.GroupPolicyObject()
                    {
                        Path = RegistryPath,
                        Name = RegistryName,
                        Type = RegistryType,
                        Value = RegistryValue,
                    }
                };
            }

            if (!string.IsNullOrEmpty(TargetPolFile))
            {
                Class.GPO.PolFile pol = Class.GPO.PolFile.Create(TargetPolFile);
                foreach (Class.GPO.GroupPolicyObject gpo in this.GroupPolicyObject)
                {
                    pol.SetValue(gpo.ConvertToPolEntry());
                }
                if (!Directory.Exists(Path.GetDirectoryName(TargetPolFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(TargetPolFile));
                }
                pol.Save(TargetPolFile);
            }
            else if (Machine)
            {
                Class.GPO.PolFile pol = Class.GPO.PolFile.Create(Class.GPO.Item.MACHINE_POL_PATH);
                foreach (Class.GPO.GroupPolicyObject gpo in this.GroupPolicyObject)
                {
                    pol.SetValue(gpo.ConvertToPolEntry());
                }
                if (!Directory.Exists(Path.GetDirectoryName(Class.GPO.Item.MACHINE_POL_PATH)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Class.GPO.Item.MACHINE_POL_PATH));
                }
                pol.Save(Class.GPO.Item.MACHINE_POL_PATH);
            }
            else if (User)
            {
                Class.GPO.PolFile pol = Class.GPO.PolFile.Create(Class.GPO.Item.USER_POL_PATH);
                foreach (Class.GPO.GroupPolicyObject gpo in this.GroupPolicyObject)
                {
                    pol.SetValue(gpo.ConvertToPolEntry());
                }
                if (!Directory.Exists(Path.GetDirectoryName(Class.GPO.Item.USER_POL_PATH)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Class.GPO.Item.USER_POL_PATH));
                }
                pol.Save(Class.GPO.Item.USER_POL_PATH);
            }
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            System.Environment.CurrentDirectory = _currentDirectory;
        }
    }
}

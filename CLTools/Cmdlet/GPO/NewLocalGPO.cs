using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLTools.Class.GPO;
using System.Management.Automation;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsCommon.New, "LocalGPO")]
    public class NewLocalGPO : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0), Alias("Path")]
        public string RegistryPath { get; set; }
        [Parameter(Mandatory = true, Position = 1), Alias("Name")]
        public string RegistryName { get; set; }
        [Parameter(Mandatory = true, Position = 2), Alias("Value")]
        public string RegistryValue { get; set; }
        [Parameter(Mandatory = true, Position = 3), Alias("Type"), ValidateSet("REG_SZ", "REGBINARY", "REG_DWORD", "REG_QWORD", "REG_MULTI_SZ", "REG_EXPAND_SZ", "REG_NONE")]
        public string RegistryType { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            var gpo = new GroupPolicyObject()
            {
                Path = RegistryPath,
                Name = RegistryName,
                Type = RegistryType,
                Value = RegistryValue,
            };

            WriteObject(gpo);
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            System.Environment.CurrentDirectory = _currentDirectory;
        }
    }
}

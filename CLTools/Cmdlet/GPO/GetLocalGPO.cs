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
    [Cmdlet(VerbsCommon.Get, "LocalGPO")]
    public class GetLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter Machine { get; set; }
        [Parameter]
        public SwitchParameter User { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            GroupPolicy gp = new GroupPolicy();

            if (!string.IsNullOrEmpty(TargetPolFile))
            {
                if (File.Exists(TargetPolFile))
                {
                    gp.SetMachine(PolFile.Create(TargetPolFile));
                    WriteObject(gp.Machine.ToArray());
                }
            }
            else if (Machine)
            {
                if (File.Exists(Item.MACHINE_POL_PATH))
                {
                    gp.SetMachine(PolFile.Create(Item.MACHINE_POL_PATH));
                    WriteObject(gp.Machine.ToArray());
                }
            }
            else if (User)
            {
                if (File.Exists(Item.USER_POL_PATH))
                {
                    gp.SetUser(PolFile.Create(Item.USER_POL_PATH));
                    WriteObject(gp.User.ToArray());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using CLTools.Class.GPO;
using CLTools.Serialize;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsData.Export, "LocalGPO")]
    public class ExportLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter IgnoreMachine { get; set; }
        [Parameter]
        public SwitchParameter IgnoreUser { get; set; }
        [Parameter(Position = 0), Alias("Path")]
        public string ExportFilePath { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            var gp = new GroupPolicy();

            if (!string.IsNullOrEmpty(TargetPolFile))
            {
                //  指定したファイルを「コンピュータの構成」として取得
                if (File.Exists(TargetPolFile))
                {
                    gp.SetMachine(PolFile.Create(TargetPolFile));
                }
            }
            else
            {
                //  「コンピュータの構成」を取得
                if (!IgnoreMachine && File.Exists(Item.MACHINE_POL_PATH))
                {
                    gp.SetMachine(PolFile.Create(Item.MACHINE_POL_PATH));
                }

                //  「ユーザーの構成」を取得
                if (!IgnoreUser && File.Exists(Item.USER_POL_PATH))
                {
                    gp.SetUser(PolFile.Create(Item.USER_POL_PATH));
                }
            }

            if (string.IsNullOrEmpty(ExportFilePath))
            {
                WriteObject(gp);
            }
            else
            {
                DataSerializer.Serialize<GroupPolicy>(gp, ExportFilePath);
            }
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            Environment.CurrentDirectory = _currentDirectory;
        }
    }
}

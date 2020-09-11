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
    [Cmdlet(VerbsData.Import, "LocalGPO")]
    public class ImportLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter IgnoreMachine { get; set; }
        [Parameter]
        public SwitchParameter IgnoreUser { get; set; }
        [Parameter(Position = 0), Alias("Path")]
        public string ImportFilePath { get; set; }
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
            Functions.CheckAdmin();

            if (!string.IsNullOrEmpty(ImportFilePath))
            {
                GroupPolicy groupPolicy = DataSerializer.Deserialize<GroupPolicy>(ImportFilePath);

                if (!IgnoreMachine && groupPolicy.Machine != null && groupPolicy.Machine.Count > 0)
                {
                    PolFile pol = new PolFile();
                    foreach (GroupPolicyObject gpo in groupPolicy.Machine)
                    {
                        pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                    }
                    //pol.Save(MACHINE_POL_PATH);
                }
                if (!IgnoreUser && groupPolicy.User != null && groupPolicy.User.Count > 0)
                {
                    PolFile pol = new PolFile();
                    foreach (GroupPolicyObject gpo in groupPolicy.User)
                    {
                        pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                    }
                    //pol.Save(USER_POL_PATH);
                }
                if (!string.IsNullOrEmpty(TargetPolFile) && groupPolicy.Machine != null && groupPolicy.Machine.Count > 0)
                {
                    PolFile pol = new PolFile();
                    foreach (GroupPolicyObject gpo in groupPolicy.Machine)
                    {
                        pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                    }
                    pol.Save(TargetPolFile);
                }
            }
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            Environment.CurrentDirectory = _currentDirectory;
        }
    }
}

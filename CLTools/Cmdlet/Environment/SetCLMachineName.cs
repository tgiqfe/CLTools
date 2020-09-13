using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;

namespace CLTools.Cmdlet.Environment
{
    /// <summary>
    /// ホスト名を変更する
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "CLMachineName")]
    public class SetCLMachineName : PSCmdlet
    {
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public bool Restart { get; set; }

        protected override void ProcessRecord()
        {
            bool ret = Functions.CheckAdmin();
            if (!ret) { return; }

            if (!string.IsNullOrEmpty(Name) &&
                !Name.Equals(System.Environment.MachineName, StringComparison.OrdinalIgnoreCase))
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments =
                        $"/c wmic computersystem where name=\"%computername%\" call rename name=\"{Name}\"";
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.WaitForExit();
                }

                if (Restart)
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = "cmd.exe";
                        proc.StartInfo.Arguments =
                            $"/c shutdown /r /t 0";
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.Start();
                    }
                }
            }
        }
    }
}

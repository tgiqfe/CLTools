using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;
using CLTools.Class.WindowSize;

namespace CLTools.Cmdlet.WindowSize
{
    [Cmdlet(VerbsCommon.Set, "AppWindowSize")]
    public class SetAppWindowSize : PSCmdlet
    {
        [Parameter(Mandatory = true), Alias("Name")]
        public string ApplicationName { get; set; }

        [Parameter]
        public int? Left { get; set; }
        [Parameter]
        public int? Top { get; set; }
        [Parameter]
        public int? Width { get; set; }
        [Parameter]
        public int? Height { get; set; }
        [Parameter]
        public int[] Size { get; set; }
        [Parameter]
        public SwitchParameter WithDropShadow { get; set; }

        protected override void ProcessRecord()
        {
            if (Size != null && Size.Length >= 4 &&
                (Left == null || Top == null || Width == null || Height == null))
            {
                this.Left = Size[0];
                this.Top = Size[1];
                this.Width = Size[2];
                this.Height = Size[3];
            }

            Process[] processes = Process.GetProcessesByName(ApplicationName);
            foreach (Process process in processes)
            {
                var summary = new AppWindowSizeSummary(process);

                if (WithDropShadow)
                {
                    //  DropShadowごとサイズ変更
                    summary.ChangeWindowSize((int)Left, (int)Top, (int)Width, (int)Height);
                }
                else
                {
                    //  DropShadowを除いてサイズ変更
                    summary.ChangeWindowSize(
                        (int)Left - (summary.X - summary.sX),
                        (int)Top - (summary.Y - summary.sY),
                        (int)Width + (summary.sWidth - summary.Width),
                        (int)Height + (summary.sHeight - summary.Height));
                }
            }
        }
    }
}

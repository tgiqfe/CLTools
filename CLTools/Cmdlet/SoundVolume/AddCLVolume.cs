using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Add, "SoundVolume")]
    public class AddCLVolume : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public int Level { get; set; }
        [Parameter(Position = 1)]
        public bool Mute { get; set; }

        protected override void ProcessRecord()
        {
            int nowLevel = (int)(Math.Round(Sound.GetVolume(), 2, MidpointRounding.AwayFromZero) * 100);
            Sound.SetVolume((float)((nowLevel + Level) / 100.0));
            Sound.SetMute(Mute);

            WriteObject(new VolumeSummary());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using CLTools.Class;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "SoundVolume")]
    public class SetSoundVolume : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public int Level { get; set; }
        [Parameter(Position = 1)]
        public bool Mute { get; set; }

        protected override void ProcessRecord()
        {
            Sound.SetVolume((float)(Level / 100.0));
            Sound.SetMute(Mute);

            WriteObject(new VolumeSummary());
        }
    }
}

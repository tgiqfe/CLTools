using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools
{
    public class VolumeSummary
    {
        public int Level { get; set; }
        public bool IsMute { get; set; }

        public VolumeSummary()
        {
            Load();
        }

        public void Load()
        {
            this.Level = (int)(Math.Round(Class.Sound.GetVolume(), 2, MidpointRounding.AwayFromZero) * 100);
            this.IsMute = Class.Sound.GetMute();
        }
    }
}

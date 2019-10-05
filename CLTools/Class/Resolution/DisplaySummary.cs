using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CLTools.Class
{
    public class DisplaySummary
    {
        const string REG_CONFIGURATION = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Configuration";
        const string PARAM_PrimSurfSize_cx = "PrimSurfSize.cx";
        const string PARAM_PrimSurfSize_cy = "PrimSurfSize.cy";
        const string PARAM_Timestamp = "Timestamp";

        public List<int[]> Resolution { get; set; }
        public int DisplayNumber { get; set; }

        public DisplaySummary()
        {
            Load();
        }

        public void Load()
        {
            MonitorRegistry mr = new MonitorRegistry();
            mr.CheckRegMonitor();

            Resolution = new List<int[]>();
            using (RegistryKey regKey =
                Registry.LocalMachine.OpenSubKey(
                    string.Format(@"{0}\{1}", REG_CONFIGURATION, mr.LatestMonitor.MonitorID), false))
            {
                foreach (string subkey in regKey.GetSubKeyNames())
                {
                    int resolutionX = (int)Registry.GetValue(
                        string.Format(@"HKEY_LOCAL_MACHINE\{0}\{1}\{2}", REG_CONFIGURATION, mr.LatestMonitor.MonitorID, subkey),
                        PARAM_PrimSurfSize_cx, "");
                    int resolutionY = (int)Registry.GetValue(
                        string.Format(@"HKEY_LOCAL_MACHINE\{0}\{1}\{2}", REG_CONFIGURATION, mr.LatestMonitor.MonitorID, subkey),
                        PARAM_PrimSurfSize_cy, "");
                    Resolution.Add(new int[2] { resolutionX, resolutionY });
                }
            }





        }
    }
}

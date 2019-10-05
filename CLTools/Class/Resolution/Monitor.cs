using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools
{
    /// <summary>
    /// モニター情報格納用クラス
    /// </summary>
    public class Monitor
    {
        public string MonitorID { get; set; }
        public string MonitorID_pre { get; set; }
        public string MonitorID_suf { get; set; }
        public DateTime TimeStamp { get; set; }

        public Monitor() { }
        public Monitor(string deviceID, long timeStamp)
        {
            this.MonitorID = deviceID;
            this.MonitorID_pre = deviceID.Substring(0, deviceID.IndexOf("^"));
            this.MonitorID_suf = deviceID.Substring(deviceID.IndexOf("^") + 1);
            this.TimeStamp = DateTime.FromFileTime(timeStamp);
        }
    }
}

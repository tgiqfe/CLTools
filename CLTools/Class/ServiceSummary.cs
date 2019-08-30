using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools.Class
{
    public class ServiceSummary
    {
        public enum ServiceStartMode { Automatic, Manual, Disabled, DelayedAutomatic };

        public string ServiceName { get; set; }
        public string DisplayName { get; set; }

    }
}

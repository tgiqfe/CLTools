using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools
{
    public class MachineSummary
    {
        public string Name { get; set; }    //  FQDNで
        public string HostName { get; set; }
        public string DomainName { get; set; }
        public string WorkgroupName { get; set; }
        public bool Domain { get; set; }


    }
}

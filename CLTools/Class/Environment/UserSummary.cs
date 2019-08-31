using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools.Class
{
    public class UserSummary
    {
        public string Name { get; set; }    //  ドメイン名を含めたフルネーム
        public string UserName { get; set; }
        public bool SystemAcount { get; set; }
        public bool LocalAccount { get; set; }
        public bool DomainAccount { get; set; }

    }
}

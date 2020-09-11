using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools.Class.GPO
{
    public class GroupPolicy
    {
        public List<GroupPolicyObject> Machine { get; set; }
        public List<GroupPolicyObject> User { get; set; }

        public GroupPolicy()
        {
            this.Machine = new List<GroupPolicyObject>();
            this.User = new List<GroupPolicyObject>();
        }

        public void SetMachine(PolFile pol)
        {
            foreach (PolEntry entry in pol.Entries.Values)
            {
                this.Machine.Add(GroupPolicyObject.ConvertFromPolEntry(entry));
            }
        }

        public void SetUser(PolFile pol)
        {
            foreach (PolEntry entry in pol.Entries.Values)
            {
                this.User.Add(GroupPolicyObject.ConvertFromPolEntry(entry));
            }
        }
    }
}

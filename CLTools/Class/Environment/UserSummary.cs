using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace CLTools.Class
{
    [Flags]
    public enum UserType { Unknown = 0, SystemAccount = 1, LocalAccount = 2, DomainAccount = 4 };

    public class UserSummary
    {
        public UserType UserType { get; set; }

        public string Name { get; set; }
        public string Identity { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }

        public string SID { get; set; }

        public bool MustChangePassword { get; set; }
        public bool CannotChangePassword { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool Enabled { get; set; }
        public bool Locked { get; set; }

        public string[] MemberOf { get; set; }
        public string ProfilePath { get; set; }
        public string LogonScript { get; set; }
        public string LocalPath { get; set; }
        public string Connect { get; set; }

        public UserSummary() : this(Environment.UserName) { }
        public UserSummary(string name)
        {
            this.Name = name;
            Load();
        }

        /// <summary>
        /// ユーザー情報を読み取って格納
        /// </summary>
        public void Load()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                ManagementObject mo = new ManagementClass("Win32_UserAccount").
                    GetInstances().
                    OfType<ManagementObject>().
                    Where(x => x["Name"] is string).
                    FirstOrDefault(x => x["Name"].ToString().Equals(Name, StringComparison.OrdinalIgnoreCase));
                if (mo != null)
                {
                    this.Identity = mo["Caption"].ToString();
                    this.FullName = mo["FullName"].ToString();
                    this.Description = mo["Description"].ToString();
                    this.SID = mo["SID"].ToString();




                }
                
                /*
                foreach (ManagementObject moo in new ManagementClass("Win32_UserAccount").
                    GetInstances().
                    OfType<ManagementObject>().
                    Where(x => x["Name"] is string))
                {
                    Console.WriteLine(moo["Name"]);
                }
                */
            }
        }


    }
}

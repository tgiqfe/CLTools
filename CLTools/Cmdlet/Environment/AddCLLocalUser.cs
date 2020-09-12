using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;

namespace CLTools.Cmdlet.Environment
{
    [Cmdlet(VerbsCommon.Add, "CLLocalUser")]
    public class AddCLLocalUser : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string User { get; set; }
        [Parameter(Mandatory = true, Position = 1)]
        public string Password { get; set; }
        [Parameter]
        public SwitchParameter IsAdmin { get; set; }
        [Parameter]
        public string FullName { get; set; }
        [Parameter]
        public string Description { get; set; }

        protected override void ProcessRecord()
        {
            /*
            //  参考)
            //  https://support.microsoft.com/ja-jp/help/306273/how-to-add-a-user-to-the-local-system-by-using-directory-services-and

            DirectoryEntry entry = new DirectoryEntry($"WinNT://{Environment.MachineName},computer");
            DirectoryEntry newUser = entry.Children.Add(User, "user");

            //  パスワードを設定
            if (!string.IsNullOrEmpty(Password))
            {
                //newUser.Invoke("SetPassword", new object[] { Password });
                newUser.Invoke("SetPassword", Password);
            }

            //  フルネームを設定
            if (!string.IsNullOrEmpty(FullName))
            {
                newUser.Invoke("Put", new object[] { "FullName", FullName });
            }

            //  説明を設定
            if (!string.IsNullOrEmpty(Description))
            {
                newUser.Invoke("Put", new object[] { "Description", Description });
            }
            newUser.CommitChanges();
            */

            UserPrincipal user_p = new UserPrincipal(new PrincipalContext(ContextType.Machine));
            user_p.Name = User;
            user_p.SetPassword(Password);
            //  フルネームの設定箇所が見つからない・・・
            //  ユーザー作成後にWMIから変更したほうが良いのかも
            user_p.Enabled = true;
            user_p.PasswordNeverExpires = true;
            user_p.Description = Description;
            user_p.Save();

            //  Users or Administratorsグループに所属
            GroupPrincipal group_p = IsAdmin ?
                GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Machine), "Administrators") :
                GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Machine), "Users");
            group_p.Members.Add(user_p);
            group_p.Save();

            Console.WriteLine(group_p.Name);

        }

    }
}

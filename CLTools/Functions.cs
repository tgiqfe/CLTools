using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace CLTools
{
    internal class Functions
    {
        /// <summary>
        /// 管理者実行しているかどうかの確認
        /// </summary>
        /// <returns></returns>
        public static bool CheckAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("管理者として実行されていません。操作は失敗する可能性があります。");
                Console.ResetColor();
            }
            return isAdmin;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;

namespace CLTools.Class
{
    public class ServiceSummary
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string StartupType { get; set; }
        public bool? TriggerStart { get; set; }
        public string ExecutePath { get; set; }
        public string Description { get; set; }
        public string LogonName { get; set; }
        public int? ProcessId { get; set; }
        public string[] ServicesDependedOn { get; set; }
        public string[] DependentServices { get; set; }

        //  sc qfailer ～ を使用してのサービス回復処理については、そのうち実装する・・・かも

        public ServiceSummary() { }
        public ServiceSummary(ServiceController sc)
        {
            if (sc != null)
            {
                ManagementObject mo = new ManagementClass("Win32_Service").
                    GetInstances().
                    OfType<ManagementObject>().
                    FirstOrDefault(x =>
                        x["Name"] != null &&
                        (x["Name"] as string).Equals(sc.ServiceName, StringComparison.OrdinalIgnoreCase));

                this.Name = sc.ServiceName;
                this.DisplayName = sc.DisplayName;
                this.Status = sc.Status.ToString();

                LoadStartupType(sc, mo);

                this.ExecutePath = mo["PathName"] as string;
                this.Description = mo["Description"] as string;
                this.LogonName = mo["StartName"] as string;
                this.ProcessId = mo["ProcessId"] as int?;

                this.ServicesDependedOn = sc.ServicesDependedOn.Select(x => x.ServiceName).ToArray();
                this.DependentServices = sc.DependentServices.Select(x => x.ServiceName).ToArray();
            }
        }

        /// <summary>
        /// スタートアップの種類を取得 (少し長いのでメソッドを分離)
        /// </summary>
        /// <param name="sc"></param>
        private void LoadStartupType(ServiceController sc, ManagementObject mo)
        {
            this.StartupType = sc.StartType.ToString();

            if (sc.StartType == ServiceStartMode.Automatic && (bool)mo["DelayedAutoStart"])
            {
                this.StartupType = "DelayedAutomatic";
            }
            //  Win32APIからのほうがスマートなので、レジストリから調べる案は廃止
            /*
            int delayedAutoStart = (int)Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + sc.ServiceName,
                "DelayedAutostart", 0);
            if (delayedAutoStart > 0)
            {
                this.StartupType = "DelayedAutomatic";
            }
            */

            if (sc.StartType == ServiceStartMode.Automatic || sc.StartType == ServiceStartMode.Manual)
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "sc";
                    proc.StartInfo.Arguments = "qtriggerinfo \"" + sc.ServiceName + "\"";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    string resultString = proc.StandardOutput.ReadToEnd();
                    this.TriggerStart =
                        resultString.Contains("[SC] QueryServiceConfig2 SUCCESS") &&
                        resultString.Contains("サービス名: " + sc.ServiceName);
                    proc.WaitForExit();
                }
            }
        }
    }
}

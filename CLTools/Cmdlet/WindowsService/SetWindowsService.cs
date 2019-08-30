using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using CLTools.Class;
using System.ComponentModel;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "WindowsService")]
    public class SetWindowsService : PSCmdlet
    {
        #region SetStartupType
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern Boolean ChangeServiceConfig(
            IntPtr hService,
            uint nServiceType,
            uint nStartType,
            uint nErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, IntPtr lpInfo);

        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
        private const int SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 0x00000003;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SERVICE_DELAYED_AUTO_START_INFO { public bool fDelayedAutostart; }

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        private static void ThrowLastWin32Error(string messagePrefix)
        {
            int nError = Marshal.GetLastWin32Error();
            var win32Exception = new Win32Exception(nError);
            string message = string.Format("{0}: {1}", messagePrefix, win32Exception.Message);
            throw new ExternalException(message);
        }

        private static IntPtr OpenServiceManagerHandle()
        {
            IntPtr serviceManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (serviceManagerHandle == IntPtr.Zero)
            {
                string message = "サービスマネージャのオープンに失敗";
                Console.WriteLine(message);
                throw new ExternalException(message);
            }
            return serviceManagerHandle;
        }
        private static IntPtr OpenServiceHandle(ServiceController serviceController, IntPtr serviceManagerHandle)
        {
            var serviceHandle = OpenService(
                serviceManagerHandle, serviceController.ServiceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
            if (serviceHandle == IntPtr.Zero)
            {
                string message = "サービスのオープンに失敗";
                Console.WriteLine(message);
                throw new ExternalException(message);
            }
            return serviceHandle;
        }
        private static void ChangeServiceStartType(IntPtr serviceHandle, string startMode)
        {
            uint mode = 0;
            switch (startMode)
            {
                case AUTOMATIC: mode = 2; break;
                case MANUAL: mode = 3; break;
                case DISABLED: mode = 4; break;
            }
            bool ret = ChangeServiceConfig(
                serviceHandle,
                SERVICE_NO_CHANGE,
                mode,
                SERVICE_NO_CHANGE,
                null,
                null,
                IntPtr.Zero,
                null,
                null,
                null,
                null);
            if (!ret)
            {
                string message = "サービス開始タイプの変更に失敗";
                Console.WriteLine(message);
                ThrowLastWin32Error(message);
            }
        }
        private static void ChangeDelayedAutoStart(IntPtr hService, bool delayed)
        {
            SERVICE_DELAYED_AUTO_START_INFO info = new SERVICE_DELAYED_AUTO_START_INFO();
            info.fDelayedAutostart = delayed;
            IntPtr hInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SERVICE_DELAYED_AUTO_START_INFO)));
            Marshal.StructureToPtr(info, hInfo, true);
            bool ret = ChangeServiceConfig2(hService, SERVICE_CONFIG_DELAYED_AUTO_START_INFO, hInfo);
            Marshal.FreeHGlobal(hInfo);
            if (!ret)
            {
                string message = "サービス遅延自動設定の変更に失敗";
                Console.WriteLine(message);
                ThrowLastWin32Error(message);
            }
        }
        #endregion

        const string NONE = "None";
        const string AUTOMATIC = "Automatic";
        const string MANUAL = "Manual";
        const string DISABLED = "Disabled";
        const string DELAYED_AUTOMATIC = "DelayedAutomatic";
        
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }
        [Parameter]
        [ValidateSet(NONE, AUTOMATIC, MANUAL, DISABLED, DELAYED_AUTOMATIC)]
        public string StartupType { get; set; } = NONE;

        protected override void ProcessRecord()
        {
            ServiceController serviceController = ServiceControl.GetServiceController(Name);

            IntPtr serviceManagerHandle = OpenServiceManagerHandle();
            IntPtr serviceHandle = OpenServiceHandle(serviceController, serviceManagerHandle);

            try
            {
                if (StartupType == DELAYED_AUTOMATIC)
                {
                    ChangeServiceStartType(serviceHandle, AUTOMATIC);
                    ChangeDelayedAutoStart(serviceHandle, true);
                }
                else
                {
                    ChangeDelayedAutoStart(serviceHandle, false);
                    ChangeServiceStartType(serviceHandle, StartupType);
                }
            }
            catch { }
            if (serviceHandle != IntPtr.Zero)
            {
                CloseServiceHandle(serviceHandle);
            }
            if (serviceHandle != IntPtr.Zero)
            {
                CloseServiceHandle(serviceManagerHandle);
            }
        }
    }
}

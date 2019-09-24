using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "ScreenSaver")]
    public class SetScreenSaver : PSCmdlet
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref uint pvParam, SPIF fWinIni);

        public enum SPI : uint
        {
            //  スクリーンセーバー有効/無効
            SPI_GETSCREENSAVEACTIVE = 0x0010,
            SPI_SETSCREENSAVEACTIVE = 0x0011,
            //  スクリーンセーバー開始までの時間
            SPI_GETSCREENSAVETIMEOUT = 0x000E,
            SPI_SETSCREENSAVETIMEOUT = 0x000F,
            //  再開時にロック
            SPI_GETSCREENSAVESECURE = 0x0076,
            SPI_SETSCREENSAVESECURE = 0x0077
        }

        public enum SPIF
        {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02,
            SPIF_SENDWININICHANGE = 0x02
        }

        [Parameter(Position = 0)]
        public string Path { get; set; }
        [Parameter]
        public SwitchParameter None { get; set; }
        [Parameter]
        public int Timeout { get; set; }
        [Parameter]
        public bool? ScreenLock { get; set; }

        protected override void ProcessRecord()
        {
            Dictionary<string, string> defaultScreenSaver = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "3Dテキスト", @"C:\WINDOWS\system32\ssText3d.scr" },
                { "バブル", @"C:\WINDOWS\system32\Bubbles.scr" },
                { "ブランク", @"C:\WINDOWS\system32\scrnsave.scr" },
                { "ライン アート", @"C:\WINDOWS\system32\Mystify.scr" },
                { "リボン", @"C:\WINDOWS\system32\Ribbons.scr" },
                { "写真", @"C:\WINDOWS\system32\PhotoScreensaver.scr" }
            };

            uint tempRes = 0;

            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    //  スクリーンセーバーのパスを設定
                    if (defaultScreenSaver.ContainsKey(Path))
                    {
                        Path = defaultScreenSaver[Path];
                    }
                    regKey.SetValue("SCRNSAVE.EXE", Path, RegistryValueKind.String);
                }
                else if (None)
                {
                    //  スクリーンセーバー無し
                    if (regKey.GetValueNames().Any(x => x.Equals("SCRNSAVE.EXE", StringComparison.OrdinalIgnoreCase)))
                    {
                        regKey.DeleteValue("SCRNSAVE.EXE");
                    }
                }

                //  再開時にロック
                if (ScreenLock != null)
                {
                    int isLock = (bool)ScreenLock ? 1 : 0;
                    regKey.SetValue("ScreenSaverIsSecure", isLock.ToString(), RegistryValueKind.String);
                    SystemParametersInfo(SPI.SPI_SETSCREENSAVESECURE, (uint)isLock, ref tempRes, SPIF.SPIF_SENDCHANGE);
                }

                //  スクリーンセーバー開始までの時間
                if (Timeout > 0)
                {
                    regKey.SetValue("ScreenSaveTimeOut", Timeout.ToString(), RegistryValueKind.String);
                    SystemParametersInfo(SPI.SPI_SETSCREENSAVETIMEOUT, (uint)Timeout, ref tempRes, SPIF.SPIF_SENDCHANGE);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CLTools.Cmdlet.Display
{
    [Cmdlet(VerbsCommon.Set, "DesktopWallpaper")]
    public class SetDesktopWallpaper : PSCmdlet
    {
        //  壁紙画像を指定する場合に指定する。
        [Parameter, Alias("Path")]
        public string ImagePath { get; set; }

        //  壁紙を単色にする場合に指定する。
        [Parameter]
        public int[] Rgb { get; set; }
        [Parameter]
        public int[] Argb { get; set; }
        [Parameter, Alias("Color")]
        public string ColorName { get; set; }

        [DllImport("user32.dll")]
        static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        const int COLOR_DESKTOP = 1;
        const uint SPI_SETDESKWALLPAPER = 0x0014;
        const uint SPIF_UPDATEINIFILE = 0x0001;
        const uint SPIF_SENDWININICHANGE = 0x0002;

        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(ImagePath))
            {
                //  単色にする場合
                var color = Color.Black;
                string colorValue = "";
                if (Rgb != null)
                {
                    color = Color.FromArgb(Rgb[0], Rgb[1], Rgb[2]);
                    colorValue = $"{Rgb[0]} {Rgb[1]} {Rgb[2]}";
                }
                else if (Argb != null)
                {
                    color = Color.FromArgb(Argb[0], Argb[1], Argb[2], Argb[3]);
                    colorValue = $"{Argb[0]} {Argb[1]} {Argb[2]} {Argb[3]}";
                }
                else if (!string.IsNullOrEmpty(ColorName))
                {
                    var colorName = Enum.TryParse(ColorName, out KnownColor tempColor) ?
                        tempColor :
                        KnownColor.Black;
                    color = Color.FromName(ColorName);
                    colorValue = $"{color.R} {color.G} {color.B}";
                }

                int[] elements = { COLOR_DESKTOP };
                int[] colors = { ColorTranslator.ToWin32(color) };
                SetSysColors(elements.Length, elements, colors);

                //  レジストリ変更
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true))
                {
                    regKey.SetValue("Background", colorValue, RegistryValueKind.String);
                }
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    regKey.SetValue("WallPaper", "", RegistryValueKind.String);
                }
            }
            else if (File.Exists(ImagePath))
            {
                //  壁紙画像を指定する場合
                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    (uint)Encoding.GetEncoding("Shift_JIS").GetByteCount(ImagePath),
                    ImagePath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

                //  レジストリ変更
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true))
                {
                    regKey.SetValue("Background", "0 0 0", RegistryValueKind.String);
                }
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    regKey.SetValue("WallPaper", ImagePath, RegistryValueKind.String);
                }
            }
        }
    }
}

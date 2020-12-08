using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management;
namespace CLTools.Cmdlet.MemoryDump
{
    [Cmdlet(VerbsCommon.Get, "PagingFileSetting")]
    public class GetPagingFileSetting : PSCmdlet
    {
        [Parameter]
        public SwitchParameter PagingFile { get; set; }

        protected override void ProcessRecord()
        {
            var setting = new Class.MemoryDump.PagingFileSetting();
            setting.Load();

            //  参考
            //  https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-logicaldisk
            const int FIXED_HARD_DISK_MEDIA = 12;

            //  ローカルドライブを取得
            var localDrives = new List<string>();
            foreach (ManagementObject mo in new ManagementClass("Win32_LogicalDisk").
                GetInstances().
                OfType<ManagementObject>().
                Where(x => x["Name"] is string))
            {
                int mediaType = mo["MediaType"] != null ? int.Parse(mo["MediaType"].ToString()) : 0;
                if (mediaType == FIXED_HARD_DISK_MEDIA)
                {
                    localDrives.Add(mo["Name"].ToString());
                }
            }

            //  ページングファイル無しのローカルディスクの有無を確認。無ければPagingFileオブジェクトを追加
            foreach (string localDrive in localDrives)
            {
                string tempLocalDrive = localDrive.TrimEnd('\\');
                if (setting.PagingFiles == null)
                {
                    setting.PagingFiles = new List<Class.MemoryDump.PagingFile>();
                    setting.PagingFiles.Add(new Class.MemoryDump.PagingFile()
                    {
                        DriveName = tempLocalDrive
                    });
                }
                else if (!setting.PagingFiles.Any(x => x.DriveName.Equals(tempLocalDrive, StringComparison.OrdinalIgnoreCase)))
                {
                    setting.PagingFiles.Add(new Class.MemoryDump.PagingFile()
                    {
                        DriveName = tempLocalDrive
                    });
                }
            }
            setting.PagingFiles = setting.PagingFiles.OrderBy(x => x.DriveName).ToList();

            //  パラメータで「-PagingFile」を指定した場合、PagingFileSettingではなくPagingFileのリストを返す。
            if (this.PagingFile)
            {
                WriteObject(setting.PagingFiles);
            }
            else
            {
                WriteObject(setting);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace CLTools.Class.MemoryDump
{
    public class PagingFileSetting
    {
        //  参照キー
        //  HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management
        public bool AutoManage { get; set; } = true;
        public List<PagingFile> PagingFiles { get; set; }

        const string memoryManagement = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
        const string name_PAGINGFILES = "PagingFiles";

        public PagingFileSetting() { }

        /// <summary>
        /// 初期値に設定
        /// </summary>
        public void Init()
        {
            this.AutoManage = true;
        }

        /// <summary>
        /// 現在の設定を取得
        /// </summary>
        public void Load()
        {
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(memoryManagement, false))
            {
                string[] tempPagingFiles = regKey.GetValue(name_PAGINGFILES, "") as string[];
                this.AutoManage =
                    tempPagingFiles.Length == 1 && tempPagingFiles[0] == @"?:\pagefile.sys";
                if (!AutoManage)
                {
                    var pagingFileList = new List<PagingFile>();
                    foreach (string tempPagingFile in tempPagingFiles)
                    {
                        string[] fields = tempPagingFile.Split(' ');
                        if (fields.Length == 3 && fields[0].EndsWith("\\pagefile.sys"))
                        {
                            var pagingFile = new PagingFile()
                            {
                                DriveName = Path.GetPathRoot(fields[0]),
                                FilePath = fields[0],
                                MinimumSize = long.TryParse(fields[1], out long tempMin) ? tempMin : 0,
                                MaximumSize = long.TryParse(fields[2], out long tempMax) ? tempMax : 0
                            };
                            pagingFileList.Add(pagingFile);
                        }
                    }
                    this.PagingFiles = pagingFileList;
                }
            }
        }

        /// <summary>
        /// 設定を更新
        /// </summary>
        public void Save()
        {
            var setting = new PagingFileSetting();
            setting.Load();

            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(memoryManagement, true))
            {
                if (this.AutoManage)
                {
                    regKey.SetValue(name_PAGINGFILES, new string[1] { @"?:\pagefile.sys" }, RegistryValueKind.MultiString);
                }
                else
                {
                    string[] pageFiles = this.PagingFiles.
                        Where(x => !string.IsNullOrEmpty(x.FilePath)).
                        Select(x => string.Format("{0} {1} {2}", x.FilePath, x.MinimumSize, x.MaximumSize)).
                        ToArray();
                    regKey.SetValue(name_PAGINGFILES, pageFiles, RegistryValueKind.MultiString);
                }
            }
        }
    }
}

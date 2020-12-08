using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace CLTools.Cmdlet.MemoryDump
{
    [Cmdlet(VerbsCommon.Set, "PagingFileSetting")]
    public class SetPagingFileSetting : PSCmdlet
    {
        [Parameter]
        public bool? AutoManage { get; set; }
        [Parameter]
        public string[] PagengFile { get; set; }
        [Parameter(ValueFromPipeline = true)]
        public Class.MemoryDump.PagingFile[] PagingFileObject { get; set; }
        [Parameter(ValueFromPipeline = true)]
        public Class.MemoryDump.PagingFileSetting PagingFileSettingObject { get; set; }

        protected override void ProcessRecord()
        {
            if (PagingFileSettingObject == null)
            {
                if (AutoManage != null && (bool)AutoManage)
                {
                    //  自動管理が有効
                    var setting = new Class.MemoryDump.PagingFileSetting();
                    setting.Init();     //  デフォルトで自動管理が有効
                    setting.Save();
                }
                else
                {
                    //  自動管理無効
                    var setting = new Class.MemoryDump.PagingFileSetting();
                    setting.Init();
                    setting.AutoManage = false;

                    if (PagingFileObject == null)
                    {
                        //  文字列でPagingFileを指定 (「C:\pagefile.sys 1024 2048」等)
                        setting.PagingFiles = new List<Class.MemoryDump.PagingFile>();
                        foreach (string pagingFileLine in PagengFile)
                        {
                            string[] fields = pagingFileLine.Split();
                            setting.PagingFiles.Add(new Class.MemoryDump.PagingFile()
                            {
                                FilePath = fields[0],
                                MinimumSize = long.Parse(fields[1]),
                                MaximumSize = long.Parse(fields[2])
                            });
                        }
                    }
                    else
                    {
                        //  別コマンドレットで生成したPagingFileオブジェクトを指定
                        setting.PagingFiles = PagingFileObject.ToList();
                    }

                    setting.Save();
                }
            }
            else
            {
                //  PaingFileSettingオブジェクト全体を指定
                PagingFileSettingObject.Save();
            }
        }
    }
}

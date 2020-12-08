using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.IO;

namespace CLTools.Cmdlet.MemoryDump
{
    [Cmdlet(VerbsCommon.New, "PagingFile")]
    public class NewPagingFile : PSCmdlet
    {
        [Parameter, Alias("Path")]
        public string FilePath { get; set; }
        [Parameter(Position = 0)]
        public string DriveName { get; set; }
        [Parameter(Position = 1), Alias("Min")]
        public long? MinimumSize { get; set; } = 0;
        [Parameter(Position = 2), Alias("Max")]
        public long? MaximumSize { get; set; } = 0;

        protected override void ProcessRecord()
        {
            var pagingFile = new Class.MemoryDump.PagingFile();

            if (!string.IsNullOrEmpty(FilePath))
            {
                pagingFile.FilePath = FilePath;
                pagingFile.DriveName = Path.GetPathRoot(FilePath);
            }
            else if (!string.IsNullOrEmpty(DriveName) && Regex.IsMatch(DriveName, @"^[a-zA-Z](:|:\\)?$"))
            {
                pagingFile.FilePath = DriveName.Substring(0, 1) + ":\\pagefile.sys";
                pagingFile.DriveName = Path.GetPathRoot(pagingFile.FilePath);
            }

            if (MinimumSize != null)
            {
                pagingFile.MinimumSize = (long)MinimumSize;
            }
            if (MaximumSize != null)
            {
                pagingFile.MaximumSize = (long)MaximumSize;
            }

            WriteObject(pagingFile);
        }
    }
}

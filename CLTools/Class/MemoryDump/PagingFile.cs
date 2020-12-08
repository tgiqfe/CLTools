using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools.Class.MemoryDump
{
    public class PagingFile
    {
        private string _driveName = null;
        public string DriveName
        {
            get { return this._driveName; }
            set { this._driveName = value.ToUpper().TrimEnd('\\'); }
        }
        public PagingFileSizeType Type
        {
            get
            {
                if (string.IsNullOrEmpty(this.FilePath))
                {
                    return PagingFileSizeType.None;
                }
                else if (this.MinimumSize == 0 && this.MaximumSize == 0)
                {
                    return PagingFileSizeType.SystemManage;
                }
                else
                {
                    return PagingFileSizeType.Custom;
                }
            }
        }
        public string FilePath { get; set; }
        public long MinimumSize { get; set; } = 0;
        public long MaximumSize { get; set; } = 0;

        public PagingFile() { }
    }
}

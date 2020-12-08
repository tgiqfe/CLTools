using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace CLTools.Cmdlet.MemoryDump
{
    [Cmdlet(VerbsCommon.Set, "MemoryDumpSetting")]
    public class SetMemoryDumpSetting : PSCmdlet, IDynamicParameters
    {
        [Parameter(ValueFromPipeline = true)]
        public Class.MemoryDump.MemoryDumpSetting MemoryDump { get; set; }
        [Parameter, Alias("Path")]
        public string DumpFilePath { get; set; }
        [Parameter]
        public string MiniDumpDir { get; set; }
        [Parameter]
        public bool? WriteSystemLog { get; set; }
        [Parameter]
        public bool? AutoReboot { get; set; }
        [Parameter]
        public bool? OverwriteExistingFile { get; set; }
        [Parameter]
        public bool? DisableAutomaticDelation { get; set; }

        #region Dynamic Parameter

        private const string PARAM_DUMPTYPE = "DumpType";
        private RuntimeDefinedParameterDictionary _dictionary;
        private string[] _dumpTypes = null;

        public object GetDynamicParameters()
        {
            _dictionary = new RuntimeDefinedParameterDictionary();

            if (_dumpTypes == null)
            {
                var list = new List<string>();
                foreach (Class.MemoryDump.DumpType dumpType in Enum.GetValues(typeof(Class.MemoryDump.DumpType)))
                {
                    list.Add(dumpType.ToString());
                }
                _dumpTypes = list.ToArray();
            }

            Collection<Attribute> attribute = new Collection<Attribute>()
            {
                new ParameterAttribute(),
                new AliasAttribute("Type"),
                new ValidateSetAttribute(_dumpTypes)
            };

            RuntimeDefinedParameter rdp = new RuntimeDefinedParameter(PARAM_DUMPTYPE, typeof(string), attribute);
            _dictionary.Add(PARAM_DUMPTYPE, rdp);

            return _dictionary;
        }

        #endregion

        protected override void ProcessRecord()
        {
            Class.MemoryDump.MemoryDumpSetting setting = null;

            if (MemoryDump == null)
            {
                setting = new Class.MemoryDump.MemoryDumpSetting();
                setting.Load();

                string dumpTypeString = _dictionary[PARAM_DUMPTYPE].Value as string;
                if (!string.IsNullOrEmpty(dumpTypeString))
                {
                    setting.Type =
                        Enum.TryParse(dumpTypeString, out Class.MemoryDump.DumpType tempDumpType) ?
                            tempDumpType :
                            Class.MemoryDump.DumpType.None;
                }
                if (!string.IsNullOrEmpty(DumpFilePath))
                {
                    setting.DumpFilePath = DumpFilePath;
                }
                if (!string.IsNullOrEmpty(MiniDumpDir))
                {
                    setting.MiniDumpDir = MiniDumpDir;
                }
                if (WriteSystemLog != null)
                {
                    setting.WriteSystemLog = (bool)WriteSystemLog;
                }
                if (AutoReboot != null)
                {
                    setting.AutoReboot = (bool)AutoReboot;
                }
                if (OverwriteExistingFile != null)
                {
                    setting.OverwriteExistingFile = (bool)OverwriteExistingFile;
                }
                if (DisableAutomaticDelation != null)
                {
                    setting.DisableAutomaticDelation = (bool)DisableAutomaticDelation;
                }
            }
            else
            {
                setting = MemoryDump;
            }

            setting.Save();
        }
    }
}

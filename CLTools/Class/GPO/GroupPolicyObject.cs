using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLTools.Class.GPO
{
    public class GroupPolicyObject
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        
        public GroupPolicyObject() { }

        /// <summary>
        /// PolEntryインスタンスからGroupPolicyObjectインスタンスへの変換
        /// </summary>
        /// <param name="polEntry"></param>
        /// <returns></returns>
        public static GroupPolicyObject ConvertFromPolEntry(PolEntry polEntry)
        {
            string valueKindString = RegistryControl.REG_SZ;
            switch (polEntry.Type)
            {
                case PolEntryType.REG_SZ: valueKindString = RegistryControl.REG_SZ; break;
                case PolEntryType.REG_BINARY: valueKindString = RegistryControl.REG_BINARY; break;
                case PolEntryType.REG_DWORD: valueKindString = RegistryControl.REG_DWORD; break;
                case PolEntryType.REG_QWORD: valueKindString = RegistryControl.REG_QWORD; break;
                case PolEntryType.REG_MULTI_SZ: valueKindString = RegistryControl.REG_MULTI_SZ; break;
                case PolEntryType.REG_EXPAND_SZ: valueKindString = RegistryControl.REG_EXPAND_SZ; break;
                case PolEntryType.REG_NONE: valueKindString = RegistryControl.REG_NONE; break;
            }

            return new GroupPolicyObject()
            {
                Path = polEntry.Path,
                Name = polEntry.Name,
                Type = valueKindString,
                Value = RegistryControl.RegistryValueToString(polEntry.Value, valueKindString),
            };
        }

        /// <summary>
        /// GroupPolicyインスタンスからPolEntryインスタンスへの変換
        /// </summary>
        /// <returns></returns>
        public PolEntry ConvertToPolEntry()
        {
            PolEntryType entryType = PolEntryType.REG_SZ;
            switch (this.Type)
            {
                case RegistryControl.REG_SZ: entryType = PolEntryType.REG_SZ; break;
                case RegistryControl.REG_BINARY: entryType = PolEntryType.REG_BINARY; break;
                case RegistryControl.REG_DWORD: entryType = PolEntryType.REG_DWORD; break;
                case RegistryControl.REG_QWORD: entryType = PolEntryType.REG_QWORD; break;
                case RegistryControl.REG_MULTI_SZ: entryType = PolEntryType.REG_MULTI_SZ; break;
                case RegistryControl.REG_EXPAND_SZ: entryType = PolEntryType.REG_EXPAND_SZ; break;
                case RegistryControl.REG_NONE: entryType = PolEntryType.REG_NONE; break;
            }

            return new PolEntry()
            {
                Path = Path,
                Name = Name,
                Type = entryType,
                Value = RegistryControl.StringToRegistryValue(this.Value, this.Type),
            };
        }
    }
}

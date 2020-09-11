using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLTools.Class.GPO
{
    public class PolEntry : IComparable<PolEntry>
    {
        private List<byte> byteList;

        public string Path { get; set; }
        public string Name { get; set; }
        public PolEntryType Type { get; set; }
        public object Value
        {
            get
            {
                byte[] bytes = DataBytes.ToArray();
                switch (this.Type)
                {
                    case PolEntryType.REG_NONE:
                        return null;
                    case PolEntryType.REG_SZ:
                    case PolEntryType.REG_EXPAND_SZ:
                        return UnicodeEncoding.Unicode.GetString(bytes).Trim('\0');
                    case PolEntryType.REG_MULTI_SZ:
                        List<string> list = new List<string>();
                        StringBuilder sb = new StringBuilder(256);
                        for (int i = 0; i < (bytes.Length - 1); i += 2)
                        {
                            char[] curChar = UnicodeEncoding.Unicode.GetChars(bytes, i, 2);
                            if (curChar[0] == '\0')
                            {
                                if (sb.Length == 0) { break; }
                                list.Add(sb.ToString());
                                sb.Length = 0;
                            }
                            else
                            {
                                sb.Append(curChar[0]);
                            }
                        }
                        return list.ToArray();
                    case PolEntryType.REG_DWORD:
                        if (bytes.Length != 4) { throw new InvalidOperationException(); }
                        if (!BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
                        return BitConverter.ToUInt32(bytes, 0);
                    case PolEntryType.REG_DWORD_BIG_ENDIAN:
                        if (bytes.Length != 4) { throw new InvalidOperationException(); }
                        if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
                        return BitConverter.ToUInt32(bytes, 0);
                    case PolEntryType.REG_QWORD:
                        if (bytes.Length != 8) { throw new InvalidOperationException(); }
                        if (BitConverter.IsLittleEndian == false) { Array.Reverse(bytes); }
                        return BitConverter.ToUInt64(bytes, 0);
                    case PolEntryType.REG_BINARY:
                        return bytes;
                }
                return null;
            }
            set
            {
                switch (this.Type)
                {
                    case PolEntryType.REG_NONE:
                        break;
                    case PolEntryType.REG_SZ:
                    case PolEntryType.REG_EXPAND_SZ:
                        if (value == null) { value = string.Empty; }
                        this.byteList.Clear();
                        this.byteList.AddRange(UnicodeEncoding.Unicode.GetBytes(value.ToString() + "\0"));
                        break;
                    case PolEntryType.REG_MULTI_SZ:
                        this.byteList.Clear();
                        if (value != null)
                        {
                            string[] dataStrings = (string[])(((string[])value).Clone());
                            for (int i = 0; i < dataStrings.Length; i++)
                            {
                                if (i > 0) { this.byteList.AddRange(UnicodeEncoding.Unicode.GetBytes("\0")); }
                                if (dataStrings[i] != null)
                                {
                                    this.byteList.AddRange(UnicodeEncoding.Unicode.GetBytes(dataStrings[i]));
                                }
                            }
                        }
                        this.byteList.AddRange(UnicodeEncoding.Unicode.GetBytes("\0\0"));
                        break;
                    case PolEntryType.REG_DWORD:
                        this.byteList.Clear();
                        int dataInt = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
                        byte[] dArrBytes = BitConverter.GetBytes(dataInt);
                        if (!BitConverter.IsLittleEndian) { Array.Reverse(dArrBytes); }
                        this.byteList.AddRange(dArrBytes);
                        break;
                    case PolEntryType.REG_DWORD_BIG_ENDIAN:
                        this.byteList.Clear();
                        int dataInt_be = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
                        byte[] bArrBytes = BitConverter.GetBytes(dataInt_be);
                        if (BitConverter.IsLittleEndian) { Array.Reverse(bArrBytes); }
                        this.byteList.AddRange(bArrBytes);
                        break;
                    case PolEntryType.REG_QWORD:
                        this.byteList.Clear();
                        long dataLong = Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture);
                        byte[] qArrBytes = BitConverter.GetBytes(dataLong);
                        if (!BitConverter.IsLittleEndian) { Array.Reverse(qArrBytes); }
                        this.byteList.AddRange(qArrBytes);
                        break;
                    case PolEntryType.REG_BINARY:
                        this.byteList.Clear();
                        if (value != null)
                        {
                            byte[] dataBytes = (byte[])value;
                            this.byteList.AddRange(dataBytes);
                        }
                        break;
                }
            }
        }
        internal List<byte> DataBytes { get { return this.byteList; } }
        private int _index { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PolEntry()
        {
            this.byteList = new List<byte>();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~PolEntry()
        {
            this.byteList = null;
        }

        /// <summary>
        /// インデックス値をセット
        /// </summary>
        /// <returns></returns>
        public int GetIndex() { return this._index; }
        public void SetIndex(int val) { this._index = val; }

        /// <summary>
        /// 値比較用メソッド
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PolEntry other)
        {
            int result = string.Compare(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
            if (result != 0) { return result; }

            result = this._index.CompareTo(other.GetIndex());
            if (result != 0) { return result; }

            bool firstSpecial = this.Name.StartsWith("**", StringComparison.OrdinalIgnoreCase);
            bool secondSpecial = other.Name.StartsWith("**", StringComparison.OrdinalIgnoreCase);
            if (firstSpecial && !secondSpecial) { return -1; }
            if (secondSpecial && !firstSpecial) { return 1; }

            return String.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}

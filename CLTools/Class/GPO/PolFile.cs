using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLTools.Class.GPO
{
    public class PolFile
    {
        #region Private Parameters

        /// <summary>
        /// Polファイル読み込み時に使用。読み込み部分
        /// </summary>
        private enum PolEntryParseState { Start, Key, ValueName }

        private static readonly uint PolHeader = 0x50526567;
        private static readonly uint PolVersion = 0x01000000;

        private int _entryIndex = 0;

        #endregion
        #region Public Parameters

        /*
        /// <summary>
        /// Polファイルへのパス
        /// </summary>
        public string FileName { get; set; }
        */

        /*
        /// <summary>
        /// コンピュータの構成/ユーザーの構成
        /// </summary>
        public string ConfigurationType { get; set; }
        */

        /// <summary>
        /// レジストリエントリ。重複排除の為にDictionary型
        /// </summary>
        public Dictionary<string, PolEntry> Entries { get; set; }
        
        #endregion
        #region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PolFile()
        {
            this.Entries = new Dictionary<string, PolEntry>(StringComparer.OrdinalIgnoreCase);
        }
        /*
        public PolFile(string fileName)
        {
            this.Entries = new Dictionary<string, PolEntry>(StringComparer.OrdinalIgnoreCase);
            this.FileName = fileName;
            this.Load(fileName);
        }
        */

        #endregion
        #region Static Methods

        /// <summary>
        /// PolFileインスタンスを生成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PolFile Create(string fileName)
        {
            /*
            return new PolFile(fileName);
            */
            var pol = new PolFile();
            pol.Load(fileName);
            return pol;
        }

        #endregion

        /// <summary>
        /// PolEntryを直接セット
        /// </summary>
        /// <param name="pe"></param>
        public void SetValue(PolEntry pe)
        {
            pe.SetIndex(_entryIndex++);
            this.Entries[pe.Path + "\\" + pe.Name] = pe;
        }

        /// <summary>
        /// Path,Name,Valut,Typeから値をセット
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="type"></param>
        public void AddValue(string path, string name, object val, PolEntryType type)
        {
            PolEntry pe = new PolEntry()
            {
                Path = path,
                Name = name,
                Type = type,
                Value = val
            };
            SetValue(pe);
        }

        /// <summary>
        /// Polファイルをロード。ファイル名を指定しない。再ロード用
        /// </summary>
        public void Load()
        {
            Load(null);
        }

        /// <summary>
        /// Polファイルをロード
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            //if (!string.IsNullOrEmpty(file)) { this.FileName = file; }
            if (!File.Exists(fileName)) { return; }
            byte[] bytes;
            int nBytes = 0;

            this.Entries.Clear();
            this._entryIndex = 0;

            //using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fs.Length];
                int nBytesToRead = (int)fs.Length;
                while (nBytesToRead > 0)
                {
                    int n = fs.Read(bytes, nBytes, nBytesToRead);
                    if (n == 0) break;
                    nBytes += n;
                    nBytesToRead -= n;
                }
            }

            if (nBytes < 8) { throw new FileFormatException(); }

            int header = (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
            int version = (bytes[4] << 24) | (bytes[5] << 16) | (bytes[6] << 8) | bytes[7];
            if (header != PolFile.PolHeader || version != PolFile.PolVersion) { throw new FileFormatException(); }

            var parseState = PolEntryParseState.Start;
            int i = 8;

            var keyName = new StringBuilder(50);
            var valueName = new StringBuilder(50);
            uint type = 0;
            int size = 0;

            while (i < (nBytes - 1))
            {
                char[] curChar = UnicodeEncoding.Unicode.GetChars(bytes, i, 2);

                switch (parseState)
                {
                    case PolEntryParseState.Start:
                        if (curChar[0] != '[') { throw new FileFormatException(); }
                        i += 2;
                        parseState = PolEntryParseState.Key;
                        continue;
                    case PolEntryParseState.Key:
                        if (curChar[0] == '\0')
                        {
                            if (i > (nBytes - 4)) { throw new FileFormatException(); }
                            curChar = UnicodeEncoding.Unicode.GetChars(bytes, i + 2, 2);
                            if (curChar[0] != ';') { throw new FileFormatException(); }

                            i += 4;
                            parseState = PolEntryParseState.ValueName;
                        }
                        else
                        {
                            keyName.Append(curChar[0]);
                            i += 2;
                        }
                        continue;
                    case PolEntryParseState.ValueName:
                        if (curChar[0] == '\0')
                        {
                            if (i > (nBytes - 16)) { throw new FileFormatException(); }
                            curChar = UnicodeEncoding.Unicode.GetChars(bytes, i + 2, 2);
                            if (curChar[0] != ';') { throw new FileFormatException(); }

                            type = (uint)(bytes[i + 7] << 24 | bytes[i + 6] << 16 | bytes[i + 5] << 8 | bytes[i + 4]);
                            if (Enum.IsDefined(typeof(PolEntryType), type) == false) { throw new FileFormatException(); }

                            curChar = UnicodeEncoding.Unicode.GetChars(bytes, i + 8, 2);
                            if (curChar[0] != ';') { throw new FileFormatException(); }

                            size = bytes[i + 13] << 24 | bytes[i + 12] << 16 | bytes[i + 11] << 8 | bytes[i + 10];
                            if ((size > 0xFFFF) || (size < 0)) { throw new FileFormatException(); }

                            curChar = UnicodeEncoding.Unicode.GetChars(bytes, i + 14, 2);
                            if (curChar[0] != ';') { throw new FileFormatException(); }

                            i += 16;

                            if (i > (nBytes - (size + 2))) { throw new FileFormatException(); }
                            curChar = UnicodeEncoding.Unicode.GetChars(bytes, i + size, 2);
                            if (curChar[0] != ']') { throw new FileFormatException(); }

                            PolEntry pe = new PolEntry();
                            pe.Path = keyName.ToString();
                            pe.Name = valueName.ToString();
                            pe.Type = (PolEntryType)type;

                            for (int j = 0; j < size; j++)
                            {
                                pe.DataBytes.Add(bytes[i + j]);
                            }

                            this.SetValue(pe);

                            i += size + 2;

                            keyName.Length = 0;
                            valueName.Length = 0;
                            parseState = PolEntryParseState.Start;
                        }
                        else
                        {
                            valueName.Append(curChar[0]);
                            i += 2;
                        }
                        continue;
                    default:
                        throw new Exception("Unreachable code");
                }
            }
        }

        /// <summary>
        /// Polファイルを保存。Load時に登録したファイルに保存
        /// </summary>
        public void Save()
        {
            Save(null);
        }

        /// <summary>
        /// Polファイルを保存
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            //if (!string.IsNullOrEmpty(file)) { this.FileName = file; }

            //using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(new byte[] { 0x50, 0x52, 0x65, 0x67, 0x01, 0x00, 0x00, 0x00 }, 0, 8);
                byte[] openBracket = UnicodeEncoding.Unicode.GetBytes("[");
                byte[] closeBracket = UnicodeEncoding.Unicode.GetBytes("]");
                byte[] semicolon = UnicodeEncoding.Unicode.GetBytes(";");
                byte[] nullChar = new byte[] { 0, 0 };

                byte[] bytes;

                List<PolEntry> EntryList = new List<PolEntry>(Entries.Values);
                //EntryList.Sort();

                foreach (PolEntry pe in EntryList)
                {
                    fs.Write(openBracket, 0, 2);
                    bytes = UnicodeEncoding.Unicode.GetBytes(pe.Path);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Write(nullChar, 0, 2);

                    fs.Write(semicolon, 0, 2);
                    bytes = UnicodeEncoding.Unicode.GetBytes(pe.Name);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Write(nullChar, 0, 2);

                    fs.Write(semicolon, 0, 2);
                    bytes = BitConverter.GetBytes((uint)pe.Type);
                    if (BitConverter.IsLittleEndian == false) { Array.Reverse(bytes); }
                    fs.Write(bytes, 0, 4);

                    fs.Write(semicolon, 0, 2);
                    byte[] data = pe.DataBytes.ToArray();
                    bytes = BitConverter.GetBytes((uint)data.Length);
                    if (BitConverter.IsLittleEndian == false) { Array.Reverse(bytes); }
                    fs.Write(bytes, 0, 4);

                    fs.Write(semicolon, 0, 2);
                    fs.Write(data, 0, data.Length);
                    fs.Write(closeBracket, 0, 2);
                }
            }
        }       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Hnx8.ReadJEnc;
using System.Text.RegularExpressions;

namespace CLTools.Serialize
{
    public class IniFile
    {
        public class Section
        {
            public string Name { get; set; }
            public Dictionary<string, string> Entries { get; set; }
            public bool PriorityFirst { get; set; }

            public Section()
            {
                this.Entries = new Dictionary<string, string>();
                this.Name = "";
            }
            public Section(string name)
            {
                this.Entries = new Dictionary<string, string>();
                this.Name = name;
            }

            /// <summary>
            /// Iniパラメータ行をセット
            /// </summary>
            /// <param name="line"></param>
            public void SetEntry(string line)
            {
                if (line.Contains("="))
                {
                    SetEntryWithoutCheckEqual(line);
                }
            }

            /// <summary>
            /// Iniパラメータ行をセット
            /// 「=」の有無チェックは行わない。
            /// </summary>
            /// <param name="line"></param>
            public void SetEntryWithoutCheckEqual(string line)
            {
                string paramName = line.Substring(0, line.IndexOf("=")).Trim();
                string paramValue = line.Substring(line.IndexOf("=") + 1).Trim();
                if (PriorityFirst)
                {
                    if (this.Entries.ContainsKey(paramName))
                    {
                        return;
                    }
                }
                this.Entries[paramName] = paramValue;
            }
        }

        public List<Section> SectionList { get; set; }
        public string Encoding { get; set; }
        public bool WithBOM { get; set; }

        /// <summary>
        /// BOM有りUTF文字コード名
        /// </summary>
        public static CharCode[] UTFCodes = new CharCode[]
        {
            CharCode.UTF8, CharCode.UTF32, CharCode.UTF32B, CharCode.UTF16, CharCode.UTF16B
        };

        public IniFile()
        {
            this.SectionList = new List<Section>();
            this.Encoding = System.Text.Encoding.GetEncoding("Shift_JIS").WebName;
        }
        public IniFile(string iniFile) : this()
        {
            this.Load(iniFile);
        }

        /// <summary>
        /// 文字コードを取得
        /// </summary>
        /// <returns></returns>
        public Encoding GetEncoding()
        {
            switch (Encoding)
            {
                case "utf-8":
                    return WithBOM ? new UTF8Encoding(true) : new UTF8Encoding(false);
                case "utf-16":
                    return WithBOM ? new UnicodeEncoding(false, true) : new UnicodeEncoding(false, false);
                case "utf-16BE":
                    return WithBOM ? new UnicodeEncoding(true, true) : new UnicodeEncoding(true, false);
                case "utf-32":
                    return WithBOM ? new UTF32Encoding(false, true) : new UTF32Encoding(false, false);
                case "utf-32BE":
                    return WithBOM ? new UTF32Encoding(true, true) : new UTF32Encoding(true, false);
                default:
                    return System.Text.Encoding.GetEncoding(Encoding);
            }
        }

        /// <summary>
        /// Iniファイル読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        public void Load(string iniFile)
        {
            FileInfo fi = new FileInfo(iniFile);
            if (fi.Length > 0)
            {
                using (FileReader fr = new FileReader(fi))
                {
                    CharCode code = fr.Read(fi);
                    this.Encoding = code.GetEncoding().WebName;
                    this.WithBOM = UTFCodes.Contains(code);

                    ReadIniSection(fr.Text);
                }
            }
            else
            {
                this.Encoding = System.Text.Encoding.GetEncoding("Shift_JIS").WebName;
            }
        }

        /// <summary>
        /// Iniファイルのテキストからセクション読み込み
        /// </summary>
        /// <param name="text"></param>
        public void ReadIniSection(string text)
        {
            using (var sr = new StringReader(text))
            {
                Section section = null;

                Regex reg_section = new Regex(@"(?<=\s*\[).+(?=\])");

                string readLine = "";
                while ((readLine = sr.ReadLine()) != null)
                {
                    if (reg_section.IsMatch(readLine))
                    {
                        section = new Section(reg_section.Match(readLine).Value);
                        SectionList.Add(section);
                        continue;
                    }
                    else if (readLine.Contains("="))
                    {
                        if (section == null)
                        {
                            section = new Section();
                            SectionList.Add(section);
                        }
                        section.SetEntryWithoutCheckEqual(readLine);
                    }
                }
            }
        }

        /// <summary>
        /// Iniファイルを保存
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            using (var sw = new StreamWriter(fileName, false, this.GetEncoding()))
            {
                Section defSection = SectionList.FirstOrDefault(x => x.Name == "");
                if (defSection != null)
                {
                    foreach (KeyValuePair<string, string> pair in defSection.Entries)
                    {
                        sw.WriteLine("{0} = {1}", pair.Key, pair.Value);
                    }
                    sw.WriteLine();
                }
                foreach (Section section in SectionList.Where(x => x.Name != ""))
                {
                    sw.WriteLine("[{0}]", section.Name);
                    foreach (KeyValuePair<string, string> pair in section.Entries)
                    {
                        sw.WriteLine("{0} = {1}", pair.Key, pair.Value);
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}

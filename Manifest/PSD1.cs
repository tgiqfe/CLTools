using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace Manifest
{
    class PSD1
    {
        public static void Create(string dllFile, string cmdletDir, string outputFile)
        {
            //  CmdletsToExportの為のコマンドレットの一覧を取得
            List<string> CmdletsToExport = new List<string>();
            foreach (string csFile in Directory.GetFiles(cmdletDir, "*.cs", SearchOption.AllDirectories))
            {
                using (StreamReader sr = new StreamReader(csFile, Encoding.UTF8))
                {
                    string readLine = "";
                    while ((readLine = sr.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(readLine, @"^\s*\[Cmdlet\(Verbs"))
                        {
                            string cmdPre = readLine.Substring(
                                readLine.IndexOf(".") + 1, readLine.IndexOf(",") - readLine.IndexOf(".") - 1);
                            string cmdSuf = readLine.Substring(
                                readLine.IndexOf("\"") + 1, readLine.LastIndexOf("\"") - readLine.IndexOf("\"") - 1);
                            CmdletsToExport.Add(cmdPre + "-" + cmdSuf);
                        }
                    }
                }
            }

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(dllFile);

            string RootModule = "CLTools.dll";
            string ModuleVersion = fvi.FileVersion;
            string Guid = "AD6A4B7D-EC8A-430F-A04B-E2169FC2E660";   //  GUIDは固定で
            string Author = "q";
            string CompanyName = "q";
            string Copyright = fvi.LegalCopyright;
            string Description = "Client Tool";

            string manifestString = string.Format(@"@{{
RootModule = ""{0}""
ModuleVersion = ""{1}""
GUID = ""{2}""
Author = ""{3}""
CompanyName = ""{4}""
Copyright = ""{5}""
Description = ""{6}""
CmdletsToExport = @(
""{7}""
)
}}",
RootModule, ModuleVersion, Guid, Author, CompanyName, Copyright, Description,
string.Join("\", \"", CmdletsToExport)
);
            Console.WriteLine(outputFile);
            using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                sw.WriteLine(manifestString);
            }
        }
    }
}

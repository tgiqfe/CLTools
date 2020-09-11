using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace CLTools.Class.GPO
{
    internal class RegistryControl
    {
        //  レジストリ値の種類
        public const string REG_SZ = "REG_SZ";
        public const string REG_BINARY = "REG_BINARY";
        public const string REG_DWORD = "REG_DWORD";
        public const string REG_QWORD = "REG_QWORD";
        public const string REG_MULTI_SZ = "REG_MULTI_SZ";
        public const string REG_EXPAND_SZ = "REG_EXPAND_SZ";
        public const string REG_NONE = "REG_NONE";

        //  レジストリルートキー名
        const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
        const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
        const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        const string HKEY_USERS = "HKEY_USERS";
        const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
        const string HKCR = "HKCR";
        const string HKCU = "HKCU";
        const string HKLM = "HKLM";
        const string HKU = "HKU";
        const string HKCC = "HKCC";
        const string HKCR_ = "HKCR:";
        const string HKCU_ = "HKCU:";
        const string HKLM_ = "HKLM:";
        const string HKU_ = "HKU:";
        const string HKCC_ = "HKCC:";

        /// <summary>
        /// レジストリルートパスを取得
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static RegistryKey GetRootkey(string rootPath)
        {
            if (rootPath.Contains("\\"))
            {
                rootPath = rootPath.Substring(0, rootPath.IndexOf("\\"));
            }
            switch (rootPath)
            {
                case HKCR:
                case HKCR_:
                case HKEY_CLASSES_ROOT:
                    return Registry.ClassesRoot;
                case HKCU:
                case HKCU_:
                case HKEY_CURRENT_USER:
                    return Registry.CurrentUser;
                case HKLM:
                case HKLM_:
                case HKEY_LOCAL_MACHINE:
                    return Registry.LocalMachine;
                case HKU:
                case HKU_:
                case HKEY_USERS:
                    return Registry.Users;
                case HKCC:
                case HKCC_:
                case HKEY_CURRENT_CONFIG:
                    return Registry.CurrentConfig;
            }
            return null;
        }

        /// <summary>
        /// 文字列のレジストリパスからRegistryKeyインスタンスを生成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCreate"></param>
        /// <param name="writable"></param>
        /// <returns></returns>
        public static RegistryKey GetRegistryKey(string path, bool isCreate, bool writable)
        {
            string keyName = path.Substring(path.IndexOf("\\") + 1);

            return isCreate ?
                GetRootkey(path).CreateSubKey(keyName, writable) :
                GetRootkey(path).OpenSubKey(keyName, writable);
        }

        /// <summary>
        /// レジストリ値の種類の変換
        /// </summary>
        /// <param name="valueKindString">レジストリ種類の文字列</param>
        /// <returns>RegistryValueKind</returns>
        public static RegistryValueKind StringToValueKind(string valueKindString)
        {
            switch (valueKindString.ToUpper())
            {
                case REG_SZ: return RegistryValueKind.String;
                case REG_BINARY: return RegistryValueKind.Binary;
                case REG_DWORD: return RegistryValueKind.DWord;
                case REG_QWORD: return RegistryValueKind.QWord;
                case REG_MULTI_SZ: return RegistryValueKind.MultiString;
                case REG_EXPAND_SZ: return RegistryValueKind.ExpandString;
                case REG_NONE: return RegistryValueKind.None;
            }
            return RegistryValueKind.String;
        }

        /// <summary>
        /// レジストリ値の種類の変換
        /// </summary>
        /// <param name="valueKind">RegistryValueKind</param>
        /// <returns>レジストリ種類の文字列</returns>
        public static string ValueKindToString(RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String: return REG_SZ;
                case RegistryValueKind.Binary: return REG_BINARY;
                case RegistryValueKind.DWord: return REG_DWORD;
                case RegistryValueKind.QWord: return REG_QWORD;
                case RegistryValueKind.MultiString: return REG_MULTI_SZ;
                case RegistryValueKind.ExpandString: return REG_EXPAND_SZ;
                case RegistryValueKind.None: return REG_NONE;
                default: return REG_SZ;
            }
        }

        /// <summary>
        /// object型のレジストリ値を文字列に変換
        /// </summary>
        /// <param name="valueObject"></param>
        /// <param name="valueKind"></param>
        /// <returns></returns>
        public static string RegistryValueToString(object valueObject, RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                    return valueObject as string;
                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                    return valueObject.ToString();
                case RegistryValueKind.ExpandString:
                    return valueObject as string;
                case RegistryValueKind.Binary:
                    return BitConverter.ToString(valueObject as byte[]).Replace("-", "").ToUpper();
                case RegistryValueKind.MultiString:
                    return string.Join("\\0", valueObject as string[]);
                case RegistryValueKind.None:
                default:
                    return null;
            }
        }
        public static string RegistryValueToString(object valueObject, string valueKindString)
        {
            return RegistryValueToString(valueObject, StringToValueKind(valueKindString));
        }

        /// <summary>
        /// 文字列からレジストリ値用のオブジェクトに変換
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="valueKind"></param>
        /// <returns></returns>
        public static object StringToRegistryValue(string valueString, RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                    return valueString;
                case RegistryValueKind.Binary:
                    if (Regex.IsMatch(valueString, @"^[0-9a-fA-F]+$"))
                    {
                        List<byte> tempBytes = new List<byte>();
                        for (int i = 0; i < valueString.Length / 2; i++)
                        {
                            tempBytes.Add(Convert.ToByte(valueString.Substring(i * 2, 2), 16));
                        }
                        return tempBytes.ToArray();
                    }
                    return new byte[0] { };
                case RegistryValueKind.DWord:
                    return int.Parse(valueString);
                case RegistryValueKind.QWord:
                    return long.Parse(valueString);
                case RegistryValueKind.MultiString:
                    return Regex.Split(valueString, "\\\\0");
                case RegistryValueKind.ExpandString:
                    return valueString;
                case RegistryValueKind.None:
                    return new byte[2] { 0, 0 };
                default:
                    return null;
            }
        }
        public static object StringToRegistryValue(string valueString, string valueKindString)
        {
            return StringToRegistryValue(valueString, StringToValueKind(valueKindString));
        }
    }
}

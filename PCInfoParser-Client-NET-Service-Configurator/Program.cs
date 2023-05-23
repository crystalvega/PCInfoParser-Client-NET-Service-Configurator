using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCInfoParser_Client_NET_Service_Configurator
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string genPath = GetDirectory("General.txt");
            string logFile = GetDirectory("PCInfoParser-Client-NET-Service.InstallLog");
            string iniFile = GetDirectory("PCInfoParser-Client.ini");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Configurator(iniFile, genPath, logFile));
        }

        public static string GetDirectory(string filename)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            string Directory = Path.Combine(path, filename);
            return Directory;
        }
    }
    public class OldConfig
    {
        private string[] val = new string[5] { "", "", "", "", "" };
        private readonly string filename = "";
        public OldConfig(string filenameset)
        {
            this.filename = filenameset;
            if (File.Exists(this.filename))
            {
                Load();
            }
        }

        public void Load()
        {
            int i = 0;
            using (var reader = new StreamReader(this.filename, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    val[i] = line.Trim();
                    i++;
                }
            }
        }

        public string[] GetValues()
        {
            return val;
        }
    }
    public class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
        private readonly string fileName;

        public IniFile(string fileName, string[] oldValues)
        {
            this.fileName = fileName;

            if (File.Exists(fileName))
            {
                Load();
            }
            else if (oldValues.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                SetValue("User", "ФИО", oldValues[0]);
                SetValue("User", "Кабинет", oldValues[1]);
                SetValue("User", "Организация", oldValues[4]);
                SetValue("User", "ID", "NeedToGet");
                SetValue("Server", "IP", "");
                SetValue("Server", "Port", "");
                SetValue("Server", "Password", "");
                SetValue("App", "Autosend", "7");
                Save();
                Load();
            }
            else
            {
                SetValue("User", "ФИО", "");
                SetValue("User", "Кабинет", "");
                SetValue("User", "Организация", "");
                SetValue("User", "ID", "NeedToGet");
                SetValue("Server", "IP", "");
                SetValue("Server", "Port", "");
                SetValue("Server", "Password", "");
                SetValue("App", "Autosend", "7");
                Save();
                Load();
            }
        }

        public string GetValue(string section, string key)
        {
            if (data.TryGetValue(section, out Dictionary<string, string> sectionData))
            {
                if (sectionData.TryGetValue(key, out string value))
                {
                    return value;
                }
            }

            return null;
        }

        public void SetValue(string section, string key, string value)
        {
            if (!data.TryGetValue(section, out Dictionary<string, string> sectionData))
            {
                sectionData = new Dictionary<string, string>();
                data[section] = sectionData;
            }

            sectionData[key] = value;
        }

        public void Load()
        {
            data.Clear();

            string currentSection = null;

            foreach (string line in File.ReadAllLines(fileName))
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!data.ContainsKey(currentSection))
                    {
                        data[currentSection] = new Dictionary<string, string>();
                    }
                }
                else if (!string.IsNullOrEmpty(trimmedLine))
                {
                    string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                    if (parts.Length > 1)
                    {
                        string currentKey = parts[0].Trim();
                        string currentValue = parts[1].Trim();
                        if (data.TryGetValue(currentSection, out Dictionary<string, string> sectionData))
                        {
                            sectionData[currentKey] = currentValue;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, string>> section in data)
            {
                lines.Add("[" + section.Key + "]");
                foreach (KeyValuePair<string, string> keyValuePair in section.Value)
                {
                    lines.Add(keyValuePair.Key + "=" + keyValuePair.Value);
                }
                lines.Add("");
            }

            File.WriteAllLines(fileName, lines.ToArray());
        }
    }
}

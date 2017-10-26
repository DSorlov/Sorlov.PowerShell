using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sorlov.PowerShell.Lib.Core
{
    public static class LibraryConfiguration
    {
        private static Dictionary<string, object> settings;

        public static bool IsTrue(string flag)
        {
            if (settings == null)
                settings = GetAllVariables();

            if (settings.ContainsKey(flag))
                if ((bool)settings[flag])
                    return true;

            return false;
        }

        public static bool IsNotNull(string flag)
        {
            if (settings == null)
                settings = GetAllVariables();

            if (settings.ContainsKey(flag))
                return true;

            return false;
        }


        public static T Setting<T>(string flag)
        {
            if (settings == null)
                settings = GetAllVariables();

            return settings.ContainsKey(flag) ? (T) settings[flag] : default(T);
        }

        private static string GetRawSection(string input)
        {
            Match match = Regex.Match(input, @"PrivateData.*?=.*?\@.*?\{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
                return match.Groups[1].Value;
            else
                return string.Empty;

        }

        private static Dictionary<string, object> GetAllVariables()
        {
            string localDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string privateData = System.IO.Path.Combine(localDir, "Sorlov.PowerShell.psd1");

            string input = GetRawSection(System.IO.File.ReadAllText(privateData, Encoding.Unicode));

            Dictionary<string, object> result = new Dictionary<string, object>();
            Regex ptrn = new Regex(@"(\w*)=(.*?);", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in ptrn.Matches(input))
            {
                string datapart = match.Groups[2].Value.Trim();

                if (datapart.StartsWith("\"") || datapart.StartsWith("'"))
                {
                    result.Add(match.Groups[1].Value.Trim(), datapart.Substring(1, datapart.Length - 2));
                }
                else if (datapart.ToLower() == "$true")
                {
                    result.Add(match.Groups[1].Value.Trim(), true);
                }
                else if (datapart.ToLower() == "$false")
                {
                    result.Add(match.Groups[1].Value.Trim(), false);
                }
                else if (datapart.StartsWith("@("))
                {
                    List<string> arrayItems = new List<string>();

                    datapart = datapart.Substring(2, datapart.Length - 3);
                    string[] dataArray = datapart.Split(',');
                    foreach (string arrayPart in dataArray)
                    {
                        if (arrayPart != string.Empty) arrayItems.Add(arrayPart.Substring(1, arrayPart.Length - 2));
                    }
                    result.Add(match.Groups[1].Value.Trim(), arrayItems);
                }


                match.NextMatch();
            }

            return result;
        }


    }
}

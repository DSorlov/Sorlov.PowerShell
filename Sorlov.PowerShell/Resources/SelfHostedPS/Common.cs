using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace Sorlov.SelfHostedPS
{
    public static class Common
    {
        public static void ExtractResource(string resourceName, string destinationFile)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (!System.IO.File.Exists(destinationFile))
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (Stream csStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (StreamReader streamReader = new StreamReader(csStream))
                        {
                            using (Stream outputStream = System.IO.File.Create(destinationFile))
                            {
                                using (StreamWriter streamWriter = new StreamWriter(outputStream))
                                {
                                    streamWriter.Write(streamReader.ReadToEnd());
                                }
                            }
                        }
                    }
                }
            }
        }


        public static void CleanUp()
        {
            Directory.Delete(Common.GetDataDirectory(),true);
        }

        public static List<string> ProcessManifest(InitialSessionState sessionState)
        {
            List<string> modulesToLoad = new List<string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> manifestData;
            string dataDirectory = Common.GetDataDirectory();

            using (Stream stream = assembly.GetManifestResourceStream("Manifest"))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    manifestData = new List<string>(streamReader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                }
            }

            foreach (string mainfestRow in manifestData)
            {
                string[] instructionSet = mainfestRow.Split('|');

                switch (instructionSet[0])
                {
                    case "Module":
                        string modulePath = System.IO.Path.Combine(dataDirectory, instructionSet[2]);
                        ExtractResource(instructionSet[1], modulePath);
                        modulesToLoad.Add(modulePath);
                        break;
                    case "Copy":
                        string filePath = System.IO.Path.Combine(dataDirectory, instructionSet[2]);
                        ExtractResource(instructionSet[1], filePath);
                        break;
                }

            }

            return modulesToLoad;
        }


        public static string GetDataDirectory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string appDataFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "SelfHostedPS");
            string resDataFolder = System.IO.Path.Combine(appDataFolder, System.IO.Path.GetFileNameWithoutExtension(assembly.Location));

            if (!System.IO.Directory.Exists(appDataFolder))
                System.IO.Directory.CreateDirectory(appDataFolder);

            if (!System.IO.Directory.Exists(resDataFolder))
                System.IO.Directory.CreateDirectory(resDataFolder);

            return resDataFolder;
        }

        public static string GetScript(string resourceName)
        {            
            string script = string.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (Stream csStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (StreamReader streamReader = new StreamReader(csStream))
                    {
                        script = streamReader.ReadToEnd();
                    }
                }
            }
            return script;
        }

    }
}

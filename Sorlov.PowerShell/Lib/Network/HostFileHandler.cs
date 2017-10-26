using Sorlov.PowerShell.Dto.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Sorlov.PowerShell.Lib.Network
{
    public static class HostFileHandler
    {

        public static void PersistHostEntryCollection(string hostFile, HostEntryCollection hosts)
        {
            StringBuilder result = new StringBuilder();

            foreach (HostEntry host in hosts)
            {
                string hostNames = string.Empty;
                foreach(string hostName in host.Hostnames)
                    hostNames = string.Format("{0} {1}", hostNames, hostName);

                string comment = (host.Comment == null) ? string.Empty : string.Format("#{0}", host.Comment.Trim());

                result.AppendFormat("{0} {1} {2}{3}", host.IPAddress.ToString(), hostNames.Trim(), comment, Environment.NewLine);
            }

            StreamWriter writer = new StreamWriter(hostFile, false);
            writer.Write(result.ToString());
            writer.Close();
        }

        public static HostEntryCollection GetHostEntryColletion(string hostFile)
        {
            HostEntryCollection result = new HostEntryCollection();

            if (hostFile == string.Empty)
                hostFile = Path.Combine(Environment.SystemDirectory,"drivers\\etc\\hosts");

            string[] fileContent = File.ReadAllLines(hostFile);

            foreach (string fileRow in fileContent)
            {
                string dataToProcess = fileRow.Trim();

                if (!dataToProcess.StartsWith("#"))
                {
                    if (dataToProcess.Contains(' '))
                    {
                        string[] fileValues = dataToProcess.Split(new char[] { ' ' }, 2);
                        
                        // Get the IPAddress
                        IPAddress ipAddresser = IPAddress.Parse(fileValues[0].Trim());

                        HostEntry hostEntry = result[ipAddresser];
                        if (hostEntry == null)
                        {
                            hostEntry = new HostEntry();
                            hostEntry.IPAddress = ipAddresser;
                            result.Add(hostEntry);
                        }

                        // Get the Comment & hostnames
                        string[] commentValue = fileValues[1].Split('#');
                        string hostnameString = string.Empty;
                        if (commentValue.Length > 1)
                        {
                            string commentString = commentValue[1].Trim();
                            if (commentString.Length > 0)
                            {
                                hostEntry.Comment = (commentValue[1] + " " + hostEntry.Comment).Trim();
                                hostnameString = fileValues[1].Replace(commentValue[1], "").Replace("#", "").Trim();
                            }
                            else
                            {
                                hostnameString = fileValues[1].Replace("#", "").Trim();
                            }
                        }
                        else
                        {
                            hostnameString = fileValues[1].Trim();
                        }

                        List<string> hostNames = new List<string>();
                        string[] fileHosts = hostnameString.Split(' ');

                        foreach(string fileHost in fileHosts)
                        {
                            string addFile = fileHost.Trim();
                            if (addFile!=null && addFile!=string.Empty)
                                if (!hostEntry.ContainsHostname(addFile)) hostEntry.AddHostname(addFile);
                        }
                    }
                }
            }

            return result;
        }
    }
}

using Sorlov.PowerShell.SelfHosted.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    class ProxyDomain : MarshalByRefObject
    {
        private Dictionary<string, string> GetAssemblyInfo(Assembly asm)
        {
            Object[] atts = asm.GetCustomAttributes(false);
            Dictionary<string, string> asmValues = new Dictionary<string, string>();

            asmValues.Add("AssemblyArchitecture", asm.GetName().ProcessorArchitecture.ToString());

            foreach (Object obj in atts)
            {
                PropertyInfo[] piArr = obj.GetType().GetProperties();

                foreach (PropertyInfo pi in piArr)
                {
                    if (pi.Name != "TypeId")
                    {
                        string attValue = pi.GetValue(obj, null).ToString();

                        asmValues.Add(obj.GetType().Name, attValue);
                        break;
                    }
                }
            }

            return asmValues;
        }


        private Dictionary<string, List<string>> GetManifest(Assembly assembly)
        {
            try
            {
                Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
                List<string> manifestData;

                using (Stream stream = assembly.GetManifestResourceStream("Manifest"))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        manifestData = new List<string>(streamReader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                    }
                }

                foreach (string row in manifestData)
                {
                    string[] rowSplit = row.Split('|');

                    if (result.ContainsKey(rowSplit[0]))
                    {
                        result[rowSplit[0]].Add(rowSplit[rowSplit.Count() - 1]);
                    }
                    else
                    {
                        List<string> newList = new List<string>();
                        newList.Add(rowSplit[rowSplit.Count() - 1]);
                        result.Add(rowSplit[0], new List<string>());
                    }

                }
                return result;
            }
            catch
            {
                return null;
            }

        }

        public void LoadAssembly(string assemblyPath)
        {
            try
            {
                Assembly.LoadFile(assemblyPath);
            }
            catch
            {
            }
        }


        public SelfHostedPSInfo GetAssembly(string assemblyPath)
        {
            try
            {
                Assembly readAssembly = Assembly.LoadFile(assemblyPath);
                Signature signature = SignatureHelper.GetSignature(assemblyPath);

                string signatureStatus = signature.Status.ToString();
                string signedBy = string.Empty;
                string timestampedBy = string.Empty;

                if (signatureStatus == "") signatureStatus = "Not signed";
                
                if (signature.SignerCertificate != null)
                    signedBy = signature.SignerCertificate.Subject;

                if (signature.TimeStamperCertificate != null)
                    timestampedBy = signature.TimeStamperCertificate.Subject;

                return new SelfHostedPSInfo(GetAssemblyInfo(readAssembly), GetManifest(readAssembly), signatureStatus, signedBy, timestampedBy, assemblyPath);
            }   
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot load assembly",ex);
            }
        }
    }
}

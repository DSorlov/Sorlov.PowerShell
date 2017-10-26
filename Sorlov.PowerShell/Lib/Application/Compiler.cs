using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Sorlov.PowerShell.Lib.API;
using Sorlov.PowerShell.Lib.Application;
using System.Collections;
using Sorlov.SelfHostedPS;

namespace Sorlov.PowerShell.SelfHosted.Lib.Application
{
    public static class Compiler
    {
        public static Collection<PSObject> CompileAdvanced(PSCmdlet caller, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            return Compile(new CallerProxy(caller), string.Empty, outputName, appdata, tempDir);
        }

        public static Collection<PSObject> CompileAdvanced(CallerProxy caller, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            return Compile(caller, string.Empty, outputName, appdata, tempDir);
        }
        public static Collection<PSObject> CompileString(PSCmdlet caller, string scriptData, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            string scriptFile = Path.Combine(tempDir, "script.ps1");
            File.WriteAllText(scriptFile,scriptData);

            return Compile(new CallerProxy(caller), scriptFile, outputName, appdata, tempDir);
        }

        public static Collection<PSObject> CompileString(CallerProxy caller, string scriptData, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            string scriptFile = Path.Combine(tempDir, "script.ps1");
            File.WriteAllText(scriptFile, scriptData);

            return Compile(caller, scriptFile, outputName, appdata, tempDir);
        }

        public static Collection<PSObject> CompileStandard(PSCmdlet caller, string sourceFile, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            return Compile(new CallerProxy(caller), sourceFile, outputName, appdata, tempDir);
        }
        public static Collection<PSObject> CompileStandard(CallerProxy caller, string sourceFile, string outputName, ApplicationData appdata)
        {
            caller.WriteVerbose("Setting up working directory..");
            string tempDir = Path.Combine(Path.GetTempPath(), "SPSH--" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            return Compile(caller, sourceFile, outputName, appdata, tempDir);
        }

        private static Collection<PSObject> Compile(CallerProxy caller, string sourceFile, string outputName, ApplicationData appdata, string tempDir)
        {
            // Make a manifest
            StringBuilder manifestContent = new StringBuilder();
            List<ParameterItem> parameters = new List<ParameterItem>();

            // Get some common metadata from the registry

            // Create list of all assemblies that needs to be embedded into code
            caller.WriteVerbose("Loading assemblies..");
            List<string> assemblyList = new List<string>();
            assemblyList.Add(GetDLLLocation("System"));
            assemblyList.Add(GetDLLLocation("System.Management"));
            assemblyList.Add(GetDLLLocation("System.Management.Automation"));
            assemblyList.Add(GetDLLLocation("System.Windows.Forms"));
            if (appdata.GetType() == typeof(ServiceData))
            {
                assemblyList.Add(GetDLLLocation("System.ServiceProcess"));
                assemblyList.Add(GetDLLLocation("System.Configuration.Install"));
            }
            if (appdata.Framework == Framework.Framework40)
            {
                assemblyList.Add(GetDLLLocation("System.Core"));
            }
            foreach (string assName in appdata.AdditionalAssemblies)
            {
                assemblyList.Add(GetDLLLocation(assName));                
            }


            // Setup the compiller
            caller.WriteVerbose("Configuring compiler..");
            CompilerParameters compilerParameters = new CompilerParameters(assemblyList.ToArray(), outputName, appdata.DebugBuild);
            compilerParameters.TempFiles = new TempFileCollection(tempDir,appdata.DebugBuild);
            compilerParameters.GenerateExecutable = true;
            compilerParameters.GenerateInMemory = false;

            if (appdata.Icon == null)
            {
                compilerParameters.CompilerOptions = string.Format(" /platform:{1} /target:{0} /optimize", (appdata.GetType() == typeof(ServiceData)) || appdata.HideConsole ? "winexe" : "exe", appdata.Platform);                                
            }
            else
            {
                caller.WriteVerbose("Creating icon..");
                string iconPath = Path.Combine(tempDir, "app.ico");

                IntPtr hiconPtr = appdata.Icon.GetHicon();
                Icon realIcon = Icon.FromHandle(hiconPtr);
                FileStream iconStream = new System.IO.FileStream(iconPath, FileMode.OpenOrCreate);
                realIcon.Save(iconStream);
                iconStream.Close();
                USER32.DestroyIcon(hiconPtr);

                compilerParameters.CompilerOptions = string.Format(" /platform:{2} /target:{1} /win32icon:{0} /optimize", iconPath, (appdata.GetType() == typeof(ServiceData)) || appdata.HideConsole ? "winexe" : "exe", appdata.Platform);                
            }


            // embedding script,core and modules
            caller.WriteVerbose("Embedding resources..");
            if (appdata.EmbedCore)
            {
                caller.WriteVerbose("Get core-dll..");
                Collection<PSObject> modulePathResult = caller.InvokeScript("(Get-Module -All Sorlov.Powershell).Path");
                if (modulePathResult.Count == 1)
                    manifestContent.AppendLine(EmbedResource(compilerParameters, modulePathResult[0].ToString(), tempDir, "CoreDLL", "Module"));
            }

            caller.WriteVerbose("Embedding extra resources..");
            {
                foreach(string extraResource in appdata.AdditionalFiles)
                manifestContent.AppendLine(EmbedResource(compilerParameters, extraResource,tempDir, Path.GetFileName(extraResource), "Copy"));                
            }

            caller.WriteVerbose("Get script data..");
            if (sourceFile!=string.Empty) manifestContent.AppendLine(EmbedResource(compilerParameters, sourceFile, tempDir, "ScriptData", "Script"));

            string scriptName;
            if (sourceFile != string.Empty)
                scriptName = System.IO.Path.GetFileName(sourceFile);

            caller.WriteVerbose("Processing arguments..");
            ScriptBlock scriptToProcess = ScriptBlock.Create(File.ReadAllText(sourceFile));
            List<Ast> paramBlockList = scriptToProcess.Ast.FindAll(ast1 => ast1.GetType() == typeof(ParamBlockAst), false).ToList();

            List<string> supportedTypes = new List<string>()
            {
                "byte","sbyte","short","ushort","int","uint","ulong","long","float","double","decimal","char","switch","bool","boolean","string"
            };

            if (paramBlockList.Count > 0)
            {
                ParamBlockAst paramBlock = paramBlockList[0] as ParamBlockAst;
                foreach (ParameterAst param in paramBlock.Parameters)
                {
                    string name = param.Name.ToString().Substring(1);
                    string helpMessage = string.Empty;
                    bool mandatory = false;
                    string type = "string";
                    foreach (AttributeBaseAst attributeAst in param.Attributes)
                    {
                        if (attributeAst.GetType() == typeof (AttributeAst))
                        {
                            foreach (NamedAttributeArgumentAst namedArgumentAst in ((AttributeAst)attributeAst).NamedArguments)
                            {
                                if (namedArgumentAst.ArgumentName.ToLower() == "helpmessage") helpMessage = namedArgumentAst.Argument.ToString();
                                if (namedArgumentAst.ArgumentName.ToLower() == "mandatory") mandatory = bool.Parse(namedArgumentAst.Argument.ToString().Replace("{", "").Replace("}", "").Replace("$", ""));
                            }
                        }
                        if (attributeAst.GetType() == typeof (TypeConstraintAst))
                        {
                            type = attributeAst.ToString().Replace("[", "").Replace("]", "").ToLower();
                            if (!supportedTypes.Contains(type))
                                caller.WriteWarning(string.Format("Argument '{0}' is of an unsupported type ({1}). Might not work or maybe it will. Who knows?", name, type));
                           
                        }
                    }
                    parameters.Add(new ParameterItem(name, helpMessage, mandatory, type));
                    
                }
            }



            caller.WriteVerbose("Get external modules..");
            if (appdata.EmbedModules != null)
                foreach (PSModuleInfo module in appdata.EmbedModules)
                    EmbedModule(module, compilerParameters, tempDir, manifestContent);

            caller.WriteVerbose("Creating manifest..");
            EmbedTextResource(compilerParameters, tempDir, manifestContent.ToString(), "Manifest");


            caller.WriteVerbose("Loading compiler..");
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", string.Format("v{0}", appdata.Framework.GetEnumDescription()));
            CSharpCodeProvider cSharpProvider = new CSharpCodeProvider(providerOptions);

            caller.WriteVerbose("Constructing assembly info..");
            string assemblyInfo = Resources.SelfHostedPS.AssemblyInfo;
            string assemblyCulture = appdata.LCID == 0 ? System.Globalization.CultureInfo.InvariantCulture.Name : System.Globalization.CultureInfo.GetCultureInfo(appdata.LCID).Name;
            assemblyInfo = assemblyInfo.Replace("$COPYRIGHT$", string.Format("© {0} {1}, All rights reserved.", DateTime.Now.Year, appdata.PublisherOrganization == string.Empty ? appdata.PublisherName : appdata.PublisherOrganization));
            assemblyInfo = assemblyInfo.Replace("$DESCRIPTION$", string.Format("{0};{1};{2};{3};{4};{5}", DateTime.Now, System.Security.Principal.WindowsIdentity.GetCurrent().Name, appdata.PublisherOrganization, appdata.PublisherName, Assembly.GetExecutingAssembly().GetName().Version, (appdata.GetType() == typeof(ServiceData))));
            assemblyInfo = assemblyInfo.Replace("$VERSION$", appdata.Version.ToString());
            assemblyInfo = assemblyInfo.Replace("$SCRIPTNAME$", appdata.ApplicationName);
            assemblyInfo = assemblyInfo.Replace("$COMPANY$", appdata.PublisherOrganization == string.Empty ? appdata.PublisherName : appdata.PublisherOrganization);
            assemblyInfo = assemblyInfo.Replace("$CULTURENAME$", assemblyCulture);
            assemblyInfo = assemblyInfo.Replace("//[", "[");
            assemblyInfo = assemblyInfo.Replace("\\", "\\\\");

            caller.WriteVerbose("Collecting code..");
            List<string> codeList = new List<string>();
            codeList.Add(assemblyInfo);
            codeList.Add(Resources.SelfHostedPS.Host);
            codeList.Add(Resources.SelfHostedPS.HostRawUserInterface);
            codeList.Add(Resources.SelfHostedPS.HostUserInterface);
            codeList.Add(Resources.SelfHostedPS.Common);
            codeList.Add(Resources.SelfHostedPS.ConsoleHandler);
            codeList.Add(Resources.SelfHostedPS.Commandline);

            caller.WriteVerbose("Making program adjustments..");
            if (appdata.ReplaceProgram == string.Empty)
            {
                if ((appdata.GetType() == typeof (ServiceData)))
                {
                    codeList.Add(Resources.SelfHostedPS.IntegratedServiceInstaller);

                    string serviceProgram = Resources.SelfHostedPS.ServiceProgram;
                    if (appdata.Mode.ToString() == "STA") { serviceProgram = serviceProgram.Replace("[MTAThread]", "[STAThread]"); }
                    if (appdata.LCID != 0) { serviceProgram = serviceProgram.Replace("System.Globalization.CultureInfo.InvariantCulture", string.Format("System.Globalization.CultureInfo.GetCultureInfo({0})", appdata.LCID)); }
                    codeList.Add(serviceProgram);

                    string serviceCode = Resources.SelfHostedPS.PSService;
                    if (appdata.Mode.ToString() == "STA") { serviceCode = serviceCode.Replace("System.Threading.ApartmentState.MTA", "System.Threading.ApartmentState.STA"); }
                    serviceCode = serviceCode.Replace("$SERVICENAME$", ((ServiceData) appdata).ServiceName);
                    serviceCode = serviceCode.Replace("$DISPLAYNAME$", ((ServiceData) appdata).DisplayName);
                    serviceCode = serviceCode.Replace("$DESCRIPTION$", ((ServiceData) appdata).Description);
                    codeList.Add(serviceCode);
                }
                else
                {
                    codeList.Add(Resources.SelfHostedPS.ParameterItem);
                    codeList.Add(Resources.SelfHostedPS.AssemblyData);

               

                    string programCode = Resources.SelfHostedPS.Program;
                    if (appdata.LCID != 0) { programCode = programCode.Replace("System.Globalization.CultureInfo.InvariantCulture", string.Format("System.Globalization.CultureInfo.GetCultureInfo({0})", appdata.LCID)); }
                    if (appdata.Mode.ToString() == "STA") { programCode = programCode.Replace("[MTAThread]", "[STAThread]").Replace("System.Threading.ApartmentState.MTA", "System.Threading.ApartmentState.STA"); }
                    if (appdata.HideConsole == true) { programCode = programCode.Replace("hideCon = false", "hideCon = true"); }

                    if (parameters.Count>0)
                    {
                        StringBuilder appendCode = new StringBuilder();
                        foreach (ParameterItem parameter in parameters)
                            appendCode.AppendFormat("validCmds.Add(new ParameterItem(\"{0}\", \"{1}\", {2}, \"{3}\"));\n", parameter.Name.Replace("\"", "").Replace("'", ""), parameter.HelpText.Replace("\"", "").Replace("'", ""), parameter.Mandatory.ToString().ToLower(), parameter.Type);
                        
                        programCode = programCode.Replace("//AddValidCommands", appendCode.ToString());
                    }
                    else
                    {
                        programCode = programCode.Replace("//AddValidCommands", "");
                    }

                    codeList.Add(programCode);
                }
            }
            else
            {
                string programCode = appdata.ReplaceProgram;
                if (appdata.LCID != 0) { programCode = programCode.Replace("System.Globalization.CultureInfo.InvariantCulture", string.Format("System.Globalization.CultureInfo.GetCultureInfo({0})", appdata.LCID)); }
                if (appdata.Mode.ToString() == "STA") { programCode = programCode.Replace("[MTAThread]", "[STAThread]").Replace("System.Threading.ApartmentState.MTA", "System.Threading.ApartmentState.STA"); }
                if (appdata.HideConsole == true) { programCode = programCode.Replace("hideCon = false", "hideCon = true"); }
                codeList.Add(programCode);                
            }

            foreach(string additionalCode in appdata.AdditionalCode)
                codeList.Add(additionalCode);

            caller.WriteVerbose("Compiling..");
            CompilerResults results = cSharpProvider.CompileAssemblyFromSource(compilerParameters, codeList.ToArray());

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning)
                        caller.WriteWarning(string.Format("({0}): {1}, at {2} line {3}", error.ErrorNumber, error.ErrorText, error.FileName, error.Line));
                    else
                        caller.WriteError(new ErrorRecord(new Exception(error.ErrorText), error.ErrorNumber, ErrorCategory.NotSpecified, error));
                }
            }



            if (!appdata.DebugBuild)
            {
                caller.WriteVerbose("Deleting temporary files..");

                foreach (string fileName in Directory.GetFiles(tempDir))
                    File.Delete(fileName);

                Directory.Delete(tempDir);
            }

            if (appdata.SigningInformation!=null)
            {
                try
                {
                    caller.WriteVerbose("Signing file..");

                    if (appdata.SigningInformation.Certificate==null)
                        caller.WriteVerbose("No certificate specified, will try to auto-find code-signing certificate in local store..");

                    if (appdata.SigningInformation.TimestampServer == string.Empty)
                        caller.WriteVerbose("No time-stamping server specified, will try to use default..");

                   SignatureHelper.SetFileSignature(outputName, appdata.SigningInformation.Certificate, appdata.SigningInformation.TimestampServer);
                }
                catch(Exception e)
                {
                    caller.ThrowTerminatingError(new ErrorRecord(new PSSecurityException("Could not sign file",e),"200",ErrorCategory.InvalidOperation,null));
                    File.Delete(outputName);
                }
            }

            return caller.InvokeScript(string.Format("Get-SelfHostedPS -Path \"{0}\"",outputName));



        }

        private static string GetDLLLocation(string name)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == name) return a.Location;
            }
            return null;
        }

        private static string CompressCopy(string fromPath, string destPath)
        {
            using (Stream fs = File.OpenRead(fromPath))
            using (Stream fd = File.Create(destPath))
            using (Stream csStream = new GZipStream(fd, CompressionMode.Compress))
            {
                byte[] buffer = new byte[1024];
                int nRead;
                while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    csStream.Write(buffer, 0, nRead);
                }
            }
            return destPath;
        }

        private static void EmbedTextResource(CompilerParameters compiler, string tempDir, string text, string resourceName)
        {
            string fileName = System.IO.Path.Combine(tempDir, resourceName);

            using (StreamWriter streamWriter = System.IO.File.CreateText(fileName))
            {
                streamWriter.Write(text);
            }

            compiler.EmbeddedResources.Add(fileName);
        }

        private static string EmbedResource(CompilerParameters compiler, string fromPath, string tempDir, string resourceName, string type)
        {
            string newFile = CompressCopy(fromPath, Path.Combine(tempDir, resourceName));
            compiler.EmbeddedResources.Add(newFile);
            return string.Format("{0}|{1}|{2}", type, resourceName, System.IO.Path.GetFileName(fromPath));
        }

        private static List<string> EmbedModule(PSModuleInfo module, CompilerParameters compilerParameters, string tempDir, StringBuilder manifest)
        {
            List<string> result = new List<string>();

            manifest.AppendLine(EmbedResource(compilerParameters, module.Path, tempDir, module.Name, "Module"));
            foreach (PSModuleInfo subModule in module.NestedModules)
                EmbedModule(subModule, compilerParameters, tempDir, manifest);

            foreach(string file in module.ExportedFormatFiles)
                manifest.AppendLine(EmbedResource(compilerParameters, System.IO.Path.Combine(module.ModuleBase,file), tempDir, System.IO.Path.GetFileName(file), "Copy"));

            foreach(string file in module.ExportedTypeFiles)
                manifest.AppendLine(EmbedResource(compilerParameters, System.IO.Path.Combine(module.ModuleBase, file), tempDir, System.IO.Path.GetFileName(file), "Copy"));

            return result;
        }


    }
}

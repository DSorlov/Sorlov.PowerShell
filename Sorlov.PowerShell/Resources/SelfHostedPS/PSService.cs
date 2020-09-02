using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Sorlov.SelfHostedPS.Application;
using System.Threading;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Configuration.Install;

namespace Sorlov.SelfHostedPS.Service
{
    public class PSService: ServiceBase
    {

        public static string PSServiceName = "$SERVICENAME$";
        public static string PSServiceDisplayName = "$DISPLAYNAME$";
        public static string PSServiceDescription = "$DESCRIPTION$";
        private static Thread thread;

        private static Host powershellHost;
        private static System.Management.Automation.PowerShell powershellEngine;
        private static object psLocker;
        private static Pipeline mainPipe;

        private static void RunScript(string script)
        {
            try
            {
                mainPipe = powershellEngine.Runspace.CreatePipeline();
                mainPipe.Commands.AddScript(script);
                mainPipe.Invoke();

            }
            catch(Exception e)
            {
                    EventLog.WriteEntry(PSService.PSServiceName, string.Format("A internal error occured.{0}{0}Error: {1}", Environment.NewLine, e.Message), EventLogEntryType.Error);
            }
            finally
            {
            }
        }


        private void RunService()
        {
            InitialSessionState initalState = InitialSessionState.CreateDefault();
            List<string> modulesToLoad = Common.ProcessManifest(initalState);

            lock (psLocker)
            {
                powershellHost = new Host();
                powershellEngine = System.Management.Automation.PowerShell.Create();

                powershellEngine.Runspace = RunspaceFactory.CreateRunspace(powershellHost, initalState);
                powershellEngine.Runspace.ApartmentState = System.Threading.ApartmentState.MTA;
                powershellEngine.Runspace.Open();
            }

            try
            {
                RunScript("Set-ExecutionPolicy Unrestricted -Scope Process");
            }
            catch
            {
            }


            foreach (string module in modulesToLoad)
                try
                {
                    RunScript(string.Format("Import-Module {0} -Scope Global", module));
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry(PSService.PSServiceName, string.Format("Error occured while initializing modules.{0}{0}Name: {1}{0}Error: {2}", Environment.NewLine, module, e.Message), EventLogEntryType.Error);
                }

            RunScript(Common.GetScript("ScriptData"));
            RunScript("if (Get-Command OnStart -ea SilentlyContinue) { OnStart }");
            RunScript("Main");

            lock (psLocker)
            {
                powershellEngine.Dispose();
                powershellEngine = null;
            }

            thread.Abort();
            thread.Join();
 
            Common.CleanUp();

        }

        public void StartInternal()
        {
            psLocker = new object();

            thread = new Thread(new ThreadStart(RunService));
            thread.Start();
        }


        public void StopInternal(string callMethod)
        {
            mainPipe.Stop();
            RunScript("if (Get-Command OnStop -ea SilentlyContinue) { OnStop('"+callMethod+"') }");
        }

        protected override void OnStart(string[] args)
        {
            StartInternal();
        }

        protected override void OnStop()
        {
            StopInternal("OnStop");
        }

        protected override void OnShutdown()
        {
            StopInternal("OnShutdown");
        }

        protected static bool IsServiceInstalled()
        {
            ServiceController[] allControllers = ServiceController.GetServices();
                
            foreach(ServiceController serviceController in allControllers)
                if (serviceController.ServiceName == PSServiceName) return true;

            return false;
        }

        public static void InstallService(ServiceAccount account, ServiceStartMode startMode, string username, string password)
        {
            if (IsServiceInstalled())
                UninstallService();

            try
            {
                IntegratedServiceInstaller.Install(Assembly.GetExecutingAssembly().Location, PSService.PSServiceName, PSService.PSServiceDisplayName, PSService.PSServiceDescription, account, startMode, username, password);
            }
            catch
            {
            }

        }

        public static void UninstallService()
        {
            try
            {
                IntegratedServiceInstaller.Uninstall(PSService.PSServiceName);
            }
            catch
            {
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace Sorlov.SelfHostedPS.Service
{
    public static class IntegratedServiceInstaller
    {
    	public static void Install(string path, string serviceName, string displayName, string description, ServiceAccount account, ServiceStartMode startMode, string username, string password)
    	{
    		ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Username = username;
            serviceProcessInstaller.Password = password;
            serviceProcessInstaller.Account = account;

    		ServiceInstaller serviceInstaller = new ServiceInstaller();
            InstallContext installContext = new InstallContext();
    		
            installContext = new InstallContext("", new string[] { string.Format("/assemblypath={0}", path) });

    		serviceInstaller.Context = installContext;
            serviceInstaller.DisplayName = displayName;
            serviceInstaller.Description = description;
            serviceInstaller.ServiceName = serviceName;
    		serviceInstaller.StartType = startMode;
    		serviceInstaller.Parent = serviceProcessInstaller;

    		ListDictionary state = new ListDictionary();
    		serviceInstaller.Install(state);

            using (RegistryKey oKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SYSTEM\CurrentControlSet\Services\{0}", serviceInstaller.ServiceName), true))
            {
                try
                {
                    Object sValue = oKey.GetValue("ImagePath");
                    oKey.SetValue("ImagePath", sValue);
                }
                catch
                {
                }
            }

    	}
    	public static void Uninstall(string serviceName)
    	{
    		ServiceInstaller serviceInstaller = new ServiceInstaller();
    		InstallContext installContext = new InstallContext("", null);

    		serviceInstaller.Context = installContext;
            serviceInstaller.ServiceName = serviceName;
            serviceInstaller.Uninstall(null);
    	}
    }
}

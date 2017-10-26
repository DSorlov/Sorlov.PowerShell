using System.Management.Automation;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Collections.ObjectModel;
using Sorlov.PowerShell.Lib.Core;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Initialize, "PSDocumentation")]
    [NoAutoDoc]
    public class InitializePSDocumentation : PSCmdlet
    {

        private string path;

        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }


        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            ProviderInfo pInfo;
            Collection<string> resolvedPaths = SessionState.Path.GetResolvedProviderPathFromPSPath(path, out pInfo);
            string pathName = resolvedPaths[0];

            if (!Directory.Exists(pathName))
            {
                ThrowTerminatingError(new ErrorRecord(new DirectoryNotFoundException(string.Format("Directory '{0}' not found",pathName)),"100",ErrorCategory.OpenError,path));
            }
            else
            {
                foreach(string fileName in Directory.GetFiles(path,"Sorlov.PowerShell.*.dll",SearchOption.TopDirectoryOnly))
                    XMLHelpGenerator.GenerateHelp(fileName,pathName, true);
            }
        }
        #endregion

    }
}

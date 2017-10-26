using System;
using System.Drawing;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New,"Base64Image")]
    [OutputType(typeof(string))]
    public class NewBase64Image: PSCmdlet
    {
        private string parPath;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The output path/file", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parPath; }
            set { parPath = value; }
        }


        protected override void ProcessRecord()
        {
            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2));

            Image rawImage = ((new Uri(parPath).IsFile)) ? rawImage = ImageEncoding.ReadImage(System.IO.Path.GetFullPath(parPath)) : ImageEncoding.DownloadImage(parPath);

            WriteObject(ImageEncoding.ImageToBase64String(rawImage));

        }
    }
}

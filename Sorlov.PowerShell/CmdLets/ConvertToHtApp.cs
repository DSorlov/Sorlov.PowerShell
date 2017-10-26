using System;
using System.Drawing;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Sorlov.PowerShell.Lib.HTApps;
using Sorlov.PowerShell.Resources;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "HTApp")]
    public class ConvertToHtApp: PSCmdlet
    {

        private string parPath;
        private string parTitle = "Automatically converted HTApp";
        private int parWidth = 300;
        private int parHeight = 300;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The output path/file", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parPath; }
            set { parPath = value; }
        }

        [Parameter(HelpMessage = "The title of the window")]
        [ValidateNotNullOrEmpty()]
        public string Title
        {
            get { return parTitle; }
            set { parTitle = value; }
        }
        [Parameter(HelpMessage = "The width of the window")]
        [ValidateRange(50, 1200)]
        public int Width
        {
            get { return parWidth; }
            set { parWidth = value; }
        }
        [Parameter(HelpMessage = "The height of the window")]
        [ValidateRange(50,1200)]
        public int Height
        {
            get { return parHeight; }
            set { parHeight = value; }
        }


        protected override void ProcessRecord()
        {
            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2));

            parPath = System.IO.Path.GetFullPath(parPath);

            string htmlData = File.ReadAllText(parPath);

            htmlData = Regex.Replace(htmlData, "(?=<img)(.*?)src=[\"|'](.*?)[\"|']", delegate(Match match)
            {
                Image rawImage = ((new Uri(match.Groups[2].Value).IsFile)) ? rawImage = ImageEncoding.ReadImage(System.IO.Path.GetFullPath(match.Groups[2].Value)) : ImageEncoding.DownloadImage(match.Groups[2].Value);

                string encodedImage = ImageEncoding.ImageToBase64String(rawImage);
                return string.Format("{0}src=\"data:image/image;base64,{1}\"", match.Groups[1].Value, encodedImage);
            });

            string newName = string.Format("{1}\\{0}.htapp", System.IO.Path.GetFileNameWithoutExtension(parPath),System.IO.Path.GetDirectoryName(parPath));

            string header = Templates.StandardBlock;
            header = header.Replace("$WIDTH$", parWidth.ToString());
            header = header.Replace("$HEIGHT$", parHeight.ToString());
            header = header.Replace("$TITLE$", parTitle);

            htmlData = header + htmlData;

            File.WriteAllText(newName,htmlData);
        }
    }
}

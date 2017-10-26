using System.Management.Automation;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    internal class ZipFileDriveInfo : PSDriveInfo
    {
        private ZipFileArchive archive;

        public ZipFileArchive Archive
        {
            get { return archive; }
            set { archive = value; }
        }

        public ZipFileDriveInfo(PSDriveInfo driveInfo)
            : base(driveInfo)
        { }

    }
}

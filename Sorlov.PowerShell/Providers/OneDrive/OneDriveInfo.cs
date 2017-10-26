using System.Management.Automation;
using Sorlov.PowerShell.Lib.MicrosoftLive;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    internal class OneDriveInfo : PSDriveInfo
    {

        private LiveConnectClient client;

        public LiveConnectClient Client
        {
            get { return client; }
            set { client = value; }
        }

        public OneDriveInfo(PSDriveInfo driveInfo)
            : base(driveInfo)
        { }

    }
}

using System.ComponentModel;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public class LiveDownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public LiveDownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, object userState)
            : base((int)((totalBytesToReceive > 0) ? (100 * bytesReceived / totalBytesToReceive) : 0), userState)
        {
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReveive = totalBytesToReceive;
        }

        public long BytesReceived { get; private set; }

        public long TotalBytesToReveive { get; private set; }
    }
}

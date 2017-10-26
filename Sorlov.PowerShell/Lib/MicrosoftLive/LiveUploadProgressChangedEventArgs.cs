using System.ComponentModel;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public class LiveUploadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public LiveUploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend, object userState)
            : base((int)((totalBytesToSend > 0) ? (100 * bytesSent / totalBytesToSend) : 0), userState)
        {
            this.BytesSent = bytesSent;
            this.TotalBytesToSend = totalBytesToSend;
        }

        public long BytesSent { get; private set; }

        public long TotalBytesToSend { get; private set; }
    }
}

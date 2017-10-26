using System;
using System.IO;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public class LiveDownloadResult
    {
        public LiveDownloadResult(Exception error, bool cancelled)
        {
            this.Error = error;
            this.Cancelled = cancelled;
        }

        public LiveDownloadResult(Stream result)
        {
            ThrowArgumentException.IfNull(result, "result");

            this.Result = result;
        }

        public bool Cancelled { get; private set; }

        public Exception Error { get; private set; }

        public Stream Result { get; private set; }
    }
}

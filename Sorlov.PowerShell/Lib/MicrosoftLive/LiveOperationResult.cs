using System;
using System.Collections.Generic;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public class LiveOperationResult
    {
        public LiveOperationResult(Exception error, bool cancelled)
        {
            this.Error = error;
            this.Cancelled = cancelled;
        }

        public LiveOperationResult(IDictionary<string, object> result, string rawResult)
        {
            ThrowArgumentException.IfNull(result, "result");
            ThrowArgumentException.IfNull(rawResult, "rawResult");

            this.Result = result;
            this.RawResult = rawResult;
        }

        public bool Cancelled { get; private set; }

        public Exception Error { get; private set; }

        public IDictionary<string, object> Result { get; private set; }

        public string RawResult { get; private set; }
    }
}

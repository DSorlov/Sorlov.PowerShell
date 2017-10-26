using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Sorlov.PowerShell.Lib.Application
{
    public class CallerProxy
    {
        private readonly PSCmdlet caller = null;

        public CallerProxy(PSCmdlet caller)
        {
            this.caller = caller;
        }

        public CallerProxy()
        {   
        }

        public delegate void CallerProxyEvent(CallerProxyEventArgs e);
        public event CallerProxyEvent Output;
        protected virtual void OnOutput(CallerProxyEventArgs e)
        {
            CallerProxyEvent handler = Output;
            if (handler != null)
            {
                handler(e);
            }
        }

        public void ThrowTerminatingError(ErrorRecord errorRecord)
        {
            if (caller != null)
                caller.ThrowTerminatingError(errorRecord);
            else
                throw new Exception("ThrowTerminatingError received a proxy error: " + errorRecord.ErrorDetails, errorRecord.Exception);
        }

        public Collection<PSObject> InvokeScript(string script)
        {
            if (caller != null)
                return caller.SessionState.InvokeCommand.InvokeScript(script);
            else
                return new Collection<PSObject>();
        }

        public void WriteVerbose(string text)
        {
            if (caller != null)
                caller.WriteVerbose(text);
            else
                OnOutput(new CallerProxyEventArgs() { StreamName = "Verbose", Text = text});
        }
        public void WriteWarning(string text)
        {
            if (caller != null)
                caller.WriteWarning(text);
            else
                OnOutput(new CallerProxyEventArgs() { StreamName = "Warning", Text = text});
        }

        public void WriteError(ErrorRecord errorRecord)
        {
            if (caller != null)
                caller.WriteError(errorRecord);
            else
                OnOutput(new CallerProxyEventArgs() { StreamName = "Error", Text = errorRecord.ToString() });
        }

    }

}

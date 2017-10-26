using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Drawing;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "DesktopColor")]
    [CmdletDescription("Get the color of the desktop",
        "Get the color of the desktop")]
    public class GetDesktopColor : PSCmdlet
    {
        #region "Private Parameters"
        private bool advanced = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter AdvancedMode
        {
            get { return advanced; }
            set { advanced = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");
            if (DWMAPI.DwmIsCompositionEnabled())
            {
                DWMAPI.DWM_COLORIZATION_PARAMS colorParam;
                DWMAPI.DwmGetColorizationParameters(out colorParam);
                if (advanced)
                {
                    WriteObject(colorParam);
                }
                else
                {
                    Color color = Color.FromArgb((int)colorParam.ColorizationColor);
                    WriteObject(color);
                }
            }
            else
            {
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException("Desktop composition is not enabled"), "100", ErrorCategory.InvalidOperation, null));
            }
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();            
        }

        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stop processing");
        }
        #endregion

    }
}

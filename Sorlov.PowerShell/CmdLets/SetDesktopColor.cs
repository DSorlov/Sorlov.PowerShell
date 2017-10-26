using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Drawing;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "DesktopColor")]
    [CmdletDescription("Sets transparancy for the current or selected window",
        "Sets transparancy for the current or selected window (Windows Vista and newer)")]
    public class SetDesktopColor : PSCmdlet
    {
        #region "Private Parameters"
        private object color = null;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, HelpMessage = "A color to set", ParameterSetName = "Standard")]
        [ValidateNotNullOrEmpty()]
        public object Color
        {
            get { return color; }
            set { color = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

            if (DWMAPI.DwmIsCompositionEnabled())
            {
                if (color.GetType() == typeof(Color))
                {
                    DWMAPI.DWM_COLORIZATION_PARAMS oldColor;
                    DWMAPI.DwmGetColorizationParameters(out oldColor);
                    oldColor.ColorizationColor = (uint)color;
                    DWMAPI.DwmSetColorizationParameters(ref oldColor, 0);
                }
                else if (color.GetType() == typeof(string))
                {
                    Color newColor = new Color();
                    try
                    {
                        newColor = System.Drawing.ColorTranslator.FromHtml((string)color);
                    }
                    catch
                    {
                        ThrowTerminatingError(new ErrorRecord(new FormatException("The supplied value could not be transformed to a real color. Try something like Red or Blue"), "100", ErrorCategory.InvalidData, color));
                    }
                    DWMAPI.DWM_COLORIZATION_PARAMS oldColor;
                    DWMAPI.DwmGetColorizationParameters(out oldColor);
                    oldColor.ColorizationColor = (uint)System.Drawing.Color.FromArgb(255, newColor.R, newColor.G, newColor.B).ToArgb();
                    DWMAPI.DwmSetColorizationParameters(ref oldColor, 0);

                }
                else if (color.GetType() == typeof(DWMAPI.DWM_COLORIZATION_PARAMS))
                {
                    DWMAPI.DWM_COLORIZATION_PARAMS newColor = (DWMAPI.DWM_COLORIZATION_PARAMS)color;
                    DWMAPI.DwmSetColorizationParameters(ref newColor, 0);
                }
                else
                {
                    ThrowTerminatingError(new ErrorRecord(new FormatException("Format of the color specified are not valid. Valid types are STRING, COLOR or DWM_COLORIZATION_PARAMS"), "100", ErrorCategory.InvalidData, color));
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

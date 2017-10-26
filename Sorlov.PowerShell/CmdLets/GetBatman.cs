using System;
using System.Management.Automation;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"Batman")]
    public class GetBatman: PSCmdlet
    {
        protected override void BeginProcessing()
        {
            string batMan =
                @"                ,/                         \," + Environment.NewLine +
                @"            /_==/          i     i          \==_\" + Environment.NewLine +
                @"           /XX/            |\___/|            \XX\" + Environment.NewLine +
                @"         /XXXX\            |XXXXX|            /XXXX\" + Environment.NewLine +
                @"        |XXXXXX\_         _XXXXXXX_         _/XXXXXX|" + Environment.NewLine +
                @"       XXXXXXXXXXXxxxxxxxXXXXXXXXXXXxxxxxxxXXXXXXXXXXX" + Environment.NewLine +
                @"      XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX|" + Environment.NewLine +
                @"      XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" + Environment.NewLine +
                @"      XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX|" + Environment.NewLine +
                @"      XXXXXX/^^^^'\XXXXXXXXXXXXXXXXXXXXX/^^^^^\XXXXXX" + Environment.NewLine +
                @"        |XXX|       \XXX/^^\XXXXX/^^\XXX/       |XXX|" + Environment.NewLine +
                @"          \XX\       \X/    \XXX/    \X/       /XX/" + Environment.NewLine +
                @"             '\       '      \X/      '      /'";

            WriteObject(batMan,true);
        }
    }
}

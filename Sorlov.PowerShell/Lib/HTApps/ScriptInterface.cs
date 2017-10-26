using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.Lib.HTApps
{
     [ComVisible(true)]
    public class ScriptInterface
    {
         private readonly ScriptForm hostForm;

        public ScriptInterface(ScriptForm hostForm)
        {
            this.hostForm = hostForm;
        }

         public string InvokeScript(string script)
         {
             return hostForm.SafeInvoke(hFrm=>hFrm.ExecuteScriptOnThread(script));
         }

         public void InfoMessage(string message)
         {
             hostForm.SafeInvoke(hFrm => hFrm.ShowDialog(this.Title, message, MessageBoxIcon.Information));
         }
         public void WarningMessage(string message)
         {
             hostForm.SafeInvoke(hFrm => hFrm.ShowDialog(this.Title, message, MessageBoxIcon.Warning));
         }
         public void ErrorMessage(string message)
         {
             hostForm.SafeInvoke(hFrm => hFrm.ShowDialog(this.Title, message, MessageBoxIcon.Error));
         }

         public void ExitObject(object exitData)
         {
             hostForm.SafeInvoke(hFrm => hFrm.ExitObject = exitData);
             hostForm.SafeInvoke(hFrm => hFrm.Close());
         }

         public void Exit()
         {
             hostForm.SafeInvoke(hFrm => hFrm.Close());
         }

         public void LoadApp(string path)
         {
             hostForm.SafeInvoke(hFrm => hFrm.LoadNewApplication(path));
         }

         public void Reload()
         {
             hostForm.SafeInvoke(hFrm => hFrm.LoadNewApplication(hostForm.SafeInvoke(hiFrm => hiFrm.LoadedScript)));
         }

        public string Title { get { return hostForm.SafeInvoke(hFrm=>hFrm.Text); } set { hostForm.SafeInvoke(hFrm=>hFrm.Text = value); } }

        public int Width { get { return hostForm.SafeInvoke(hFrm => hFrm.Width); } set { hostForm.SafeInvoke(hFrm => hFrm.Width = value); } }
        public int Height { get { return hostForm.SafeInvoke(hFrm => hFrm.Height); } set { hostForm.SafeInvoke(hFrm => hFrm.Height = value); } }

        public int Left { get { return hostForm.SafeInvoke(hFrm => hFrm.Left); } set { hostForm.SafeInvoke(hFrm => hFrm.Left = value); } }
        public int Top { get { return hostForm.SafeInvoke(hFrm => hFrm.Top); } set { hostForm.SafeInvoke(hFrm => hFrm.Top = value); } }






    }
}


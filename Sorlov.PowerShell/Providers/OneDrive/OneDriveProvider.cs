using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sorlov.PowerShell.Dto.MicrosoftLive;
using Sorlov.PowerShell.Lib.MicrosoftLive.Controls;
using Sorlov.PowerShell.Providers.FileSystem;
using Sorlov.PowerShell.Lib.MicrosoftLive;

namespace Sorlov.PowerShell.Providers.OneDrive
{
    [CmdletProvider("OneDrive", ProviderCapabilities.Credentials | ProviderCapabilities.ExpandWildcards)]
    public class OneDriveProvider : NavigationCmdletProvider, IContentCmdletProvider
    {

        private string hiddenRoot = "me/skydrive:me/skydrive/files";

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            try
            {
                using (var dialog = new SignInDialog())
                {
                    dialog.Scopes = new[] { "wl.basic", "wl.signin", "wl.offline_access", "wl.skydrive_update", "wl.contacts_skydrive", "wl.emails" };
                    dialog.ShowInTaskbar = true;
                    dialog.Locale = "en";
                    dialog.ClientId = "000000004412E411";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        OneDriveInfo oneDrive = new OneDriveInfo(drive);
                        oneDrive.Client = new LiveConnectClient(dialog.Session);

                        LiveOperationCompletedEventArgs eventArgs = default(LiveOperationCompletedEventArgs);
                        using (ManualResetEvent signal = new ManualResetEvent(false))
                        {
                            oneDrive.Client.GetCompleted += (s, e) => { eventArgs = e; signal.Set(); };
                            oneDrive.Client.GetAsync("me", signal);

                            signal.WaitOne();
                        }

                        if (eventArgs.Error == null)
                        {
                            foreach (var key in eventArgs.Result.Keys)
                                WriteVerbose(string.Format("{0}={1}", key, eventArgs.Result[key]));
                            return oneDrive;
                        }

                        if (eventArgs.Cancelled)
                        {
                            WriteError(new ErrorRecord(new Exception("Operation cancelled by user."), "503", ErrorCategory.InvalidOperation, null));
                            return null;
                        }

                        WriteError(new ErrorRecord(new Exception(eventArgs.Error.Message), "200", ErrorCategory.AuthenticationError, null));
                        return null;
                    }
                    else
                    {
                        WriteError(new ErrorRecord(new Exception("Operation cancelled by user."), "502", ErrorCategory.InvalidOperation, null));
                        return null;                        
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex.InnerException, "100", ErrorCategory.NotSpecified, null));
                return null;
            }
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            OneDriveInfo oneDrive = drive as OneDriveInfo;
            return oneDrive;
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            // Files = me/skydrive:me/skydrive/files
            // Folders = me/skydrive:me/skydrive/files?filter=folders
            // Photos = me/skydrive:me/skydrive/files?filter=albums

            OneDriveInfo oneDrive = PSDriveInfo as OneDriveInfo;

            LiveOperationCompletedEventArgs eventArgs = default(LiveOperationCompletedEventArgs);
            using (ManualResetEvent signal = new ManualResetEvent(false))
            {
                oneDrive.Client.GetCompleted += (s, e) => { eventArgs = e; signal.Set(); };
                oneDrive.Client.GetAsync(hiddenRoot + path, signal);

                signal.WaitOne();
            }

            var items = ((object[])eventArgs.Result["data"]).Cast<IDictionary<string, object>>();
            foreach (var item in items)
            {
                OneDriveItem onedriveItem = new OneDriveItem(item);
                WriteItemObject(onedriveItem, hiddenRoot + onedriveItem.Name, onedriveItem.IsFolder || onedriveItem.IsAlbum ? true : false);
            }
        }

        private bool PathIsDrive(string path)
        {
            return PSDriveInfo.Root == path ? true : false;
        }

        protected override bool HasChildItems(string path)
        {
            if (PathIsDrive(path))
                return true;

            return true;
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            GetChildItems(path, false);
        }


        protected override bool ItemExists(string path)
        {
            if (PathIsDrive(path))
                return true;

            return true;
        }

        protected override bool IsValidPath(string path)
        {
            if (PathIsDrive(path))
                return true;

            try
            {
                OneDriveInfo oneDrive = PSDriveInfo as OneDriveInfo;

                LiveOperationCompletedEventArgs eventArgs = default(LiveOperationCompletedEventArgs);
                using (ManualResetEvent signal = new ManualResetEvent(false))
                {
                    oneDrive.Client.GetCompleted += (s, e) => { eventArgs = e; signal.Set(); };
                    oneDrive.Client.GetAsync(hiddenRoot + path, signal);

                    signal.WaitOne();
                }

                OneDriveItem onedriveItem = new OneDriveItem((((object[])eventArgs.Result["data"]).Cast<IDictionary<string, object>>()).First());
                return onedriveItem.IsFolder || onedriveItem.IsAlbum ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ClearContent(string path)
        {
            throw new NotImplementedException();
        }

        public object ClearContentDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }

        public IContentReader GetContentReader(string path)
        {
            throw new NotImplementedException();
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }

        public IContentWriter GetContentWriter(string path)
        {
            throw new NotImplementedException();
        }

        public object GetContentWriterDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Org.BouncyCastle.Crypto.Parameters;
using Sorlov.PowerShell.Builder.Properties;
using Sorlov.PowerShell.Lib.Application;
using Sorlov.PowerShell.SelfHosted.Lib.Application;

namespace Sorlov.PowerShell.Builder
{
    public partial class BuilderForm : Form
    {
        private string fileName;

        public BuilderForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void threadingMTA_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void threadingSTA_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void exe64_CheckedChanged(object sender, EventArgs e)
        {
            serviceDataFrame.Enabled = false;
        }

        private void exe32_CheckedChanged(object sender, EventArgs e)
        {
            serviceDataFrame.Enabled = false;
        }

        private void exe64svc_CheckedChanged(object sender, EventArgs e)
        {
            serviceDataFrame.Enabled = true;
        }

        private void exe32svc_CheckedChanged(object sender, EventArgs e)
        {
            serviceDataFrame.Enabled = true;
        }

        private void BuilderForm_Load(object sender, EventArgs e)
        {
            System.Management.ObjectQuery oQuery = new ObjectQuery();

            authorCompany.Text = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion", "RegisteredOrganization", "");
            authorName.Text = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion", "RegisteredOwner", "");


            regionalSettings.Items.Add(new ComboListItem<int>("Culture invariant", 0));
            foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
                int newItem = regionalSettings.Items.Add(new ComboListItem<int>(string.Format("{0} - {1} ({2})", cultureInfo.LCID, regionInfo.EnglishName, regionInfo.TwoLetterISORegionName), cultureInfo.LCID));
            }
            regionalSettings.SelectedIndex = 0;


            frameworkVersion.SelectedIndex = 2;


            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                foreach (X509Extension extension in mCert.Extensions)
                {
                    if (extension.Oid.Value == "2.5.29.15")
                    //  if (extension.Oid.FriendlyName == "Key Usage")
                    {
                        X509KeyUsageExtension ext = (X509KeyUsageExtension)extension;
                        if (ext.KeyUsages.ToString()=="DigitalSignature")
                            certificateSelector.Items.Add(new ComboListItem<string>(mCert.SubjectName.Name, mCert.Thumbprint));
                    }
                }
            }
            if (certificateSelector.Items.Count == 0)
            {
                signFile.Enabled = false;
            }
            else
            {
                certificateSelector.SelectedIndex = 0;                
            }


            string[] args = Environment.GetCommandLineArgs();

            if (args.Length>1)
            {
                fileName = args[1];
            }
            else
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("You must select a script to start the application!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                else
                {
                    fileName = openFileDialog.FileName;
                }                
            }

            this.Text = string.Format("{0}: {1}", this.Text, fileName);

        }

        private void signFile_CheckedChanged(object sender, EventArgs e)
        {
            signBox.Enabled = signFile.Checked;
        }

        private void addFile_Click(object sender, EventArgs e)
        {
            if (addFileDialog.ShowDialog() == DialogResult.OK)
                addFiles.Items.Add(addFileDialog.FileName);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to exit? You changes will not be saved!","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void removeFile_Click(object sender, EventArgs e)
        {
            try
            {
                addFiles.Items.RemoveAt(addFiles.SelectedIndex);
            }
            catch (Exception)
            {
            }
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            targetFileDialog.FileName = fileName.Replace(".ps1", ".exe");
            if (targetFileDialog.ShowDialog() != DialogResult.OK)
                return;

            ApplicationData newApplicationData;

            if (exe64svc.Checked || exe32svc.Checked)
            {
                ServiceData newApp = new ServiceData();
                newApp.ServiceName = serviceName.Text;
                newApp.Description = serviceDescription.Text;
                newApp.DisplayName = serviceDisplayName.Text;
                newApplicationData = newApp;
            }
            else
            {
                newApplicationData = new ApplicationData();
            }

            newApplicationData.ApplicationName = Path.GetFileName(fileName);

            switch (frameworkVersion.Text)
            {
                case "Framework 2.0":
                    newApplicationData.Framework = Framework.Framework20;;
                    break;
                case "Framework 3.5":
                    newApplicationData.Framework = Framework.Framework35;
                    break;
                case "Framework 4.0":
                    newApplicationData.Framework = Framework.Framework40;
                    break;
                default:
                    newApplicationData.Framework = Framework.Framework40;
                    break;
            }

            newApplicationData.DebugBuild = !buildRelease.Checked;

            newApplicationData.HideConsole = hideConsole.Checked;

            if (iconImage.Tag != null)
            {
                newApplicationData.Icon = Properties.Resources.exe;
            }

            newApplicationData.LCID = ((ComboListItem<int>) regionalSettings.SelectedItem).Value;

            newApplicationData.Mode = threadingSTA.Checked ? ThreadMode.STA : ThreadMode.MTA;

            newApplicationData.Platform = (exe32.Checked || exe32svc.Checked) ? Platform.x86 : Platform.x64;

            newApplicationData.PublisherName = authorName.Text;

            newApplicationData.PublisherOrganization = authorCompany.Text;

            newApplicationData.Version = new Version(versionBox.Text.Replace(",","."));

            newApplicationData.AdditionalFiles.AddRange(addFiles.Items.Cast<string>());

            if (signFile.Checked)
            {
                X509Store store = new X509Store(StoreName.My);
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 mCert in store.Certificates)
                {
                    if (mCert.Thumbprint == ((ComboListItem<string>)certificateSelector.SelectedItem).Value)
                    {
                        newApplicationData.SigningInformation = new SingingInformation()
                        {
                            Certificate = mCert,
                            TimestampServer = timestampURL.Text
                        };
                    }
                }
            }


            using (BuildLog buildLog = new BuildLog(fileName, targetFileDialog.FileName, newApplicationData, verboseBuild.Checked))
            {
                buildLog.ShowDialog();
            }



        }

        private void loadIcon_Click(object sender, EventArgs e)
        {
            if (selectIconDialog.ShowDialog() == DialogResult.OK)
            {
                iconImage.ImageLocation = selectIconDialog.FileName;
                iconImage.Tag = selectIconDialog.FileName;
            }
        }

    }
}


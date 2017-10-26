namespace Sorlov.PowerShell.Builder
{
    partial class BuilderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuilderForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.frameworkVersion = new System.Windows.Forms.ComboBox();
            this.exe32svc = new System.Windows.Forms.RadioButton();
            this.exe64svc = new System.Windows.Forms.RadioButton();
            this.exe32 = new System.Windows.Forms.RadioButton();
            this.exe64 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.verboseBuild = new System.Windows.Forms.CheckBox();
            this.signFile = new System.Windows.Forms.CheckBox();
            this.buildRelease = new System.Windows.Forms.CheckBox();
            this.hideConsole = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.regionalSettings = new System.Windows.Forms.ComboBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.threadingMTA = new System.Windows.Forms.RadioButton();
            this.threadingSTA = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.versionBox = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.authorCompany = new System.Windows.Forms.TextBox();
            this.authorName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serviceDataFrame = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.serviceName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.serviceDescription = new System.Windows.Forms.TextBox();
            this.serviceDisplayName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.signBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.timestampURL = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.certificateSelector = new System.Windows.Forms.ComboBox();
            this.buildButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.selectIconDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.removeFile = new System.Windows.Forms.Button();
            this.addFile = new System.Windows.Forms.Button();
            this.addFiles = new System.Windows.Forms.ListBox();
            this.addFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.loadIcon = new System.Windows.Forms.Button();
            this.iconImage = new System.Windows.Forms.PictureBox();
            this.targetFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.serviceDataFrame.SuspendLayout();
            this.signBox.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "*.ps1";
            this.openFileDialog.Filter = "PowerShell Script|*.ps1";
            this.openFileDialog.Title = "Select PowerShell script..";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.frameworkVersion);
            this.groupBox2.Controls.Add(this.exe32svc);
            this.groupBox2.Controls.Add(this.exe64svc);
            this.groupBox2.Controls.Add(this.exe32);
            this.groupBox2.Controls.Add(this.exe64);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 109);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output target";
            // 
            // frameworkVersion
            // 
            this.frameworkVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameworkVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frameworkVersion.FormattingEnabled = true;
            this.frameworkVersion.Items.AddRange(new object[] {
            "Framework 2.0",
            "Framework 3.5",
            "Framework 4.0",
            "Framework 4.5"});
            this.frameworkVersion.Location = new System.Drawing.Point(6, 73);
            this.frameworkVersion.Name = "frameworkVersion";
            this.frameworkVersion.Size = new System.Drawing.Size(275, 21);
            this.frameworkVersion.TabIndex = 0;
            // 
            // exe32svc
            // 
            this.exe32svc.AutoSize = true;
            this.exe32svc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exe32svc.Location = new System.Drawing.Point(165, 44);
            this.exe32svc.Name = "exe32svc";
            this.exe32svc.Size = new System.Drawing.Size(88, 17);
            this.exe32svc.TabIndex = 3;
            this.exe32svc.Text = "32-bit service";
            this.exe32svc.UseVisualStyleBackColor = true;
            this.exe32svc.CheckedChanged += new System.EventHandler(this.exe32svc_CheckedChanged);
            // 
            // exe64svc
            // 
            this.exe64svc.AutoSize = true;
            this.exe64svc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exe64svc.Location = new System.Drawing.Point(16, 44);
            this.exe64svc.Name = "exe64svc";
            this.exe64svc.Size = new System.Drawing.Size(88, 17);
            this.exe64svc.TabIndex = 2;
            this.exe64svc.Text = "64-bit service";
            this.exe64svc.UseVisualStyleBackColor = true;
            this.exe64svc.CheckedChanged += new System.EventHandler(this.exe64svc_CheckedChanged);
            // 
            // exe32
            // 
            this.exe32.AutoSize = true;
            this.exe32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exe32.Location = new System.Drawing.Point(165, 21);
            this.exe32.Name = "exe32";
            this.exe32.Size = new System.Drawing.Size(106, 17);
            this.exe32.TabIndex = 1;
            this.exe32.Text = "32-bit executable";
            this.exe32.UseVisualStyleBackColor = true;
            this.exe32.CheckedChanged += new System.EventHandler(this.exe32_CheckedChanged);
            // 
            // exe64
            // 
            this.exe64.AutoSize = true;
            this.exe64.Checked = true;
            this.exe64.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exe64.Location = new System.Drawing.Point(16, 21);
            this.exe64.Name = "exe64";
            this.exe64.Size = new System.Drawing.Size(106, 17);
            this.exe64.TabIndex = 0;
            this.exe64.TabStop = true;
            this.exe64.Text = "64-bit executable";
            this.exe64.UseVisualStyleBackColor = true;
            this.exe64.CheckedChanged += new System.EventHandler(this.exe64_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.verboseBuild);
            this.groupBox4.Controls.Add(this.signFile);
            this.groupBox4.Controls.Add(this.buildRelease);
            this.groupBox4.Controls.Add(this.hideConsole);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(769, 366);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(157, 109);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "General options";
            // 
            // verboseBuild
            // 
            this.verboseBuild.AutoSize = true;
            this.verboseBuild.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verboseBuild.Location = new System.Drawing.Point(8, 83);
            this.verboseBuild.Name = "verboseBuild";
            this.verboseBuild.Size = new System.Drawing.Size(107, 17);
            this.verboseBuild.TabIndex = 16;
            this.verboseBuild.Text = "Detailed build log";
            this.verboseBuild.UseVisualStyleBackColor = true;
            // 
            // signFile
            // 
            this.signFile.AutoSize = true;
            this.signFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signFile.Location = new System.Drawing.Point(7, 62);
            this.signFile.Name = "signFile";
            this.signFile.Size = new System.Drawing.Size(102, 17);
            this.signFile.TabIndex = 15;
            this.signFile.Text = "Sign executable";
            this.signFile.UseVisualStyleBackColor = true;
            this.signFile.CheckedChanged += new System.EventHandler(this.signFile_CheckedChanged);
            // 
            // buildRelease
            // 
            this.buildRelease.AutoSize = true;
            this.buildRelease.Checked = true;
            this.buildRelease.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buildRelease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildRelease.Location = new System.Drawing.Point(7, 41);
            this.buildRelease.Name = "buildRelease";
            this.buildRelease.Size = new System.Drawing.Size(113, 17);
            this.buildRelease.TabIndex = 2;
            this.buildRelease.Text = "Build release code";
            this.buildRelease.UseVisualStyleBackColor = true;
            // 
            // hideConsole
            // 
            this.hideConsole.AutoSize = true;
            this.hideConsole.Checked = true;
            this.hideConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hideConsole.Location = new System.Drawing.Point(7, 20);
            this.hideConsole.Name = "hideConsole";
            this.hideConsole.Size = new System.Drawing.Size(143, 17);
            this.hideConsole.TabIndex = 0;
            this.hideConsole.Text = "Hide PowerShell window";
            this.hideConsole.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.regionalSettings);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(12, 127);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(288, 53);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Regional Settings";
            // 
            // regionalSettings
            // 
            this.regionalSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.regionalSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regionalSettings.FormattingEnabled = true;
            this.regionalSettings.Location = new System.Drawing.Point(7, 19);
            this.regionalSettings.Name = "regionalSettings";
            this.regionalSettings.Size = new System.Drawing.Size(275, 21);
            this.regionalSettings.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.threadingMTA);
            this.groupBox7.Controls.Add(this.threadingSTA);
            this.groupBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(769, 311);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(156, 49);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Threading";
            // 
            // threadingMTA
            // 
            this.threadingMTA.AutoSize = true;
            this.threadingMTA.Checked = true;
            this.threadingMTA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.threadingMTA.Location = new System.Drawing.Point(20, 23);
            this.threadingMTA.Name = "threadingMTA";
            this.threadingMTA.Size = new System.Drawing.Size(48, 17);
            this.threadingMTA.TabIndex = 5;
            this.threadingMTA.TabStop = true;
            this.threadingMTA.Text = "MTA";
            this.threadingMTA.UseVisualStyleBackColor = true;
            this.threadingMTA.CheckedChanged += new System.EventHandler(this.threadingMTA_CheckedChanged);
            // 
            // threadingSTA
            // 
            this.threadingSTA.AutoSize = true;
            this.threadingSTA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.threadingSTA.Location = new System.Drawing.Point(81, 23);
            this.threadingSTA.Name = "threadingSTA";
            this.threadingSTA.Size = new System.Drawing.Size(46, 17);
            this.threadingSTA.TabIndex = 4;
            this.threadingSTA.Text = "STA";
            this.threadingSTA.UseVisualStyleBackColor = true;
            this.threadingSTA.CheckedChanged += new System.EventHandler(this.threadingSTA_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.versionBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.authorCompany);
            this.groupBox1.Controls.Add(this.authorName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(306, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 168);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Metadata";
            // 
            // versionBox
            // 
            this.versionBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionBox.Location = new System.Drawing.Point(11, 132);
            this.versionBox.Mask = "#.#.#.#";
            this.versionBox.Name = "versionBox";
            this.versionBox.Size = new System.Drawing.Size(69, 20);
            this.versionBox.TabIndex = 6;
            this.versionBox.Text = "1000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Company";
            // 
            // authorCompany
            // 
            this.authorCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorCompany.Location = new System.Drawing.Point(11, 83);
            this.authorCompany.Name = "authorCompany";
            this.authorCompany.Size = new System.Drawing.Size(210, 20);
            this.authorCompany.TabIndex = 2;
            // 
            // authorName
            // 
            this.authorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorName.Location = new System.Drawing.Point(11, 38);
            this.authorName.Name = "authorName";
            this.authorName.Size = new System.Drawing.Size(210, 20);
            this.authorName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Author";
            // 
            // serviceDataFrame
            // 
            this.serviceDataFrame.Controls.Add(this.label4);
            this.serviceDataFrame.Controls.Add(this.serviceName);
            this.serviceDataFrame.Controls.Add(this.label5);
            this.serviceDataFrame.Controls.Add(this.serviceDescription);
            this.serviceDataFrame.Controls.Add(this.serviceDisplayName);
            this.serviceDataFrame.Controls.Add(this.label6);
            this.serviceDataFrame.Enabled = false;
            this.serviceDataFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceDataFrame.Location = new System.Drawing.Point(548, 12);
            this.serviceDataFrame.Name = "serviceDataFrame";
            this.serviceDataFrame.Size = new System.Drawing.Size(214, 168);
            this.serviceDataFrame.TabIndex = 9;
            this.serviceDataFrame.TabStop = false;
            this.serviceDataFrame.Text = "Service Data";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Service name";
            // 
            // serviceName
            // 
            this.serviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceName.Location = new System.Drawing.Point(11, 130);
            this.serviceName.Name = "serviceName";
            this.serviceName.Size = new System.Drawing.Size(193, 20);
            this.serviceName.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Description";
            // 
            // serviceDescription
            // 
            this.serviceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceDescription.Location = new System.Drawing.Point(11, 83);
            this.serviceDescription.Name = "serviceDescription";
            this.serviceDescription.Size = new System.Drawing.Size(193, 20);
            this.serviceDescription.TabIndex = 2;
            // 
            // serviceDisplayName
            // 
            this.serviceDisplayName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceDisplayName.Location = new System.Drawing.Point(11, 38);
            this.serviceDisplayName.Name = "serviceDisplayName";
            this.serviceDisplayName.Size = new System.Drawing.Size(193, 20);
            this.serviceDisplayName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Service display name";
            // 
            // signBox
            // 
            this.signBox.Controls.Add(this.label8);
            this.signBox.Controls.Add(this.timestampURL);
            this.signBox.Controls.Add(this.label7);
            this.signBox.Controls.Add(this.certificateSelector);
            this.signBox.Enabled = false;
            this.signBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signBox.Location = new System.Drawing.Point(13, 186);
            this.signBox.Name = "signBox";
            this.signBox.Size = new System.Drawing.Size(657, 119);
            this.signBox.TabIndex = 10;
            this.signBox.TabStop = false;
            this.signBox.Text = "Sign script";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Timestamp Server URL";
            // 
            // timestampURL
            // 
            this.timestampURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timestampURL.Location = new System.Drawing.Point(6, 85);
            this.timestampURL.Name = "timestampURL";
            this.timestampURL.Size = new System.Drawing.Size(645, 20);
            this.timestampURL.TabIndex = 2;
            this.timestampURL.Text = "http://timestamp.digicert.com";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Certificate";
            // 
            // certificateSelector
            // 
            this.certificateSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.certificateSelector.FormattingEnabled = true;
            this.certificateSelector.Location = new System.Drawing.Point(6, 39);
            this.certificateSelector.Name = "certificateSelector";
            this.certificateSelector.Size = new System.Drawing.Size(645, 21);
            this.certificateSelector.TabIndex = 0;
            // 
            // buildButton
            // 
            this.buildButton.Location = new System.Drawing.Point(807, 51);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(86, 33);
            this.buildButton.TabIndex = 11;
            this.buildButton.Text = "&Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(807, 94);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(86, 27);
            this.exitButton.TabIndex = 12;
            this.exitButton.Text = "&Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // selectIconDialog
            // 
            this.selectIconDialog.Filter = "Icon file|*.ico";
            this.selectIconDialog.Title = "Select icon file";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.removeFile);
            this.groupBox8.Controls.Add(this.addFile);
            this.groupBox8.Controls.Add(this.addFiles);
            this.groupBox8.Location = new System.Drawing.Point(13, 311);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(750, 164);
            this.groupBox8.TabIndex = 14;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Additional Files";
            // 
            // removeFile
            // 
            this.removeFile.Location = new System.Drawing.Point(663, 48);
            this.removeFile.Name = "removeFile";
            this.removeFile.Size = new System.Drawing.Size(75, 23);
            this.removeFile.TabIndex = 2;
            this.removeFile.Text = "Remove";
            this.removeFile.UseVisualStyleBackColor = true;
            this.removeFile.Click += new System.EventHandler(this.removeFile_Click);
            // 
            // addFile
            // 
            this.addFile.Location = new System.Drawing.Point(663, 19);
            this.addFile.Name = "addFile";
            this.addFile.Size = new System.Drawing.Size(75, 23);
            this.addFile.TabIndex = 1;
            this.addFile.Text = "Add";
            this.addFile.UseVisualStyleBackColor = true;
            this.addFile.Click += new System.EventHandler(this.addFile_Click);
            // 
            // addFiles
            // 
            this.addFiles.FormattingEnabled = true;
            this.addFiles.Location = new System.Drawing.Point(7, 20);
            this.addFiles.Name = "addFiles";
            this.addFiles.Size = new System.Drawing.Size(650, 134);
            this.addFiles.TabIndex = 0;
            // 
            // addFileDialog
            // 
            this.addFileDialog.Filter = "All files|*.*";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.loadIcon);
            this.groupBox3.Controls.Add(this.iconImage);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(677, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(85, 119);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Icon";
            // 
            // loadIcon
            // 
            this.loadIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadIcon.Location = new System.Drawing.Point(9, 77);
            this.loadIcon.Name = "loadIcon";
            this.loadIcon.Size = new System.Drawing.Size(69, 23);
            this.loadIcon.TabIndex = 4;
            this.loadIcon.Text = "Browse..";
            this.loadIcon.UseVisualStyleBackColor = true;
            this.loadIcon.Click += new System.EventHandler(this.loadIcon_Click);
            // 
            // iconImage
            // 
            this.iconImage.Image = global::Sorlov.PowerShell.Builder.Properties.Resources.exe;
            this.iconImage.Location = new System.Drawing.Point(28, 36);
            this.iconImage.Name = "iconImage";
            this.iconImage.Size = new System.Drawing.Size(32, 32);
            this.iconImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconImage.TabIndex = 3;
            this.iconImage.TabStop = false;
            // 
            // targetFileDialog
            // 
            this.targetFileDialog.DefaultExt = "*.exe";
            this.targetFileDialog.Filter = "Executable file|*.exe";
            this.targetFileDialog.Title = "Specify output file..";
            // 
            // BuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 484);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.serviceDataFrame);
            this.Controls.Add(this.buildButton);
            this.Controls.Add(this.signBox);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuilderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PowerExe Builder";
            this.Load += new System.EventHandler(this.BuilderForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.serviceDataFrame.ResumeLayout(false);
            this.serviceDataFrame.PerformLayout();
            this.signBox.ResumeLayout(false);
            this.signBox.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton exe32;
        private System.Windows.Forms.RadioButton exe64;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox hideConsole;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton threadingMTA;
        private System.Windows.Forms.RadioButton threadingSTA;
        private System.Windows.Forms.RadioButton exe32svc;
        private System.Windows.Forms.RadioButton exe64svc;
        private System.Windows.Forms.ComboBox frameworkVersion;
        private System.Windows.Forms.ComboBox regionalSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox buildRelease;
        private System.Windows.Forms.MaskedTextBox versionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox authorCompany;
        private System.Windows.Forms.TextBox authorName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox serviceDataFrame;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox serviceName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox serviceDescription;
        private System.Windows.Forms.TextBox serviceDisplayName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox signBox;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.OpenFileDialog selectIconDialog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox certificateSelector;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox timestampURL;
        private System.Windows.Forms.CheckBox signFile;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button removeFile;
        private System.Windows.Forms.Button addFile;
        private System.Windows.Forms.ListBox addFiles;
        private System.Windows.Forms.OpenFileDialog addFileDialog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button loadIcon;
        private System.Windows.Forms.PictureBox iconImage;
        private System.Windows.Forms.CheckBox verboseBuild;
        private System.Windows.Forms.SaveFileDialog targetFileDialog;
    }
}


#region LICENSE

// This software is licensed under the New BSD License.
//
// Copyright (c) 2012, Hiroaki SHIBUKI
// All rights reserved.
//
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Hiroaki SHIBUKI nor the names of its contributors may be
//   used to endorse or promote products derived from this software without specific
//   prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

#endregion

using System;
using System.Windows.Forms;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive.Controls
{
    public sealed partial class SignInDialog : Form
    {
        public SignInDialog()
        {
            InitializeComponent();

            this.progressBar1.Visible = false;
            this.progressBar1.Value = 0;

            this.panel1.Height = 0;
        }

        public WebBrowser WebBrowser
        {
            get { return this.webBrowser1; }
        }

#if DEBUG
        bool _Debug;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public bool Debug
        {
            get { return this._Debug; }
            set
            {
                this._Debug = value;

                this.webBrowser1.IsWebBrowserContextMenuEnabled = !this._Debug;
                this.panel1.Height = this._Debug ? 32 : 0;
            }
        }
#endif

        public static readonly Uri RedirectUri = new Uri("https://oauth.live.com/desktop");

        bool _CloseOnAuthError = true;

        public bool CloseOnAuthError
        {
            get { return this._CloseOnAuthError; }
            set
            {
                this._CloseOnAuthError = value;
                this.CloseOnAuthErrorChanged.Fire(this);
            }
        }

        public EventHandler CloseOnAuthErrorChanged;

        string _ClientId;

        public string ClientId
        {
            get { return this._ClientId; }
            set
            {
                this._ClientId = value;
                this.ClientIdChanged.Fire(this);
            }
        }

        public EventHandler ClientIdChanged;

        string[] _Scopes = new[] { "wl.signin" };

        public string[] Scopes
        {
            get { return this._Scopes; }
            set
            {
                this._Scopes = value;
                this.ScopesChanged.Fire(this);
            }
        }

        public EventHandler ScopesChanged;

        string _Locale;

        public string Locale
        {
            get { return this._Locale; }
            set
            {
                this._Locale = value;
                this.LocaleChanged.Fire(this);
            }
        }

        public EventHandler LocaleChanged;

        ResponseType _ResponseType = ResponseType.Token;

        public ResponseType ResponseType
        {
            get { return this._ResponseType; }
            set
            {
                this._ResponseType = value;
                this.ResponseTypeChanged.Fire(this);
            }
        }

        public EventHandler ResponseTypeChanged;

        LiveConnectSession _Session;

        public LiveConnectSession Session
        {
            get { return this._Session; }
            set
            {
                this._Session = value;
                this.SessionChanged.Fire(this);
            }
        }

        public EventHandler SessionChanged;

        public Exception Error { get; private set; }

        void LiveAuthDialog_Load(object sender, EventArgs e)
        {
            var url = LiveAuthHelper.Request.Create(this.ClientId, this.Scopes, SignInDialog.RedirectUri, this.Locale);

            this.webBrowser1.Navigate(url);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(this.textBox1.Text);
        }

        private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.progressBar1.Visible = true;
            this.progressBar1.Value = 0;
        }

        private void WebBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            this.progressBar1.Value = (int)((e.MaximumProgress > 0) ? (e.CurrentProgress / e.MaximumProgress) : 0);
        }

        void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.progressBar1.Visible = false;
            this.progressBar1.Value = 0;

            var uri = e.Url;

            this.textBox1.Text = uri.AbsoluteUri;

            try
            {
                var session = default(LiveConnectSession);

                if (LiveAuthHelper.Response.TryParse(uri, SignInDialog.RedirectUri, out session))
                {
                    this.Session = session;
                    this.timer1.Start();
                }
            }
            catch (Exception x)
            {
                this.Error = x;

                if (this.CloseOnAuthError)
                {
                    this.timer1.Start();
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.Session != null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}

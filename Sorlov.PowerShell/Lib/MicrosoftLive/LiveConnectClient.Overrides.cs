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
using System.Collections.Generic;
using System.IO;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
	partial class LiveConnectClient
	{
		public EventHandler<LiveOperationCompletedEventArgs> CopyCompleted;

		public void CopyAsync(string path, string destination)
		{
			this.CopyAsync(path, destination, null);
		}
		
        public void CopyAsync(string path, string destination, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(destination, "destination");

            var address = this.CreateAddress(path);
            var method = Methods.Copy;
            var data = new Dictionary<string, object> { { "destination", destination } }.ToJsonString();

            var completed = this.CopyCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginCopy(string path, string destination)
		{
			return this.BeginCopy(path, destination, null, null);
		}
		
		public IAsyncResult BeginCopy(string path, string destination, AsyncCallback asyncCallback)
		{
			return this.BeginCopy(path, destination, asyncCallback, null);
		}
		
		public IAsyncResult BeginCopy(string path, string destination, object userState)
		{
			return this.BeginCopy(path, destination, null, userState);
		}
		
        public IAsyncResult BeginCopy(string path, string destination, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(destination, "destination");

            var address = this.CreateAddress(path);
            var method = Methods.Copy;
            var data = new Dictionary<string, object> { { "destination", destination } }.ToJsonString();

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelCopy(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndCopy(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public EventHandler<LiveOperationCompletedEventArgs> DeleteCompleted;

		public void DeleteAsync(string path)
		{
			this.DeleteAsync(path, null);
		}
		
        public void DeleteAsync(string path, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ;

            var address = this.CreateAddress(path);
            var method = Methods.Delete;
            var data = string.Empty;

            var completed = this.DeleteCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginDelete(string path)
		{
			return this.BeginDelete(path, null, null);
		}
		
		public IAsyncResult BeginDelete(string path, AsyncCallback asyncCallback)
		{
			return this.BeginDelete(path, asyncCallback, null);
		}
		
		public IAsyncResult BeginDelete(string path, object userState)
		{
			return this.BeginDelete(path, null, userState);
		}
		
        public IAsyncResult BeginDelete(string path, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ;

            var address = this.CreateAddress(path);
            var method = Methods.Delete;
            var data = string.Empty;

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelDelete(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndDelete(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public EventHandler<LiveOperationCompletedEventArgs> GetCompleted;

		public void GetAsync(string path)
		{
			this.GetAsync(path, null);
		}
		
        public void GetAsync(string path, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ;

            var address = this.CreateAddress(path);
            var method = Methods.Get;
            var data = string.Empty;

            var completed = this.GetCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginGet(string path)
		{
			return this.BeginGet(path, null, null);
		}
		
		public IAsyncResult BeginGet(string path, AsyncCallback asyncCallback)
		{
			return this.BeginGet(path, asyncCallback, null);
		}
		
		public IAsyncResult BeginGet(string path, object userState)
		{
			return this.BeginGet(path, null, userState);
		}
		
        public IAsyncResult BeginGet(string path, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ;

            var address = this.CreateAddress(path);
            var method = Methods.Get;
            var data = string.Empty;

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelGet(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndGet(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public EventHandler<LiveOperationCompletedEventArgs> MoveCompleted;

		public void MoveAsync(string path, string destination)
		{
			this.MoveAsync(path, destination, null);
		}
		
        public void MoveAsync(string path, string destination, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(destination, "destination");

            var address = this.CreateAddress(path);
            var method = Methods.Move;
            var data = new Dictionary<string, object> { { "destination", destination } }.ToJsonString();

            var completed = this.MoveCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginMove(string path, string destination)
		{
			return this.BeginMove(path, destination, null, null);
		}
		
		public IAsyncResult BeginMove(string path, string destination, AsyncCallback asyncCallback)
		{
			return this.BeginMove(path, destination, asyncCallback, null);
		}
		
		public IAsyncResult BeginMove(string path, string destination, object userState)
		{
			return this.BeginMove(path, destination, null, userState);
		}
		
        public IAsyncResult BeginMove(string path, string destination, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(destination, "destination");

            var address = this.CreateAddress(path);
            var method = Methods.Move;
            var data = new Dictionary<string, object> { { "destination", destination } }.ToJsonString();

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelMove(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndMove(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public EventHandler<LiveOperationCompletedEventArgs> PostCompleted;

		public void PostAsync(string path, IDictionary<string, object> body)
		{
			this.PostAsync(path, body, null);
		}
		
        public void PostAsync(string path, IDictionary<string, object> body, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Post;
            var data = body.ToJsonString();

            var completed = this.PostCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginPost(string path, IDictionary<string, object> body)
		{
			return this.BeginPost(path, body, null, null);
		}
		
		public IAsyncResult BeginPost(string path, IDictionary<string, object> body, AsyncCallback asyncCallback)
		{
			return this.BeginPost(path, body, asyncCallback, null);
		}
		
		public IAsyncResult BeginPost(string path, IDictionary<string, object> body, object userState)
		{
			return this.BeginPost(path, body, null, userState);
		}
		
        public IAsyncResult BeginPost(string path, IDictionary<string, object> body, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Post;
            var data = body.ToJsonString();

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelPost(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndPost(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public void PostAsync(string path, string body)
		{
			this.PostAsync(path, body, null);
		}
		
        public void PostAsync(string path, string body, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Post;
            var data = body;

            var completed = this.PostCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginPost(string path, string body)
		{
			return this.BeginPost(path, body, null, null);
		}
		
		public IAsyncResult BeginPost(string path, string body, AsyncCallback asyncCallback)
		{
			return this.BeginPost(path, body, asyncCallback, null);
		}
		
		public IAsyncResult BeginPost(string path, string body, object userState)
		{
			return this.BeginPost(path, body, null, userState);
		}
		
        public IAsyncResult BeginPost(string path, string body, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Post;
            var data = body;

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public EventHandler<LiveOperationCompletedEventArgs> PutCompleted;

		public void PutAsync(string path, IDictionary<string, object> body)
		{
			this.PutAsync(path, body, null);
		}
		
        public void PutAsync(string path, IDictionary<string, object> body, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Put;
            var data = body.ToJsonString();

            var completed = this.PutCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginPut(string path, IDictionary<string, object> body)
		{
			return this.BeginPut(path, body, null, null);
		}
		
		public IAsyncResult BeginPut(string path, IDictionary<string, object> body, AsyncCallback asyncCallback)
		{
			return this.BeginPut(path, body, asyncCallback, null);
		}
		
		public IAsyncResult BeginPut(string path, IDictionary<string, object> body, object userState)
		{
			return this.BeginPut(path, body, null, userState);
		}
		
        public IAsyncResult BeginPut(string path, IDictionary<string, object> body, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Put;
            var data = body.ToJsonString();

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public void CancelPut(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndPut(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public void PutAsync(string path, string body)
		{
			this.PutAsync(path, body, null);
		}
		
        public void PutAsync(string path, string body, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Put;
            var data = body;

            var completed = this.PutCompleted;

            this.UploadStringAsync(address, method, data, completed, userState);
        }
		
		public IAsyncResult BeginPut(string path, string body)
		{
			return this.BeginPut(path, body, null, null);
		}
		
		public IAsyncResult BeginPut(string path, string body, AsyncCallback asyncCallback)
		{
			return this.BeginPut(path, body, asyncCallback, null);
		}
		
		public IAsyncResult BeginPut(string path, string body, object userState)
		{
			return this.BeginPut(path, body, null, userState);
		}
		
        public IAsyncResult BeginPut(string path, string body, AsyncCallback asyncCallback, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(body, "body");

            var address = this.CreateAddress(path);
            var method = Methods.Put;
            var data = body;

            return this.BeginUploadString(address, method, data, asyncCallback, userState);
        }

		public EventHandler<LiveDownloadCompletedEventArgs> DownloadCompleted;

		public EventHandler<LiveDownloadProgressChangedEventArgs> DownloadProgressChanged;

		public void DownloadAsync(string path)
		{
			this.DownloadAsync(path, null);
		}
		
		public IAsyncResult BeginDownload(string path)
		{
			return this.BeginDownload(path, null, null, null);
		}
		
		public IAsyncResult BeginDownload(string path, AsyncCallback asyncCallback)
		{
			return this.BeginDownload(path, asyncCallback, null, null);
		}
		
		public IAsyncResult BeginDownload(string path, object userState)
		{
			return this.BeginDownload(path, null, null, userState);
		}
		
		public IAsyncResult BeginDownload(string path, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginDownload(path, null, progress, null);
		}
		
		public IAsyncResult BeginDownload(string path, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginDownload(path, asyncCallback, progress, null);
		}
		
		public IAsyncResult BeginDownload(string path, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
		{
			return this.BeginDownload(path, null, progress, userState);
		}
		
		public void CancelDownload(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveDownloadResult>(asyncResult);
		}

		public LiveDownloadResult EndDownload(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveDownloadResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public EventHandler<LiveOperationCompletedEventArgs> UploadCompleted;

		public EventHandler<LiveUploadProgressChangedEventArgs> UploadProgressChanged;

		public void UploadAsync(string path, string fileName, Stream inputStream)
		{
			this.UploadAsync(path, fileName, inputStream, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream)
		{
			return this.BeginUpload(path, fileName, inputStream, null, null, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, AsyncCallback asyncCallback)
		{
			return this.BeginUpload(path, fileName, inputStream, asyncCallback, null, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, null, null, userState);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginUpload(path, fileName, inputStream, null, progress, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginUpload(path, fileName, inputStream, asyncCallback, progress, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, null, progress, userState);
		}
		
		public void CancelUpload(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            this.Cancel<LiveOperationResult>(asyncResult);
		}

		public LiveOperationResult EndUpload(IAsyncResult asyncResult)
		{
            ThrowArgumentException.IfNull(asyncResult, "asyncResult");

            var ar = asyncResult as LiveConnectClientAsyncResult<LiveOperationResult>;

            ThrowArgumentException.IfOutOfRange(ar == null, "asyncResult");

            return this.End(ar);
		}

		public void UploadAsync(string path, string fileName, Stream inputStream, bool overwiteExisting)
		{
			this.UploadAsync(path, fileName, inputStream, overwiteExisting, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, null, null, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting, AsyncCallback asyncCallback)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, asyncCallback, null, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, null, null, userState);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, null, progress, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, asyncCallback, progress, null);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwiteExisting, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, overwiteExisting, null, progress, userState);
		}
		
		public void UploadAsync(string path, string fileName, Stream inputStream, object state)
		{
			 this.UploadAsync(path, fileName, inputStream, false, state);
		}
		
		public IAsyncResult BeginDownload(string path, AsyncCallback asyncCallback, object userState)
		{
			return this.BeginDownload(path, asyncCallback, null, userState);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, AsyncCallback asyncCallback, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, asyncCallback, null, userState);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, false, asyncCallback, progress, userState);
		}
		
		public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwriteExisting, AsyncCallback asyncCallback, object userState)
		{
			return this.BeginUpload(path, fileName, inputStream, overwriteExisting, asyncCallback, null, userState);
		}
		
	}
}

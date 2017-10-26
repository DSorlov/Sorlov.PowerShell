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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Mjollnir;
using Mjollnir.IO;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public sealed partial class LiveConnectClient
    {
        static class Keys
        {
            public const string Error = "error";

            public const string ErrorDescription = "error_description";

            public const string OverriteExisting = "overwriteExisting";

            public const string AccessToken = "access_token";
        }

        static class Methods
        {
            public const string Copy = "COPY";

            public const string Delete = "DELETE";

            public const string Get = WebRequestMethods.Http.Get;

            public const string Move = "MOVE";

            public const string Post = WebRequestMethods.Http.Post;

            public const string Put = WebRequestMethods.Http.Put;
        }

        static class ContentTypes
        {
            public const string Json = "application/json";

            public const string Multipart = "multipart/form-data; boundary={0}";
        }

        public LiveConnectClient(LiveConnectSession session)
            : this(session, SynchronizationContext.Current)
        {
            // nothing to do.
        }

        public LiveConnectClient(LiveConnectSession session, SynchronizationContext synchronizationContext)
        {
            ThrowArgumentException.IfNull(session, "session");

            this.Session = session;
            this.SynchronizationContext = synchronizationContext;
        }

        public static readonly Uri EndPoint = new Uri("https://apis.live.net/v5.0");

        object syncRoot = new object();

        public LiveConnectSession Session { get; internal set; }

        public SynchronizationContext SynchronizationContext { get; private set; }

        public void DownloadAsync(string path, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");

            var address = this.CreateAddress(path);
            var method = Methods.Get;

            var completed = this.DownloadCompleted;
            var progressChanged = this.DownloadProgressChanged;

            this.DownloadStreamAsync(address, method, completed, progressChanged, userState);
        }

        public IAsyncResult BeginDownload(string path, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");

            var address = this.CreateAddress(path);
            var method = Methods.Get;

            return this.BeginDownloadStream(address, method, asyncCallback, progress, userState);
        }

        public void UploadAsync(string path, string fileName, Stream inputStream, bool overwriteExisting, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(fileName, "fileName");
            ThrowArgumentException.IfNull(inputStream, "inputStream");

            var address = this.CreateAddress(path);

            if (overwriteExisting)
            {
                address = QueryParameters.Append(address, "overwrite", "true");
            }

            var method = Methods.Post;

            var completed = this.UploadCompleted;
            var progressChanged = this.UploadProgressChanged;

            this.UploadStreamAsync(address, method, fileName, inputStream, completed, progressChanged, userState);
        }

        public IAsyncResult BeginUpload(string path, string fileName, Stream inputStream, bool overwriteExisting, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
        {
            ThrowArgumentException.IfNull(path, "path");
            ThrowArgumentException.IfNull(fileName, "fileName");
            ThrowArgumentException.IfNull(inputStream, "inputStream");

            var address = this.CreateAddress(path);

            if (overwriteExisting)
            {
                address = QueryParameters.Append(address, "overwrite", "true");
            }

            var method = Methods.Post;

            return this.BeginUploadStream(address, method, fileName, inputStream, asyncCallback, progress, userState);
        }

        string CreateAddress(string path)
        {
            var address = string.Format("{0}{1}{2}", LiveConnectClient.EndPoint, (path[0] == '/' ? string.Empty : "/"), path);

            address = QueryParameters.Append(address, Keys.AccessToken, this.Session.AccessToken);

            return address;
        }

        static string DownloadString(HttpWebRequest request, Action<long> reportProgress)
        {
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                using (var outputStream = new MemoryStream())
                {
                    responseStream.CopyTo(outputStream, reportProgress);

                    // TODO: response.ContentType から Encoding を抽出します。
                    var encoding = Encoding.UTF8;

                    var bytes = outputStream.ToArray();

                    return encoding.GetString(bytes);
                }
            }
        }

        void Cancel<TResult>(IAsyncResult asyncResult)
        {
            var ar = (LiveConnectClientAsyncResult<TResult>)asyncResult;

            ar.Cancel();
        }

        TResult End<TResult>(LiveConnectClientAsyncResult<TResult> ar)
        {
            ar.AsyncWaitHandle.WaitOne();
            ar.AsyncWaitHandle.Close();

            return ar.Result;
        }

        void UploadStringAsync(string address, string method, string body, EventHandler<LiveOperationCompletedEventArgs> completed, object userState)
        {
            var asyncCallback = (AsyncCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveOperationResult>)state;

                var e = (ar.Result.Error == null && !ar.Result.Cancelled)
                    ? new LiveOperationCompletedEventArgs(ar.Result.Result, ar.Result.RawResult, ar.AsyncState)
                    : new LiveOperationCompletedEventArgs(ar.Result.Error, ar.Result.Cancelled, ar.AsyncState);

                completed.Fire(this, e);
            });

            this.BeginUploadString(address, method, body, asyncCallback, userState);
        }

        LiveConnectClientAsyncResult<LiveOperationResult> BeginUploadString(string address, string method, string data, AsyncCallback asyncCallback, object userState)
        {
            var asyncResult = new LiveConnectClientAsyncResult<LiveOperationResult>(userState, this.SynchronizationContext);

            var task = (WaitCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveOperationResult>)state;

                ar.CompletedSynchronously = false;

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(address);

                    request.Method = method;

                    if (!string.IsNullOrEmpty(data))
                    {
                        request.ContentType = ContentTypes.Json;

                        using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                        using (var requestStream = request.GetRequestStream())
                        {
                            inputStream.CopyTo(requestStream, bytesTranferrd => ar.ThrowIfCancellationRequested());
                        }
                    }

                    var rawResult = LiveConnectClient.DownloadString(request, bytesTransferred => ar.ThrowIfCancellationRequested());

                    var result = string.IsNullOrEmpty(rawResult)
                        ? new Dictionary<string, object>()
                        : Json.Parse(rawResult);

                    if (result.ContainsKey(Keys.Error))
                    {
                        var errorCode = (string)result[Keys.Error];
                        var message = (string)result[Keys.ErrorDescription];
                        var error = new LiveConnectException(errorCode, message);

                        ar.Result = new LiveOperationResult(error, false);
                    }
                    else
                    {
                        ar.Result = new LiveOperationResult(result, rawResult);
                    }
                }
                catch (Exception error)
                {
                    ar.Result = new LiveOperationResult(error, error is OperationCanceledException);
                }
                finally
                {
                    ar.IsCompleted = true;

                    if (asyncCallback != null)
                    {
                        ar.Sync(() => asyncCallback(ar));
                    }
                }
            });

            if (!ThreadPool.QueueUserWorkItem(task, asyncResult)) throw new OutOfMemoryException();

            return asyncResult;
        }

        void DownloadStreamAsync(string address, string method, EventHandler<LiveDownloadCompletedEventArgs> completed, EventHandler<LiveDownloadProgressChangedEventArgs> progress, object userState)
        {
            var asyncCallback = (AsyncCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveDownloadResult>)state;

                var e = (ar.Result.Error == null && !ar.Result.Cancelled)
                    ? new LiveDownloadCompletedEventArgs(ar.Result.Result, ar.AsyncState)
                    : new LiveDownloadCompletedEventArgs(ar.Result.Error, ar.Result.Cancelled, ar.AsyncState);

                completed.Fire(this, e);
            });

            var progressCallback = (Action<IAsyncResult, LiveOperationProgress>)((ar, report) =>
            {
                var e = new LiveDownloadProgressChangedEventArgs(report.BytesTransferred, report.TotalBytes, userState);

                progress.Fire(this, e);
            });

            this.BeginDownloadStream(address, method, asyncCallback, progressCallback, userState);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:スコープを失う前にオブジェクトを破棄")]
        LiveConnectClientAsyncResult<LiveDownloadResult> BeginDownloadStream(string address, string method, AsyncCallback asyncCallback, Action<IAsyncResult, LiveOperationProgress> progressCallback, object userState)
        {
            var asyncResult = new LiveConnectClientAsyncResult<LiveDownloadResult>(userState, this.SynchronizationContext);

            var task = (WaitCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveDownloadResult>)state;

                ar.CompletedSynchronously = false;

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(address);

                    request.Method = method;

                    using (var response = request.GetResponse())
                    using (var responseStream = response.GetResponseStream())
                    {
                        ar.ThrowIfCancellationRequested();

                        // NOTE:
                        // responseStream を直接返すべきと思ったけど、サーバ応答を MemoryStream に全部読み込んでから
                        // Seek(0, SeekOrigin.Begin) せずに返すのが本家 Live SDK の DownloadAsync() の仕様らしいので、
                        // それに合わせてます。

                        var outputStream = new MemoryStream();

                        var contentLength = (response.ContentLength < 1) ? -1 : response.ContentLength;

                        ar.Sync(() =>
                        {
                            if (progressCallback != null)
                            {
                                ar.Sync(() => progressCallback(ar, new LiveOperationProgress(0, contentLength)));
                            }
                        });

                        responseStream.CopyTo(outputStream, bytesTransferred =>
                        {
                            ar.ThrowIfCancellationRequested();

                            if (progressCallback != null)
                            {
                                ar.Sync(() => progressCallback(ar, new LiveOperationProgress(bytesTransferred, contentLength)));
                            }
                        });

                        ar.Result = new LiveDownloadResult(outputStream);
                    }
                }
                catch (Exception error)
                {
                    ar.Result = new LiveDownloadResult(error, error is OperationCanceledException);
                }
                finally
                {
                    ar.IsCompleted = true;

                    if (asyncCallback != null)
                    {
                        ar.Sync(() => asyncCallback(ar));
                    }
                }
            });

            if (!ThreadPool.QueueUserWorkItem(task, asyncResult)) throw new OutOfMemoryException();

            return asyncResult;
        }

        void UploadStreamAsync(string address, string method, string fileName, Stream inputStream, EventHandler<LiveOperationCompletedEventArgs> completed, EventHandler<LiveUploadProgressChangedEventArgs> progress, object userState)
        {
            var asyncCallback = (AsyncCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveOperationResult>)state;

                var e = (ar.Result.Error == null && !ar.Result.Cancelled)
                    ? new LiveOperationCompletedEventArgs(ar.Result.Result, ar.Result.RawResult, ar.AsyncState)
                    : new LiveOperationCompletedEventArgs(ar.Result.Error, ar.Result.Cancelled, ar.AsyncState);

                completed.Fire(this, e);
            });

            var progressCallback = (Action<IAsyncResult, LiveOperationProgress>)((ar, report) =>
            {
                var e = new LiveUploadProgressChangedEventArgs(report.BytesTransferred, report.TotalBytes, userState);

                progress.Fire(this, e);
            });

            this.BeginUploadStream(address, method, fileName, inputStream, asyncCallback, progressCallback, userState);
        }

        IAsyncResult BeginUploadStream(string address, string method, string fileName, Stream inputStream, AsyncCallback completed, Action<IAsyncResult, LiveOperationProgress> progress, object userState)
        {
            var asyncResult = new LiveConnectClientAsyncResult<LiveOperationResult>(userState, this.SynchronizationContext);

            var task = (WaitCallback)(state =>
            {
                var ar = (LiveConnectClientAsyncResult<LiveOperationResult>)state;

                ar.CompletedSynchronously = false;

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(address);

                    request.Method = method;

                    if (inputStream != null)
                    {
                        var boundary = "AAA" + string.Join(string.Empty, Guid.NewGuid().ToByteArray().Select(_ => _.ToString("X")).ToArray()) + "AAA";

                        var head = StringFactory.Create(sb =>
                        {
                            sb.AppendLine(string.Format(@"--{0}", boundary));
                            sb.AppendLine(string.Format(@"Content-Disposition: form-data; name=""file""; filename=""{0}""", fileName));
                            sb.AppendLine(string.Format(@"Content-Type: application/octet-stream"));
                            sb.AppendLine();
                        });

                        var tail = StringFactory.Create(sb =>
                        {
                            sb.AppendLine();
                            sb.AppendLine(string.Format(@"--{0}--", boundary));
                        });

                        request.ContentType = string.Format(ContentTypes.Multipart, boundary);

                        using (var requestStream = request.GetRequestStream())
                        {
                            using (var stream = Encoding.UTF8.GetBytes(head).AsStream())
                            {
                                stream.CopyTo(requestStream);
                            }

                            var contentLength = -1;
                            var totalBytes = 0L;

                            ar.Sync(() =>
                            {
                                if (progress != null)
                                {
                                    ar.Sync(() => progress(ar, new LiveOperationProgress(totalBytes, contentLength)));
                                }
                            });

                            totalBytes = inputStream.CopyTo(requestStream, bytesTransferred =>
                            {
                                ar.ThrowIfCancellationRequested();

                                if (progress != null)
                                {
                                    ar.Sync(() => progress(ar, new LiveOperationProgress(bytesTransferred, contentLength)));
                                }
                            });

                            if (progress != null)
                            {
                                ar.Sync(() => progress(ar, new LiveOperationProgress(totalBytes, totalBytes)));
                            }

                            using (var stream = Encoding.UTF8.GetBytes(tail).AsStream())
                            {
                                stream.CopyTo(requestStream);
                            }
                        }
                    }

                    var rawResult = LiveConnectClient.DownloadString(request, bytesTransfeerd => ar.ThrowIfCancellationRequested());

                    var result = string.IsNullOrEmpty(rawResult) ? new Dictionary<string, object>() : Json.Parse(rawResult);

                    if (result.ContainsKey(Keys.Error))
                    {
                        var errorCode = (string)result[Keys.Error];
                        var message = (string)result[Keys.ErrorDescription];
                        var error = new LiveConnectException(errorCode, message);

                        ar.Result = new LiveOperationResult(error, false);
                    }
                    else
                    {
                        ar.Result = new LiveOperationResult(result, rawResult);
                    }
                }
                catch (Exception error)
                {
                    ar.Result = new LiveOperationResult(error, error is OperationCanceledException);
                }
                finally
                {
                    ar.IsCompleted = true;

                    if (completed != null)
                    {
                        ar.Sync(() => completed(ar));
                    }
                }
            });

            if (!ThreadPool.QueueUserWorkItem(task, asyncResult)) throw new OutOfMemoryException();

            return asyncResult;
        }
    }
}

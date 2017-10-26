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
using Mjollnir;
using Mjollnir.Collections;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public sealed class LiveAuthHelper
    {
        static class Keys
        {
            public const string ResponseType = "response_type";

            public const string ClientId = "client_id";

            public const string Scope = "scope";

            public const string Locale = "locale";

            public const string RedirectUri = "redirect_uri";

            public const string AccessToken = "access_token";

            public const string AuthenticationToken = "authentication_token";

            public const string Expires = "expires_in";

            public const string RefreshToken = "refresh_token";

            public const string Error = "error";

            public const string ErrorDescription = "error_description";
        }

        public static readonly Uri EndPoint = new Uri("https://oauth.live.com/authorize");

        public static class Request
        {
            public static Uri Create(string clientId, string[] scopes, Uri redirectUri)
            {
                return LiveAuthHelper.Request.Create(clientId, scopes, redirectUri, null);
            }

            public static Uri Create(string clientId, string[] scopes, Uri redirectUri, string locale)
            {
                ThrowArgumentException.IfNull(clientId, "clientId");
                ThrowArgumentException.IfNull(scopes, "scopes");
                ThrowArgumentException.IfNull(redirectUri, "redirectUri");

                var uri = LiveAuthHelper.EndPoint.ToString();

                uri = QueryParameters.Append(uri, Keys.ResponseType, "token");
                uri = QueryParameters.Append(uri, Keys.ClientId, clientId);
                uri = QueryParameters.Append(uri, Keys.Scope, string.Join(" ", scopes));

                if (!string.IsNullOrEmpty(locale))
                {
                    uri = QueryParameters.Append(uri, Keys.Locale, locale);
                }

                uri = QueryParameters.Append(uri, Keys.RedirectUri, redirectUri.ToString());

                return new Uri(uri);
            }
        }

        public static class Response
        {
            public static bool TryParse(Uri uri, Uri redirectUri, out LiveConnectSession session)
            {
                ThrowArgumentException.IfNull(uri, "uri");
                ThrowArgumentException.IfNull(redirectUri, "redirectUri");

                if (uri.Scheme == LiveAuthHelper.EndPoint.Scheme && uri.Host == LiveAuthHelper.EndPoint.Host && uri.Port == LiveAuthHelper.EndPoint.Port && uri.AbsolutePath == "/error")
                {
                    var @params = QueryParameters.Parse(uri.Query.Substring(1));

                    var errorCode = @params.GetValueOrDefault(Keys.Error);
                    var message = @params.GetValueOrDefault(Keys.ErrorDescription);

                    throw new LiveAuthException(errorCode, message);
                }

                if (uri.Scheme == redirectUri.Scheme && uri.Host == redirectUri.Host && uri.Port == redirectUri.Port && uri.AbsolutePath == redirectUri.AbsolutePath)
                {
                    var @params = QueryParameters.Parse(uri.Fragment.Substring(1));

                    if (@params.ContainsKey("error"))
                    {
                        var errorCode = @params.GetValueOrDefault(Keys.Error);
                        var message = @params.GetValueOrDefault(Keys.ErrorDescription);

                        throw new LiveAuthException(errorCode, message);
                    }

                    var _session = new LiveConnectSession();

                    @params.InvokeIfContainsKey(Keys.AccessToken, _ => { _session.AccessToken = _; });
                    @params.InvokeIfContainsKey(Keys.AuthenticationToken, _ => { _session.AuthenticationToken = _; });
                    @params.InvokeIfContainsKey(Keys.Expires, _ => { _session.Expires = new DateTimeOffset().AddSeconds(int.Parse(_)); });
                    @params.InvokeIfContainsKey(Keys.RefreshToken, _ => { _session.RefreshToken = _; });
                    @params.InvokeIfContainsKey(Keys.Scope, _ => { _session.Scopes = _.Split(' '); });

                    session = _session;
                    return true;
                }

                session = null;
                return false;
            }
        }
    }
}

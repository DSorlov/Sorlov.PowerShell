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

using System.Collections.Generic;
using System.Linq;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    static class QueryParameters
    {
        public static string Append(string source, string key, string value)
        {
            ThrowArgumentException.IfNull(source, "source");
            ThrowArgumentException.IfNull(key, "key");
            ThrowArgumentException.IfNull(value, "value");

            ThrowArgumentException.IfOutOfRange(string.IsNullOrEmpty(key), "key");

            return source + (source.Contains("?") ? "&" : "?") + string.Format("{0}={1}", key, System.Web.HttpUtility.UrlEncode(value));
        }

        public static IDictionary<string, string> Parse(string source)
        {
            ThrowArgumentException.IfNull(source, "source");

            int index1 = source.IndexOf('?');

            if (index1 >= 0)
            {
                source = source.Substring(index1 + 1);
            }

            return source.Split('&').Select(_ => _.Split('=')).Select(_ => new { Key = _[0], Vale = System.Web.HttpUtility.UrlDecode(_[1]) }).ToDictionary(_ => _.Key, _ => _.Vale);
        }
    }
}

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
using System.IO;

namespace Mjollnir.IO
{
    static partial class StreamExtensions
    {
        const int CopyToDefaultBufferSize = 4096;

        public static long CopyTo(this Stream source, Stream destination)
        {
            return CopyTo(source, destination, CopyToDefaultBufferSize, _ => { });
        }

        public static long CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            return CopyTo(source, destination, bufferSize, _ => { });
        }

        public static long CopyTo(this Stream source, Stream destination, Action<long> reportProgress)
        {
            return CopyTo(source, destination, CopyToDefaultBufferSize, reportProgress);
        }

        public static long CopyTo(this Stream source, Stream destination, int bufferSize, Action<long> reportProgress)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (reportProgress == null) throw new ArgumentNullException("reportProgress");

            var buffer = new byte[bufferSize];

            var transferredBytes = 0L;

            for (var bytesRead = source.Read(buffer, 0, buffer.Length); bytesRead > 0; bytesRead = source.Read(buffer, 0, buffer.Length))
            {
                transferredBytes += bytesRead;
                reportProgress(transferredBytes);

                destination.Write(buffer, 0, bytesRead);
            }

            destination.Flush();

            return transferredBytes;
        }
    }
}

﻿using System;
using System.Runtime.Serialization;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Exceptions
{
    /// <summary>
    /// Thrown if an operation name was already defined when performing a late binding call over an instance.
    /// </summary>
    [Serializable]
    public class AlreadyDefinedOperationNameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyDefinedOperationNameException"/> class.
        /// </summary>
        public AlreadyDefinedOperationNameException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyDefinedOperationNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AlreadyDefinedOperationNameException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyDefinedOperationNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AlreadyDefinedOperationNameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyDefinedOperationNameException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected AlreadyDefinedOperationNameException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

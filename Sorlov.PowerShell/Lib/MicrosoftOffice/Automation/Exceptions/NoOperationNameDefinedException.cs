using System;
using System.Runtime.Serialization;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Exceptions
{
    /// <summary>
    /// Thrown if an operation name is not specified when performing a late binding call.
    /// </summary>
    [Serializable]
    public class NoOperationNameDefinedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoOperationNameDefinedException"/> class.
        /// </summary>
        public NoOperationNameDefinedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOperationNameDefinedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NoOperationNameDefinedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOperationNameDefinedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NoOperationNameDefinedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOperationNameDefinedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected NoOperationNameDefinedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

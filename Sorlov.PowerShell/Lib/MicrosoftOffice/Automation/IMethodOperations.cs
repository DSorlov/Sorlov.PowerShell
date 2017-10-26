using System;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Provides an interface which will allow adding parameters to a method defined
    /// by a <see cref="IMethodCall"/>,  and invoke that method
    /// </summary>
    public interface IMethodOperations
    {
        /// <summary>
        /// Adds a parameter to the method call, passed with value semantics
        /// </summary>
        /// <param name="value">object to be passed as parameter</param>
        /// <returns>
        /// A reference to the object which made this operation
        /// </returns>
        IMethodOperations AddParameter(object value);

        /// <summary>
        /// Adds a parameter to the method call, passed with reference semantics
        /// </summary>
        /// <param name="value">object to be passed as parameter</param>
        /// <returns>
        /// A reference to the <see cref="IMethodOperations"/> instance which called this method.
        /// </returns>
        IMethodOperations AddRefParameter(object value);

        /// <summary>
        /// Add a specific number of instances for System.Type.Missing parameters to the call.
        /// Usefull for interop operations as C#3.0 doesn't supports default values for parameters in a method call.
        /// </summary>
        /// <param name="repetitions">
        /// Number of missing parameters to use. Must be equal or greater than one
        /// </param>
        /// <returns>
        /// A reference to the <see cref="IMethodOperations"/> instance which called this method.
        /// </returns>
        IMethodOperations AddMissingParameters(int repetitions);

        /// <summary>
        /// Add a specific number of instances for System.Type.Missing parameters to the call,
        /// passing the parameters by reference.
        /// Usefull for interop operations as C#3.0 doesn't supports default values for parameters in a method call.
        /// </summary>
        /// <param name="repetitions">
        /// Number of missing parameters to use. Must be equal or greater than one
        /// </param>
        /// <returns>
        /// A reference to the <see cref="IMethodOperations"/> instance which called this method.
        /// </returns>
        IMethodOperations AddRefMissingParameters(int repetitions);

        /// <summary>
        /// Performs the call to the method defined by a previous <see cref="IMethodCall.Method"/>
        /// call, with the parameters specified by the <see cref="IMethodOperations.AddParameter"/> calls
        /// casting the return value to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the data returned by the method call</typeparam>
        /// <returns>
        /// The data returned by the method call, casted to the <typeparamref name="T">T type param</typeparamref> 
        /// </returns>
        /// <remarks>
        /// The type parameter T must match the type of data returned by the method call 
        /// access or an exception will be throw.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// If the type parameter T does not match the type of data returned, thus a casting 
        /// could not be performed
        /// </exception>
        T Invoke<T>();

        /// <summary>
        /// Performs the call to the method which was defined by a previous <see cref="IMethodCall"/>
        /// call, with the parameters specified by the <see cref="IMethodOperations.AddParameter"/> calls
        /// The Method called either has no return parameters or they will be not needed.
        /// </summary>
        IDynamic Invoke();
    }
}

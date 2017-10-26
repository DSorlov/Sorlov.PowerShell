using System;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Provides get and set operations that can be invoked on a type's
    /// property o field
    /// </summary>
    public interface IGetSetOperations
    {
        /// <summary>
        /// Performs a Get operation to retrieve data, and returns it casted to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the data accessed</typeparam>
        /// <returns>
        /// The data accessed, casted to the <typeparamref name="T">T type param</typeparamref> 
        /// </returns>
        /// <remarks>
        /// The type parameter T must match the type of data we are trying to 
        /// access or an exception will be throw.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// If the type parameter T does not match the type of data accessed, thus a casting 
        /// could not be performed
        /// </exception>
        T Get<T>();

        /// <summary>
        /// Performs a Get operation to retrieve data.
        /// </summary>
        /// <returns>
        /// The data accessed as an <see cref="object"/>
        /// </returns>
        IDynamic Get();

        /// <summary>
        /// Performs a Set operation to modify data
        /// </summary>
        /// <param name="obj">New value </param>
        /// <remarks>
        /// The type of data passed as parameter 
        /// must match the type of data we are trying to modify or an 
        /// exception will be throw.
        /// </remarks>
        void Set(object obj);

        IGetSetOperations PropertyParam(object value);
    }
}

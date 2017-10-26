using System.Collections.Generic;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Defines operation to build a parameter list which will be passed
    /// for an operation invocation
    /// </summary>
    public interface IParameterBuilder
    {
        /// <summary>
        /// Returns the total number of parameters in the list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Indexer to the parameter list
        /// </summary>
        /// <param name="index">Position of the list to the parameter we want to access</param>
        /// <returns>The parameter </returns>
        object this[int index] { get; }

        /// <summary>
        /// Adds a parameter to the list which is meant to be called using by reference semantics.
        /// </summary>
        /// <param name="value">An object with the parameter value.</param>
        /// <returns>
        /// A reference to the <see cref="IParameterBuilder"/>instance which 
        /// called this method.
        /// </returns>
        IParameterBuilder AddRefParameter(object value);

        /// <summary>
        /// Adds a parameter to the list which is meant to be called using by value semantics.
        /// </summary>
        /// <param name="value">An object with the parameter value.</param>
        /// <returns>
        /// A reference to the <see cref="IParameterBuilder"/>instance which 
        /// called this method.
        /// </returns>
        IParameterBuilder AddParameter(object value);

        /// <summary>
        /// Access the parameter list as an array
        /// </summary>
        /// <returns>An array of objects with the currenly aded parameters.</returns>
        object[] GetParametersAsArray();

        /// <summary>
        /// Removes all parameters in the list
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a list of booleans with the same length as the total
        /// number of parameters in the list. Each position in the
        /// array of booleans has the value <c>true</c> or <c>false</c> 
        /// if the parameter with the same position index
        /// is passed by reference or not respectively.
        /// </summary>
        /// <returns>An IList of booleans.</returns>
        IList<bool> GetReferenceParameterList();
    }
}

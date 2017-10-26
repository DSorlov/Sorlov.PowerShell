using System.Collections.Generic;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Implementation
{
    /// <summary>
    /// Implements the <see cref="IParameterBuilder"/> interface to 
    /// save information about parameters in a method call.
    /// </summary>
    internal class ParameterBuilder : IParameterBuilder
    {
        /// <summary>
        /// Holds which position in the list of parameters
        /// is passed with by reference semantics of by value
        /// semantics
        /// </summary>
        private readonly List<bool> _isRef = new List<bool>();

        /// <summary>
        /// Holds the list of parameters
        /// </summary>
        private readonly List<object> _parameters = new List<object>();

        #region Miembros de IParameterList

        /// <summary>
        /// Adds a parameter to the list which is meant to be called using by value semantics.
        /// </summary>
        /// <param name="value">An object with the parameter value.</param>
        /// <returns>
        /// A reference to the <see cref="IParameterBuilder"/>instance which
        /// called this method.
        /// </returns>
        public IParameterBuilder AddParameter(object value)
        {
            _parameters.Add(value);
            _isRef.Add(false);

            return this;
        }

        /// <summary>
        /// Adds a parameter to the list which is meant to be called using by reference semantics.
        /// </summary>
        /// <param name="value">An object with the parameter value.</param>
        /// <returns>
        /// A reference to the <see cref="IParameterBuilder"/>instance which
        /// called this method.
        /// </returns>
        public IParameterBuilder AddRefParameter(object value)
        {
            _parameters.Add(value);
            _isRef.Add(true);

            return this;
        }

        /// <summary>
        /// Removes all parameters in the list
        /// </summary>
        public void Clear()
        {
            _parameters.Clear();
            _isRef.Clear();
        }

        /// <summary>
        /// Access the parameter list as an array
        /// </summary>
        /// <returns></returns>
        public object[] GetParametersAsArray()
        {
            return _parameters.ToArray();
        }

        /// <summary>
        /// Indexer to the parameter list
        /// </summary>
        /// <value></value>
        /// <returns>The parameter </returns>
        public object this[int index]
        {
            get { return _parameters[index]; }
        }

        /// <summary>
        /// Gets a list of booleans with the same length as the total
        /// number of parameters in the list. Each position in the
        /// array of booleans has the value <c>true</c> or <c>false</c>
        /// if the parameter with the same position index
        /// is passed by reference or not respectively.
        /// </summary>
        /// <returns>An IList of booleans.</returns>
        public IList<bool> GetReferenceParameterList()
        {
            return _isRef.AsReadOnly();
        }

        /// <summary>
        /// Returns the total number of parameters in the list.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get { return _parameters.Count; }
        }

        #endregion
    }
}

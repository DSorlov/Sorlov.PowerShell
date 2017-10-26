using System;
using System.Collections.Generic;
using System.Reflection;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Exceptions;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Implementation
{
    /// <summary>
    /// Implementation for IInvoker
    /// </summary>
    internal class Invoker :
        IDynamic,
        IMethodOperations,
        IGetSetOperations
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of the type Invoker
        /// </summary>
        /// <param name="invocationObject">object where the late binding calls will be made</param>
        internal Invoker(object invocationObject)
        {
            InstanceObject = invocationObject;
        }

        #endregion

        #region Fields
        /// <summary>
        /// Where the parameters added by a AddParameter call
        /// will be stored before the late binding call is made 
        /// </summary>
        private readonly IParameterBuilder _innerParameterBuilder = new ParameterBuilder();

        /// <summary>
        /// Object which recieves the late binding calls
        /// </summary>
        private object _instanceObject;

        /// <summary>
        /// Value of the parameters after the call to any
        /// <see cref="IMethodOperations.Invoke"/> method
        /// Only usefull to retrieve the values of the call that were passed as
        /// reference, and thus has been potentially modified.
        /// </summary>
        private object[] _lastCallParameters;

        /// <summary>
        /// Name of the method/property/field that will be 
        /// made next
        /// </summary>
        private string _operationName = string.Empty;

        /// <summary>
        /// Defines the element where the next get /set 
        /// call will be performed
        /// </summary>
        private EGetSetInvokerOperation? _operationType;
        #endregion

        #region IDynamic Members

        /// <summary>
        /// Object which recieves the late binding calls
        /// </summary>
        public object InstanceObject
        {
            get { return _instanceObject; }
            private set { _instanceObject = value; }
        }

        /// <summary>
        /// Selects the field over which we will invoke an operation.
        /// </summary>
        /// <param name="fieldName">String with the name of the field to be invoked</param>
        /// <returns> 
        /// An <see cref="IGetSetOperations"/> that will establish the operation to
        /// perform over the field specifyed by the fieldName parameter.
        /// </returns>
        public IGetSetOperations Field(string fieldName)
        {
            if (OperationName != string.Empty)
                throw new AlreadyDefinedOperationNameException();

            OperationName = fieldName;
            OperationType = EGetSetInvokerOperation.Field;

            return this;
        }

        /// <summary>
        /// Sets the Method name to be invoked
        /// </summary>
        /// <param name="methodName">String with the name of the method to be invoked</param>
        /// <returns> 
        /// An <see cref="IMethodOperations"/> that will establish the parameters to
        /// add to this method invocation, and also will allow to perform the method real invocation.
        /// </returns>
        public IMethodOperations Method(string methodName)
        {
            if (OperationName != string.Empty)
                throw new AlreadyDefinedOperationNameException();

            OperationName = methodName;

            return this;
        }

        /// <summary>
        /// Returns the value of the parameters after the call to any
        /// <see cref="IMethodOperations.Invoke"/> method
        /// Only usefull to retrieve the values of the call that were passed as
        /// reference, and thus has been potentially modified.
        /// </summary>
        /// <remarks>
        /// All parameters passed to the method are referenced after the call to an
        /// <see cref="IMethodOperations.Invoke"/> method, either they were passed as
        /// reference or not.
        /// </remarks>
        public object[] LastCallParameters
        {
            get { return _lastCallParameters; }
            private set { _lastCallParameters = value; }
        }

        /// <summary>
        /// Selects the property over which we will invoke an operation.
        /// </summary>
        /// <param name="propertyName">String with the name of the property to be invoked</param>
        /// <returns> 
        /// An <see cref="IGetSetOperations"/> that will establish the operation to
        /// perform over the property specifyed by the propertyName parameter.
        /// </returns>
        public IGetSetOperations Property(string propertyName)
        {
            if (OperationName != string.Empty)
                throw new AlreadyDefinedOperationNameException();

            OperationName = propertyName;
            OperationType = EGetSetInvokerOperation.Property;

            return this;
        }

        /// <summary>
        /// Performs indexer access over a type.
        /// </summary>
        /// <param name="indexList">object used as indexer</param>
        /// <returns>
        /// An <see cref="IGetSetOperations"/> that will establish the operation to
        /// perform over the specifyed index.
        /// </returns>
        public IGetSetOperations Index(params object[] indexList)
        {
            if (OperationName != string.Empty)
                throw new AlreadyDefinedOperationNameException();

            OperationName = "Item";
            OperationType = EGetSetInvokerOperation.Index;
            foreach (object idx in indexList)
                InnerParameterBuilder.AddParameter(idx);

            return this;
        }

        #endregion

        #region Object Methods Overrides

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            return InstanceObject.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return InstanceObject.GetHashCode();
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Once a late binding call is performed after calling Invoke, Get or Set methods,
        /// reinitialices the structures to allow making a distint call
        /// </summary>
        private void ClearCall()
        {
            OperationType = null;
            OperationName = string.Empty;
            InnerParameterBuilder.Clear();
        }

        /// <summary>
        ///     Checks if a number is greater than 1, throwing an exception if this is not true.
        /// </summary>
        /// <param name="integer">
        ///     Number to be checked.
        /// </param>
        private void AssertIntegerIsPositive(int integer)
        {
            if (integer <= 0)
                throw new ArgumentException(string.Format("integer must be equal or greater than 1 and is [{0}]", integer));
        }

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Where the parameters added by a AddParameter call
        /// will be stored before the late binding call is made 
        /// </summary>
        private IParameterBuilder InnerParameterBuilder
        {
            get { return _innerParameterBuilder; }
        }

        /// <summary>
        /// Name of the method/property/field that will be 
        /// made next
        /// </summary>
        private string OperationName
        {
            get { return _operationName; }
            set { _operationName = value; }
        }

        /// <summary>
        /// Element where the next get /set call will be performed 
        /// </summary>
        private EGetSetInvokerOperation? OperationType
        {
            get { return _operationType; }
            set { _operationType = value; }
        }

        #endregion

        #region IGetSetOperations Members

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
        public T Get<T>()
        {
            if (OperationName == string.Empty)
                throw new NoOperationNameDefinedException();

            object retVal = null;
            object[] args = null;
            EOperationType op;

            if (OperationType == EGetSetInvokerOperation.Index || InnerParameterBuilder.Count > 0)
            {
                op = EOperationType.PropertyGet;
                args = InnerParameterBuilder.GetParametersAsArray();
            }
            else
            {
                op = (OperationType == EGetSetInvokerOperation.Property)
                         ?
                             EOperationType.PropertyGet
                         : EOperationType.FieldGet;
            }

            try
            {
                CommonLateBindingOperations.CallOperation(
                    InstanceObject,
                    OperationName,
                    args,
                    out retVal,
                    op);
            }
            catch
            {
                // Sometimes -like when using Office Interop- the index get operation 
                // must be treated as a method
                if (OperationType == EGetSetInvokerOperation.Index)
                {
                    op = EOperationType.Method;
                    try
                    {
                        CommonLateBindingOperations.CallOperation(
                            InstanceObject,
                            OperationName,
                            args,
                            out retVal,
                            op);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                ClearCall();
            }
            return CommonLateBindingOperations.ComputeReturnType<T>(retVal);
        }

        /// <summary>
        /// Performs a Get operation to retrieve data.
        /// </summary>
        /// <returns>
        /// The data accessed as an <see cref="object"/>
        /// </returns>
        public IDynamic Get()
        {
            return Get<IDynamic>();
        }

        /// <summary>
        /// Performs a Set operation to modify data
        /// </summary>
        /// <param name="obj">New value </param>
        /// <remarks>
        /// The type of data passed as parameter 
        /// must match the type of data we are trying to modify or an 
        /// exception will be throw.
        /// </remarks>
        public void Set(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            if (OperationName == string.Empty)
                throw new NoOperationNameDefinedException();

            object retVal;
            object[] args;
            EOperationType op;


            if (OperationType == EGetSetInvokerOperation.Index)
            {
                op = EOperationType.PropertySet;
                List<object> tmp = new List<object>(InnerParameterBuilder.GetParametersAsArray());
                tmp.Add(obj);
                args = tmp.ToArray();
            }
            else
            {
                op = OperationType == EGetSetInvokerOperation.Property ?
                             EOperationType.PropertySet
                         : EOperationType.FieldSet;
                args = new object[] { obj };
            }

            try
            {
                CommonLateBindingOperations.CallOperation(
                    InstanceObject,
                    OperationName,
                    args,
                    out retVal,
                    op);
            }
            catch
            {
                throw;
            }
            finally
            {
                ClearCall();
            }
        }

        public IGetSetOperations PropertyParam(object value)
        {
            InnerParameterBuilder.AddParameter(value);

            return this;
        }

        #endregion

        #region IMethodOperations Members

        /// <summary>
        /// Adds a parameter to the method call, passed with value semantics
        /// </summary>
        /// <param name="value">object to be passed as parameter</param>
        /// <returns>
        /// A reference to the object which made this operation
        /// </returns>
        public IMethodOperations AddParameter(object value)
        {
            InnerParameterBuilder.AddParameter(value);

            return this;
        }

        /// <summary>
        /// Adds a parameter to the method call, passed with reference semantics
        /// </summary>
        /// <param name="value">object to be passed as parameter</param>
        /// <returns>
        /// A reference to the instance which called this operation.
        /// </returns>
        public IMethodOperations AddRefParameter(object value)
        {
            InnerParameterBuilder.AddRefParameter(value);

            return this;
        }

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
        public T Invoke<T>()
        {
            object retValue;
            object[] args;

            args = InnerParameterBuilder.GetParametersAsArray();

            if (InnerParameterBuilder.Count <= 0)
            {
                try
                {
                    CommonLateBindingOperations.CallOperation(
                        InstanceObject,
                        OperationName,
                        args,
                        out retValue,
                        EOperationType.Method);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    ClearCall();
                }
            }
            else
            {
                ParameterModifier refParams = new ParameterModifier(args.Length);
                for (int i = 0; i < args.Length; ++i)
                {
                    refParams[i] = InnerParameterBuilder.GetReferenceParameterList()[i];
                }

                try
                {
                    CommonLateBindingOperations.CallOperation(
                        InstanceObject,
                        OperationName,
                        args,
                        out retValue,
                        refParams,
                        EOperationType.Method);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    ClearCall();
                }
            }

            LastCallParameters = args;

            return CommonLateBindingOperations.ComputeReturnType<T>(retValue);
        }

        /// <summary>
        /// Performs the call to the method which was defined by a previous <see cref="IMethodCall.Method"/>
        /// call, with the parameters specified by the <see cref="IMethodOperations.AddParameter"/> calls
        /// The Method called either has no return parameters or they will be not needed.
        /// </summary>
        public IDynamic Invoke()
        {
            return Invoke<IDynamic>();
        }


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
        public IMethodOperations AddMissingParameters(int repetitions)
        {
            AssertIntegerIsPositive(repetitions);

            for (int i = 0; i < repetitions; i++)
                AddParameter(Type.Missing);

            return this;
        }

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
        public IMethodOperations AddRefMissingParameters(int repetitions)
        {
            AssertIntegerIsPositive(repetitions);

            for (int i = 0; i < repetitions; i++)
                AddRefParameter(Type.Missing);

            return this;
        }

        #endregion

        #region Nested type: EGetSetInvokerOperation

        /// <summary>
        /// Defines the element where the next get /set 
        /// call will be performed
        /// </summary>
        private enum EGetSetInvokerOperation
        {
            Field,
            Property,
            Index
        }

        #endregion
    }
}

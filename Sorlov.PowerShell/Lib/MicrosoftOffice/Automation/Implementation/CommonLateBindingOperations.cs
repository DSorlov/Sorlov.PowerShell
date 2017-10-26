using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Exceptions;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Implementation
{
    /// <summary>
    /// Some common internal operations to simplyfing low-level late binding calls.
    /// </summary>
    internal static class CommonLateBindingOperations
    {
        /// <summary>
        /// Calls an operation of specifyed name, with arguments.
        /// Operation in this context is any method or property in
        /// an object
        /// </summary>
        /// <param name="instanceObject">
        /// Instance object which recieves the operation call.
        /// </param>
        /// <param name="operationName">
        /// Name of operation to be called</param>
        /// <param name="args">
        /// Args for this operation call. Must be passed as an array of 
        /// objects, or null if the method doesn't need arguments
        /// </param>
        /// <param name="retVal">
        /// Object containing the operation return value, or null if 
        /// method returns nothing.
        /// The object must be casted by the user to the correct type
        /// </param>
        /// <param name="refParams">
        /// Indicates if any of the arguments must be passed by reference 
        /// </param>
        /// <param name="type">
        /// Value from <see cref="EOperationType"/> representing
        /// the type of call we are making
        /// </param>
        /// <returns>
        /// Bool if call was succesfull, false if some error ocurred. 
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Throwed if any argument is incorrect
        /// </exception>
        /// <exception cref="Exception">
        /// Throwed if the call fails
        /// </exception>
        internal static bool CallOperation(
            object instanceObject,
            string operationName,
            object[] args,
            out object retVal,
            ParameterModifier refParams,
            EOperationType type)
        {
            //Error check
            if (instanceObject == null)
                throw new NullReferenceException("Instance object has not been properly initialized.");

            //Must make an assignation 'cause retVal is marked with 'out'
            retVal = null;

            StackFrame sf = new StackFrame(true);

            //Do late call
            try
            {
                retVal = instanceObject.GetType().InvokeMember(operationName,
                                                               ComputeBindingFlags(type),
                                                               null,
                                                               instanceObject,
                                                               args,
                                                               new ParameterModifier[] { refParams },
                                                               null,
                                                               null);
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException is COMException)
                {
                    throw new Exception(
                        ErrorStrings.LATEBINDING_CALL_FAILED + " " + e.Message,
                        Marshal.GetExceptionForHR((e.InnerException as COMException).ErrorCode));
                }

                throw new Exception(ErrorStrings.LATEBINDING_CALL_FAILED + " " + e.Message, e);
            }

            //All OK
            return true;
        }

        /// <summary>
        /// Calls an operation of specifyed name, with arguments.
        /// Operation in this context is any method or property in
        /// an object
        /// </summary>
        /// <param name="instanceObject">
        /// Instance object which recieves the operation call.
        /// </param>
        /// <param name="operationName">Name of operation to be called</param>
        /// <param name="args">Args for this operation call. Must be passed as an array of
        /// objects, or null if the method doesn't need arguments</param>
        /// <param name="retVal">Object containing the operation return value, or null if
        /// method returns nothing.
        /// The object must be casted by the user to the correct type</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <returns>
        /// Bool if call was succesfull, false if some error ocurred.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Throwed if any argument is incorrect
        /// </exception>
        /// <exception cref="Exception">
        /// Throwed if the call fails
        /// </exception>
        internal static bool CallOperation(
            object instanceObject,
            string operationName,
            object[] args,
            out object retVal,
            EOperationType operationType)
        {
            if (instanceObject == null)
                throw new Exception(ErrorStrings.LATEBINDING_OBJECT_NOT_SET);

            //Must assign this
            retVal = null;

            //Bad method name
            if (operationName == string.Empty)
                throw new ArgumentException(ErrorStrings.LATEBINDING_EMPTY_CALL_STRING);

            BindingFlags bFlags = ComputeBindingFlags(operationType);

            //Do call
            try
            {
                //Do late binding call
                retVal = instanceObject.GetType().InvokeMember(operationName,
                                                               bFlags,
                                                               null,
                                                               instanceObject,
                                                               args);
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException is COMException)
                {
                    throw new OperationCallFailedException(
                        ErrorStrings.LATEBINDING_CALL_FAILED + " " + e.Message,
                        Marshal.GetExceptionForHR((e.InnerException as COMException).ErrorCode));
                }

                throw new OperationCallFailedException(ErrorStrings.LATEBINDING_CALL_FAILED + " " + e.Message, e);
            }

            //All OK
            return true;
        }

        /// <summary>
        /// Generates the correct bindingFlag necessary for a late binding call
        /// given an <see cref="EOperationType"/> type.
        /// </summary>
        /// <param name="type">
        /// <see cref="EOperationType"/> type
        /// </param>
        /// <returns>
        /// Correct binding flags for that type call, added to
        /// BindingFlags.Public | BindingFlags.Static flags
        /// </returns>
        internal static BindingFlags ComputeBindingFlags(EOperationType type)
        {
            switch (type)
            {
                case EOperationType.Method:
                    return BindingFlags.InvokeMethod;

                case EOperationType.PropertyGet:
                    return BindingFlags.GetProperty;

                case EOperationType.PropertySet:
                    return BindingFlags.SetProperty;

                case EOperationType.FieldSet:
                    return BindingFlags.SetField;

                case EOperationType.FieldGet:
                    return BindingFlags.GetField;

                case EOperationType.IndexGet:
                    return BindingFlags.InvokeMethod;

                case EOperationType.IndexSet:
                    return BindingFlags.InvokeMethod;
            }

            return BindingFlags.Default;
        }

        /// <summary>
        /// Given a generic type T:
        /// - If T is a a <see cref="IDynamic"/> 
        /// returns a new IDynamic to be used over the retVal parameter.
        /// - If T is any other type, it just returns the retVal parameter casted to T
        /// </summary>
        internal static T ComputeReturnType<T>(object retVal)
        {
            if (typeof(IDynamic).IsAssignableFrom(typeof(T))
                || typeof(IPropertyCall).IsAssignableFrom(typeof(T))
                || typeof(IMethodOperations).IsAssignableFrom(typeof(T))
                || typeof(IIndexerCall).IsAssignableFrom(typeof(T)))

                return (T)(object)new Invoker(retVal); //Ugly cast 
            return (T)retVal;
        }
    }
}

using System;
using System.Reflection;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Implementation;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// <para> Factory DP </para>
    /// Creates bindings of IInvoker instances to objects
    /// </summary>
    public static class BindingFactory
    {
        /// <summary>
        /// Creates a <see cref="IDynamic"/> instance binded to a object instance.
        /// </summary>
        /// <param name="obj">
        /// Object instance.
        /// </param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateObjectBinding(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return new Invoker(obj);
        }

        /// <summary>
        /// Creates a <see cref="IDynamic"/> instance binded to a new instance of a type.
        /// </summary>
        /// <param name="lbType">Type of the object used to create the instance.</param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateObjectBinding(Type lbType)
        {
            if (lbType == null)
                throw new ArgumentNullException("lbType");
            return new Invoker(Activator.CreateInstance(lbType));
        }

        /// <summary>
        /// Creates a new <see cref="IDynamic"/> instance binded to a internal created 
        /// instance of a type using the specified arguments to the constructor
        /// </summary>
        /// <param name="lbType">Type of the object to instantiate</param>
        /// <param name="args">Arguments for the type constructor</param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateObjectBinding(Type lbType, params object[] args)
        {
            if (lbType == null)
                throw new ArgumentNullException("lbType");
            return new Invoker(Activator.CreateInstance(lbType, args));
        }

        /// <summary>
        /// Creates a new <see cref="IDynamic"/> instance binded to a internal created 
        /// instance of a type using the specified arguments to the constructor
        /// </summary>
        /// <param name="assemblyName">Name of the assembly which contains the type to be instantiated.</param>
        /// <param name="typeName">Full name of the type to be instantiated.</param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateObjectBinding(string assemblyName, string typeName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException("assemblyName");

            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("assemblyName");

            Type lbType = Assembly.Load(assemblyName).GetType(typeName, false);

            return new Invoker(Activator.CreateInstance(lbType));
        }

        /// <summary>
        /// Creates a new <see cref="IDynamic"/> instance binded to a internal created 
        /// instance of a type using the specified arguments to the constructor
        /// </summary>
        /// <param name="assemblyName">Type of the object to instantiate</param>
        /// <param name="typeName">Full name of the type to be instantiated.</param>
        /// <param name="args">Arguments for the type constructor</param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateObjectBinding(string assemblyName, string typeName, params object[] args)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException("assemblyName");

            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("assemblyName");

            //TODO: Obsolete but needed to avoid passing the complete assembly name (which includes
            //assembly version and public key
#pragma warning disable 618,612
            Type lbType = Assembly.LoadWithPartialName(assemblyName).GetType(typeName, false);
#pragma warning restore 618,612

            return new Invoker(Activator.CreateInstance(lbType, args));
        }

        /// <summary>
        /// Creates a <see cref="IDynamic"/> instance binded to a new instance of 
        /// the automation object referenced by a progid <paramref name="progID"/>
        /// </summary>
        /// <param name="progID">An string with the application's ProgID </param>
        /// <returns>
        /// A new <see cref="IDynamic"/> instance.
        /// </returns>
        public static IDynamic CreateAutomationBinding(string progID)
        {
            if (string.IsNullOrEmpty(progID))
                throw new ArgumentNullException("progID");

            Type objectType = Type.GetTypeFromProgID(progID);

            return new Invoker(Activator.CreateInstance(objectType));
        }
    }
}

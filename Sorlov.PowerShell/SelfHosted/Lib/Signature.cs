using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    internal static class SignatureProxy
    {
        internal static Signature GenerateSignature(params object[] inputArgs)
        {
            List<Type> constructorTypes = new List<Type>();

            foreach (object inputObject in inputArgs)
                constructorTypes.Add(inputObject.GetType());

            Type type = typeof(System.Management.Automation.Signature);
            ConstructorInfo dynMethod = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, constructorTypes.ToArray(), null);
            return (Signature)dynMethod.Invoke(inputArgs);
        }

    }
}

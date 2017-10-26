namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Provides an interface for specifying a method to invoke on a type 
    /// </summary>
    public interface IMethodCall
    {
        /// <summary>
        /// Sets the Method name to be invoked
        /// </summary>
        /// <param name="methodName">String with the name of the method to be invoked</param>
        /// <returns> 
        /// An <see cref="IMethodOperations"/> that will establish the parameters to
        /// add to this method invocation, and also will allow to perform the method real invocation.
        /// </returns>
        IMethodOperations Method(string methodName);
    }
}

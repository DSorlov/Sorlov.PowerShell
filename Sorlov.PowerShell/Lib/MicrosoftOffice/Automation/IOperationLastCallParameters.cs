namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Represents the parameters used in a Method invocation
    /// </summary>
    public interface IOperationLastCallParameters
    {
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
        object[] LastCallParameters { get; }
    }
}

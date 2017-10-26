namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Provides an interface for specifying a field to access on a type 
    /// </summary>
    public interface IFieldCall
    {
        /// <summary>
        /// Selects the field over which we will invoke an operation.
        /// </summary>
        /// <param name="fieldName">String with the name of the field to be invoked</param>
        /// <returns> 
        /// An <see cref="IGetSetOperations"/> that will establish the operation to
        /// perform over the field specifyed by the fieldName parameter.
        /// </returns>
        IGetSetOperations Field(string fieldName);
    }
}

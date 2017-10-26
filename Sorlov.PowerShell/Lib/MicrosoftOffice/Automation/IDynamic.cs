namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Implements an interface to perform operations over a type using
    /// late binding calls
    /// </summary>
    public interface IDynamic :
        IPropertyCall,
        IMethodCall,
        IIndexerCall,
        IFieldCall,
        IOperationLastCallParameters
    {
        /// <summary>
        /// Object which late binding calls will be dispatched to
        /// </summary>
        object InstanceObject { get; }
    }
}

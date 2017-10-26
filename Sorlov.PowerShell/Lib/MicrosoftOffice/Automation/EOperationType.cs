namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    /// <summary>
    /// Establish the type of binding that 
    /// .NET must perform on an late binding call
    /// </summary>
    public enum EOperationType
    {
        /// <summary>
        /// The operation calls a method
        /// </summary>
        Method,

        /// <summary>
        /// The operation sets the value for a property
        /// </summary>
        PropertyGet,

        /// <summary>
        /// The operation retrieves a value for a property
        /// </summary>
        PropertySet,

        /// <summary>
        /// The operation gets the value for a field
        /// </summary>
        FieldGet,

        /// <summary>
        /// The operation sets the value for a field
        /// </summary>
        FieldSet,

        /// <summary>
        /// The operation gets the value for an index 
        /// </summary>
        IndexGet,

        /// <summary>
        /// The operation performs sets the value for t an index 
        /// </summary>
        IndexSet
    }
}

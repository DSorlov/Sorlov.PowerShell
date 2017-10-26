using System;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckNotNull : ValidateArgumentsAttribute
{
    private string message = "Value may not be null";

    public CheckNotNull(string message)
    {
        this.message = message;
    }

    public CheckNotNull()
    {
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);
    }

    public string Message
    {
        get { return message; }
        set { this.message = value; }
    }

}
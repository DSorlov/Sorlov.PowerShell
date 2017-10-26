using System;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckNotNullOrEmpty : ValidateArgumentsAttribute
{
    private string message = "Value may not be null or empty";

    public CheckNotNullOrEmpty(string message)
    {
        this.message = message;
    }

    public CheckNotNullOrEmpty()
    {
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);

        if (arguments.ToString()==string.Empty)
            throw new CheckValidationException(message);
    }

    public string Message
    {
        get { return message; }
        set { this.message = value; }
    }

}
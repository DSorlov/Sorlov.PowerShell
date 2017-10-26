using System;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckLength : ValidateArgumentsAttribute
{
    private int maxLength = int.MaxValue;
    private int minLength = int.MinValue;
    private string message = "Could not validate length";

    public CheckLength()
    {
    }
    public CheckLength(int minLength)
    {
        this.minLength = minLength;
    }
    public CheckLength(int minLength, int maxLength)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }
    public CheckLength(int minLength, int maxLength, string message)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.message = message;
    }
    public CheckLength(int minLength, string message)
    {
        this.minLength = minLength;
        this.message = message;
    }
    public CheckLength(string message)
    {
        this.message = message;
    }


    public string Message { get { return message; } set { message = value; }}
    public int MaxLength { get { return maxLength; } set { maxLength = value; } }
    public int MinLength { get { return minLength; } set { minLength = value; } }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        long value = arguments.ToString().Length;

        if (value<minLength || value>maxLength)
        {
            throw new CheckValidationException(message);
        }
    }
}
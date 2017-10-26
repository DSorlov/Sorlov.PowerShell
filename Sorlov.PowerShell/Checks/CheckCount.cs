using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckCount : ValidateArgumentsAttribute
{
    private int maxLength = int.MaxValue;
    private int minLength = int.MinValue;
    private string message = "Could not validate number of elements";

    private static bool IsGenericEnumerable(Type type)
    {
        return type.IsGenericType &&
           typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition());
    }

    public CheckCount()
    {
    }
    public CheckCount(int minLength)
    {
        this.minLength = minLength;
    }
    public CheckCount(int minLength, int maxLength)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }
    public CheckCount(int minLength, int maxLength, string message)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.message = message;
    }
    public CheckCount(int minLength, string message)
    {
        this.minLength = minLength;
        this.message = message;
    }
    public CheckCount(string message)
    {
        this.message = message;
    }


    public string Message { get { return message; } set { message = value; }}
    public int MaxLength { get { return maxLength; } set { maxLength = value; } }
    public int MinLength { get { return minLength; } set { minLength = value; } }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (!IsGenericEnumerable(arguments.GetType()))
            throw new CheckValidationException(message);

        int value = ((IEnumerable<object>) arguments).Count();

        if (value < minLength || value > maxLength)
        {
            throw new CheckValidationException(message);
        }
    }
}
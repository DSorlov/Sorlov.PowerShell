using System;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckRange : ValidateArgumentsAttribute
{
    private int maxRange = int.MaxValue;
    private int minRange = int.MinValue;
    private string message = "Could not validate range";

    public CheckRange()
    {
    }
    public CheckRange(int minRange)
    {
        this.minRange = minRange;
    }
    public CheckRange(int minRange, int maxRange)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
    }
    public CheckRange(int minRange, int maxRange, string message)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.message = message;
    }
    public CheckRange(int minRange, string message)
    {
        this.minRange = minRange;
        this.message = message;
    }
    public CheckRange(string message)
    {
        this.message = message;
    }


    public string Message { get { return message; } set { message = value; }}
    public int MaxRange { get { return maxRange; } set { maxRange = value; } }
    public int MinRange { get { return minRange; } set { minRange = value; } }


    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        long value = long.Parse(arguments.ToString());

        if (value<minRange || value>maxRange)
        {
            throw new CheckValidationException(message);
        }
    }
}
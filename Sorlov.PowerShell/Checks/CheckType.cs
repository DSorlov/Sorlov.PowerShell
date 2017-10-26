using System;
using System.Globalization;
using System.Management.Automation;
using System.Threading;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckType : ValidateArgumentsAttribute
{
    private readonly string[] validValues;
    private string message = "Could not validate the inputs type, possible types are: {0}";
    private bool ignoreCase = true;
    private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

    public CheckType(params string[] validValues)
    {
        this.validValues = validValues;
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        string input = arguments.ToString();

        foreach (string validValue in validValues) {
            if (string.Compare(arguments.GetType().ToString(),validValue,true,Thread.CurrentThread.CurrentCulture)==0) return;
        }

        throw new CheckValidationException(message.Replace("{0}",string.Join(",", validValues)));
    }

    public string Message
    {
        get { return message; }
        set { this.message = value; }
    }

    public bool IgnoreCase
    {
        get { return ignoreCase; }
        set { this.ignoreCase = value; }
    }

    public CultureInfo CultureInfo
    {
        get { return cultureInfo; }
        set { this.cultureInfo = value; }
    }
}
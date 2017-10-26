using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckPattern : ValidateArgumentsAttribute
{
    private RegexOptions regexpOptions = RegexOptions.IgnoreCase;
    private string regexString;
    private string message = "Could not validate regexString";

    public CheckPattern(string regexString)
    {
        this.regexString = regexString;
    }

    public CheckPattern(string regexString, string message)
    {
        this.regexString = regexString;
        this.message = message;
    }

    public string Message { get { return message; } set { message = value; } }
    public string RegexString { get { return regexString; } set { regexString = value; } }
    public RegexOptions Options { get { return regexpOptions; } set { regexpOptions = value; } }


    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        string input = arguments.ToString();
        Regex regex = null;
        regex = new Regex(regexString, regexpOptions);
        if (!regex.Match(input).Success)
        {
            throw new CheckValidationException(message);
        }
    }
}
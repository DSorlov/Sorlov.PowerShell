using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Windows.Forms;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckFileSize : ValidateArgumentsAttribute
{
    private string message = "File does not fill the size requirements";
    private string filename;
    private long minLength = int.MinValue;
    private long maxLength = int.MaxValue;

    public CheckFileSize(long minLength, long maxLength, string message)
    {
        this.message = message;
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    public CheckFileSize(long minLength, long maxLength)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);

        try
        {
            ProviderInfo providerInfo = null;
            Collection<string> paths = engineIntrinsics.SessionState.Path.GetResolvedProviderPathFromPSPath((string)arguments, out providerInfo);

            FileInfo file = new FileInfo(paths[0]);
            if (!file.Exists)
                throw new CheckValidationException(message);

            if (file.Length < minLength || file.Length > maxLength)
                throw new CheckValidationException(message);
        }
        catch (Exception)
        {
            throw new CheckValidationException(message);
        }
    }

    public string Message
    {
        get { return message; }
        set { this.message = value; }
    }

}
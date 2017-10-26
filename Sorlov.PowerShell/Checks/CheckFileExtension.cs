using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Threading;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckFileExtension : ValidateArgumentsAttribute
{
    private string message = "File extension is not valid";
    private string extension;

    public CheckFileExtension(string extension, string message)
    {
        this.message = message;
        this.extension = extension;
    }

    public CheckFileExtension(string extension)
    {
        this.extension = extension;
    }

    public string Extension { get { return extension; } set { extension = value; }}

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);

        try
        {
            ProviderInfo providerInfo = null;
            Collection<string> paths = engineIntrinsics.SessionState.Path.GetResolvedProviderPathFromPSPath((string)arguments, out providerInfo);

            FileInfo file = new FileInfo(paths[0]);
            if (string.Compare(file.Extension, extension, true, Thread.CurrentThread.CurrentCulture) != 0)
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
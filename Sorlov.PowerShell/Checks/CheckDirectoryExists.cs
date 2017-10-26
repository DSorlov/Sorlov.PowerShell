using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckDirectoryExists : ValidateArgumentsAttribute
{
    private string message = "The directory does not exist";

    public CheckDirectoryExists(string message)
    {
        this.message = message;
    }

    public CheckDirectoryExists()
    {
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);

        try
        {
            ProviderInfo providerInfo = null;
            Collection<string> paths = engineIntrinsics.SessionState.Path.GetResolvedProviderPathFromPSPath((string)arguments, out providerInfo);
            if (!Directory.Exists(paths[0]))
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
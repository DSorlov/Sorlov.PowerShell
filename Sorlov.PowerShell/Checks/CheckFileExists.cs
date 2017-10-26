using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckFileExists : ValidateArgumentsAttribute
{
    private string message = "The file was not found";

    public CheckFileExists(string message)
    {
        this.message = message;
    }

    public CheckFileExists()
    {
    }

    protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
    {
        if (arguments==null)
            throw new CheckValidationException(message);

        try
        {
            ProviderInfo providerInfo = null;
            Collection<string> paths = engineIntrinsics.SessionState.Path.GetResolvedProviderPathFromPSPath((string) arguments, out providerInfo);

            FileInfo file = new FileInfo(paths[0]);
            if (!file.Exists)
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
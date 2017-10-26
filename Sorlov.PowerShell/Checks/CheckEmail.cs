using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CheckEmail : CheckPattern
{
    private const string regexp = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

    public CheckEmail()
        : base(regexp,"Could not validate email address")
    {
    }

    public CheckEmail(string message)
        : base(regexp,message)
    {
    }

}
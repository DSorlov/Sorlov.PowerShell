using System;

public class CheckValidationException: Exception
{
    public CheckValidationException(string errorMessage) : base(errorMessage) {}
    public CheckValidationException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) {}
}
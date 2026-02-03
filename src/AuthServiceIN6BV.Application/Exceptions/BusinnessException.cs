namespace AuthServiceIN6BV.Application.Exceptions;

public class BusinnessException : Exception
{
    public string ErrorCode {get;}

    public BusinessException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
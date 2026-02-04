namespace AuthServiceIN6BV.Application.Exceptions;
 
public class BusinnessException : Exception
{
    public string ErrorCode { get; }
 
    public BusinnessException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
 
    public BusinnessException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
 
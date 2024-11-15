using Grpc.Core;

namespace Employee.Service.Exceptions;

public class EmployeeException : Exception
{
    public StatusCode ErrorCode { get; protected init; }

    public EmployeeException(StatusCode errorCode = StatusCode.Internal)
        : base("Internal server error")
    {
        ErrorCode = errorCode;
    }

    public EmployeeException(string message, StatusCode errorCode = StatusCode.Internal)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public EmployeeException(string message, Exception innerException, StatusCode errorCode = StatusCode.Internal)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
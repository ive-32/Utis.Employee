using Grpc.Core;

namespace Employee.Service.Exceptions;

public class InvalidArgumentEmployeeException : EmployeeException
{
    public InvalidArgumentEmployeeException(StatusCode errorCode = StatusCode.InvalidArgument)
        : base("Invalid Argument")
    {
        ErrorCode = errorCode;
    }

    public InvalidArgumentEmployeeException(string message, StatusCode errorCode = StatusCode.InvalidArgument)
        : base(message)
    {
        ErrorCode = errorCode;
    }

}
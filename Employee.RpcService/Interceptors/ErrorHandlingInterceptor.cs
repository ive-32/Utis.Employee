using System.Text;
using Employee.RpcService.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Employee.RpcService.Interceptors;

public class ExceptionHandlingServerInterceptor : Interceptor
{
    private readonly ILogger<ExceptionHandlingServerInterceptor> _logger;

    public ExceptionHandlingServerInterceptor(ILogger<ExceptionHandlingServerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        TResponse response;
        try
        {
            _logger.LogTrace("Incoming request: {req}", context.Method);
            response = await continuation(request, context);
        }
        catch (EmployeeException ex)
        {
            _logger.LogInformation(ex, "Client error occured while handling {Method} request",
                context.Method);
            throw new RpcException(new Status(ex.ErrorCode, ex.Message), new Metadata()
            {
                {
                    "Code",
                    ex.ErrorCode.ToString()
                },
                {
                    "Message-bin",
                    Encoding.UTF8.GetBytes(ex.Message)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "While handling grpc request({callPath}) error occurred", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }

        return response;
    }
}


using NLog;
using System.Text;

namespace GKTodoManager.API.Middleware;

public class LoggerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task Invoke(HttpContext context)
    {
        LogRequest(context);
        await _next(context);
    }

    private static void LogRequest(HttpContext context)
    {
        var request = context.Request;

        var requestLog = new StringBuilder();
        requestLog.AppendLine($"{request.Method} {request.Path}");
        // requestLog.AppendLine($"Host: {request.Host}");

        _logger.Info(requestLog.ToString());
    }
}
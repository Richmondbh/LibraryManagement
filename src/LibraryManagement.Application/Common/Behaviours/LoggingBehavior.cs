using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8]; // Short ID for correlation

        // Log request
        _logger.LogInformation(
            "[{RequestId}] Starting {RequestName}",
            requestId,
            requestName);

        // Log request details at Debug level (avoiding logging sensitive data in production)
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                var requestBody = JsonSerializer.Serialize(request, JsonOptions);
                _logger.LogDebug(
                    "[{RequestId}] {RequestName} Request: {RequestBody}",
                    requestId,
                    requestName,
                    requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(
                    "[{RequestId}] Could not serialize request: {Error}",
                    requestId,
                    ex.Message);
            }
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            // Log success
            _logger.LogInformation(
                "[{RequestId}] Completed {RequestName} in {ElapsedMs}ms",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds);

            // Log response at Debug level
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                try
                {
                    var responseBody = JsonSerializer.Serialize(response, JsonOptions);
                    _logger.LogDebug(
                        "[{RequestId}] {RequestName} Response: {ResponseBody}",
                        requestId,
                        requestName,
                        responseBody);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(
                        "[{RequestId}] Could not serialize response: {Error}",
                        requestId,
                        ex.Message);
                }
            }

            // Warn if request took too long
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "[{RequestId}] Long running request: {RequestName} took {ElapsedMs}ms",
                    requestId,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failure
            _logger.LogError(
                ex,
                "[{RequestId}] {RequestName} failed after {ElapsedMs}ms: {ErrorMessage}",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            throw;
        }
    }
}

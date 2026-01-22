using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Common.Behaviours
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer;

        private const int WarningThresholdMs = 500;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMs = _timer.ElapsedMilliseconds;

            if (elapsedMs > WarningThresholdMs)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "⚠️ Long Running Request: {RequestName} ({ElapsedMs}ms)",
                    requestName,
                    elapsedMs);
            }

            return response;
        } 
    }
}

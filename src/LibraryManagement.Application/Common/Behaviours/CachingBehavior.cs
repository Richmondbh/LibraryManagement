using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Common.Behaviours;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
       TRequest request,
       RequestHandlerDelegate<TResponse> next,
       CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        // Try to get from cache
        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", cacheKey);

        // Get from handler
        var response = await next();

        // Store in cache
        await _cacheService.SetAsync(cacheKey, response, request.CacheDuration, cancellationToken);

        return response;
    }
}

using DotnetSitemapGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mvp.Foundation.DataFetching.GraphQL;
using Mvp.Project.MvpSite.Middleware;
using Mvp.Project.MvpSite.Sitemap.Providers;
using SitemapDataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Mvp.Project.MvpSite.Rendering;

public class MvpSitemapUrlProvider : ISitemapProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MvpSitemapUrlProvider> _logger;
    private readonly IGraphQLClientFactory _graphQLClientFactory;
    private readonly IGraphQLRequestBuilder _graphQLRequestBuilder;
    private readonly IConfiguration _configuration;

    public MvpSitemapUrlProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<MvpSitemapUrlProvider> logger,
        IGraphQLClientFactory graphQLClientFactory,
        IGraphQLRequestBuilder graphQLRequestBuilder,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _graphQLClientFactory = graphQLClientFactory;
        _graphQLRequestBuilder = graphQLRequestBuilder;
        _configuration = configuration;
    }

    public Task<IReadOnlyCollection<SitemapNode>> GetNodes()
    {
        var nodes = new List<SitemapNode>();
        var results = GetSiteMap();

        foreach (var result in results)
        {
            var url = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + result.Url.Path;
                
            _logger.LogInformation("Adding product: {URL}", url);

            SitemapNode node = new(url)
            {
                LastModificationDate = DateTime.ParseExact(result.UpdatedDatetime?.Value,
                "yyyyMMdd'T'HHmmss'Z'",
                CultureInfo.InvariantCulture),
                Priority = Convert.ToDecimal(result.Priority?.TargetItem?.DisplayName),
                ChangeFrequency = Common.GetChangeFrequency(result.ChangeFrequency?.TargetItem?.DisplayName)
            };
            nodes.Add(node);
        }

        return Task.FromResult<IReadOnlyCollection<SitemapNode>>(nodes);
    }

    private List<Result> GetSiteMap()
    {
        CustomGraphQlSitemapUrlServiceHandler customGraphQlLayoutServiceHandler = new(_configuration, _graphQLRequestBuilder, _graphQLClientFactory);
        return  customGraphQlLayoutServiceHandler.GetSitemap().Result;
    }
}
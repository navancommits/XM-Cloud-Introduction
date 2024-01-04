using DotnetSitemapGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mvp.Foundation.DataFetching.GraphQL;
using Mvp.Project.MvpSite.Middleware;
using Mvp.Project.MvpSite.Models;
using Mvp.Project.MvpSite.Sitemap.Providers;
using SitemapXmlModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mvp.Project.MvpSite.Rendering;

public class MvpSitemapXmlDataProvider : ISitemapProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MvpSitemapXmlDataProvider> _logger;
    private readonly IGraphQLClientFactory _graphQLClientFactory;
    private readonly IGraphQLRequestBuilder _graphQLRequestBuilder;
    private readonly IConfiguration _configuration;

    public MvpSitemapXmlDataProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<MvpSitemapXmlDataProvider> logger,
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

    private Urlset GetXmlString(List<XmlResult> result)
    {
        var row = result.FirstOrDefault();        
        var sitemapxmlstring = row.SitemapXml.Value;

        XmlSerializer serializer = new(typeof(Urlset));
        using StringReader reader = new(sitemapxmlstring);
        Urlset sitemapxml = (Urlset)serializer.Deserialize(reader);

        return sitemapxml;
    }

    public Task<IReadOnlyCollection<SitemapNode>> GetNodes()
    {
        var nodes = new List<SitemapNode>();
        var result = GetSiteMap();

        Urlset sitemap=GetXmlString(result);

        foreach (var row in sitemap.Url)
        {
            var url = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + row.Loc;

            _logger.LogInformation("Adding product: {URL}", url);

            SitemapNode node = new(url)
            {
                LastModificationDate = DateTime.ParseExact(row.Lastmod,
                "yyyyMMdd'T'HHmmss'Z'",
                CultureInfo.InvariantCulture),
                Priority = Convert.ToDecimal(row.Priority),
                ChangeFrequency = Common.GetChangeFrequency(row.Changefreq)
            };
            nodes.Add(node);
        }        

        return Task.FromResult<IReadOnlyCollection<SitemapNode>>(nodes);
    }

    private List<XmlResult> GetSiteMap()
    {
        CustomGraphQlSitemapXmlServiceHandler customGraphQlLayoutServiceHandler = new(_configuration, _graphQLRequestBuilder, _graphQLClientFactory);
        return customGraphQlLayoutServiceHandler.GetSitemap().Result;
    }
}
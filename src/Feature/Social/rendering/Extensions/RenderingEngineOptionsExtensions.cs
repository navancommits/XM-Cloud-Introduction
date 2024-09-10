﻿using Mvp.Feature.Social.Models;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Feature.Social.FeedReader;

namespace Mvp.Feature.Social.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static IServiceCollection AddFeatureSocialServices(this IServiceCollection services)
        {
            services.AddTransient<IFeedReader, RssFeedReader>();
            return services;
        }

        public static RenderingEngineOptions AddFeatureSocial(this RenderingEngineOptions options)
        {
            options.AddModelBoundView<Rss>("Rss");
            return options;
        }
    }
}
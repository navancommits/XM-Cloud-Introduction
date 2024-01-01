using Sitecore.Links;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System.Linq;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System;
using Sitecore.Data.Fields;
using Sitecore.Globalization;

namespace XmCloudPreview
{
    public class SitemapGenerator : Command
    {
        public override void Execute(CommandContext context)
        {
            var result = GetSitemapData();

            Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase("master");
            Item contextItem = database.SelectSingleItem("/sitecore/content/MvpSite/Shared Content/MVP Sitemap");

            if (!string.IsNullOrWhiteSpace(result))
            {
                using (new LanguageSwitcher("en"))
                {
                    var itemver=contextItem.Versions.AddVersion();
                    itemver.Editing.BeginEdit();
                    itemver["SitemapXml"] = result;
                    itemver.Editing.EndEdit();
                }
            }

            SheerResponse.Alert("Sitemap XML Generated.");
            return;
        }

        public string GetSitemapData()
        {        
            Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase("master");
            Item contextItem = database.SelectSingleItem("/sitecore/content/mvpsite");

            List<Item> pagelist = contextItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == "{A054184C-3B20-4C2B-AB04-C8A03ACD5522}" || x.TemplateID.ToString() == "{9C1E30A0-8691-4AD4-BEF0-E1F446EEF8F3}").ToList();

            if (contextItem == null) return null;

            string sitemap = "<urlset>";
            foreach (var page in pagelist)
            {

                Item item = contextItem.Database.GetItem(page.Paths.FullPath);
                if (item != null)
                {
                    var includeinsitemap = item["IncludeinSitemap"];
                    if (!string.IsNullOrWhiteSpace(includeinsitemap))
                    {
                        sitemap=$"{sitemap}<url>";                    
                                          
                        string location = string.Empty;
                        var options = LinkManager.GetDefaultUrlBuilderOptions();
                        options.AlwaysIncludeServerUrl = false;
                        string link = LinkManager.GetItemUrl(item,options);
                        var abspath = new Uri(link).AbsolutePath;
                        sitemap=$"{sitemap}<loc>{abspath}</loc>";

                        ReferenceField changefrequencyfield = item.Fields["ChangeFrequency"];
                        Item changefrequency = changefrequencyfield.TargetItem;
                        sitemap = $"{sitemap}<changefreq>{changefrequency.Name}</changefreq>";

                        ReferenceField priorityfield = item.Fields["Priority"];
                        Item priority = priorityfield.TargetItem;
                        sitemap = $"{sitemap}<priority>{priority.Name}</priority>";

                        sitemap = $"{sitemap}<lastmod>{item.Fields["__Updated"]}</lastmod>";

                        sitemap = $"{sitemap}</url>";
                    }
                }

            }
            sitemap = $"{sitemap}</urlset>";

            return sitemap;
        }
    }
}

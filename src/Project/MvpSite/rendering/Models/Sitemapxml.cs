using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mvp.Project.MvpSite.Models
{
    [XmlRoot(ElementName = "url")]
    public class Url
    {

        [XmlElement(ElementName = "loc")]
        public string Loc { get; set; }

        [XmlElement(ElementName = "changefreq")]
        public string Changefreq { get; set; }

        [XmlElement(ElementName = "priority")]
        public int Priority { get; set; }

        [XmlElement(ElementName = "lastmod")]
        public string Lastmod { get; set; }
    }

    [XmlRoot(ElementName = "urlset")]
    public class Urlset
    {

        [XmlElement(ElementName = "url")]
        public List<Url> Url { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //public class SitemapXmlData
    //{
    //    public ItemData Item { get; set; }
    //}

    //public class ItemData
    //{
    //    public SitemapXml Sitemap { get; set; }
    //    public string DisplayName { get; set; }
    //    public Myfield MyFieldVal { get; set; }
    //}

    //public class Myfield
    //{
    //    public string Value { get; set; }
    //}

    //public class SitemapXmlDataRoot
    //{
    //    public SitemapXmlData SitemapXmlDataRootNode { get; set; }
    //}

    //public class SitemapXml
    //{
    //    public string Value { get; set; }
    //}

    //public class SitemapXmlData
    //{
    //    public Item SitemapItem { get; set; }
    //}

    //public class Field
    //{
    //    public string Name { get; set; }
    //    public string Id { get; set; }
    //    public string Value { get; set; }
    //}

    //public class Item
    //{
    //    public List<Field> SitemapFields { get; set; }
    //}

    //public class SitemapXmlRoot
    //{
    //    public SitemapXmlData SitemapXml { get; set; }
    //}
}

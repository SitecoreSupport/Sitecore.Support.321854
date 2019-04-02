namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.Links;
  using Sitecore.XA.Feature.SiteMetadata.Enums;
  using Sitecore.XA.Feature.SiteMetadata.Sitemap;
  using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;

  public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
  {
    public SitemapGenerator() : base() { }

    public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings)
    {
    }

    protected override StringBuilder BuildMultilanguageSitemap(IEnumerable<Item> childrenTree, SitemapLinkOptions options)
    {
      UrlOptions urlOptions = GetUrlOptions();
      SitemapLinkOptions sitemapLinkOptions = new SitemapLinkOptions(options.Scheme, urlOptions, options.TargetHostname);
      UrlOptions urlOptions2 = (UrlOptions)urlOptions.Clone();
      urlOptions2.LanguageEmbedding = LanguageEmbedding.Always;
      SitemapLinkOptions sitemapLinkOptions2 = new SitemapLinkOptions(options.Scheme, urlOptions2, options.TargetHostname);
      List<XElement> list = new List<XElement>();
      foreach (Item item4 in childrenTree)
      {
        SitemapChangeFrequency sitemapChangeFrequency = item4.Fields[Sitecore.XA.Feature.SiteMetadata.Templates.Sitemap._Sitemap.Fields.ChangeFrequency].ToEnum<SitemapChangeFrequency>();
        if (sitemapChangeFrequency != 0)
        {
          List<XElement> list2 = new List<XElement>();
          Language[] languages = item4.Languages;
          foreach (Language language in languages)
          {
            Item item = item4.Database.GetItem(item4.ID, language);
            if (item.Versions.Count > 0)
            {
              sitemapLinkOptions2.UrlOptions.Language = language;
              string fullLink = GetFullLink(item, sitemapLinkOptions2);
              string name = language.CultureInfo.Name;
              XElement item2 = BuildAlternateLinkElement(fullLink, name, "alternate");
              list2.Add(item2);
            }
          }
          sitemapLinkOptions.UrlOptions.Language = item4.Language;
          string fullLink2 = GetFullLink(item4, sitemapLinkOptions);
          string updatedDate = GetUpdatedDate(item4);
          string changefreq = sitemapChangeFrequency.ToString().ToLower();
          string priority = GetPriority(item4);
          XElement item3 = BuildPageElement(fullLink2, updatedDate, changefreq, priority, list2);
          list.Add(item3);
        }
      }
      XDocument xDocument = BuildXmlDocument(list);
      StringBuilder stringBuilder = new StringBuilder();
      using (TextWriter textWriter = new StringWriter(stringBuilder))
      {
        xDocument.Save(textWriter);
      }
      FixDeclaration(stringBuilder);
      return stringBuilder;
    }

    protected override IList<Item> ChildrenSearch(Item homeItem)
    {
      List<Item> list = new List<Item>();
      Queue<Item> queue = new Queue<Item>();
      if (homeItem.HasChildren)
      {
        queue.Enqueue(homeItem);
        if (homeItem.Versions.Count > 0)
        {
          list.Add(homeItem);
        }

        list.AddRange(GetItemsForOtherLanguages(homeItem));
        while (queue.Count != 0)
        {
          foreach (Item child in queue.Dequeue().Children)
          {
            if (!list.Contains(child))
            {
              if (!ShouldBeSkipped(child))
              {
                if (child.Versions.Count > 0)
                {
                  list.Add(child);
                }

                list.AddRange(GetItemsForOtherLanguages(child));
              }

              if (child.HasChildren)
              {
                queue.Enqueue(child);
              }
            }
          }
        }
      }

      return list;
    }

    protected override IEnumerable<Item> GetItemsForOtherLanguages(Item item)
    {
      foreach (Language item3 in from language in item.Languages
                                 where language != item.Language
                                 select language)
      {
        Item item2 = item.Database.GetItem(item.ID, item3);
        if (item2.Versions.Count > 0)
        {
          yield return item2;
        }
      }
    }

  }
}
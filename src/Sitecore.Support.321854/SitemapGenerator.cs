namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
  {
    public SitemapGenerator() : base() { }

    public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings)
    {
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
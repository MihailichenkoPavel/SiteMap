using SiteMap.DAL.Entity;
using System;
using System.Collections.Generic;

namespace SiteMap.DAL.Interfaces
{
    public interface IRepository
    {
        Sitemap GetSitemap(int id);
        Sitemap GetSitemap(string url);
        IEnumerable<Sitemap> GetSitemaps(int count, int skip);
        int SitemapsCount(Func<Sitemap, bool> predicate = null);
        Sitemap AddSitemap(string url);
        void AddSitemapLinks(int sitemapId, IEnumerable<SitemapLink> links);
    }
}
using PagedList;
using SiteMap.DAL.Entity;
using System.Collections.Generic;

namespace SiteMap.Interfaces
{
    interface ISitemapService
    {
        StaticPagedList<Sitemap> GetHistory(int page);
        List<SitemapLink> GetSitemapLink(string url);
        List<string> GetLinks(string url);
    }
}

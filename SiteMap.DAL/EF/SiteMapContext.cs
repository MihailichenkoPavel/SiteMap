using SiteMap.DAL.Entity;
using System.Data.Entity;

namespace SiteMap.DAL.EF
{
    public class SiteMapContext : DbContext
    {
        public SiteMapContext() : base("DBConnection")
        {
        }

        public DbSet<Sitemap> Sitemaps { get; set; }
        public DbSet<SitemapLink> SitemapLinks { get; set; }
    }
}

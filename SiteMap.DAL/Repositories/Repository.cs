using SiteMap.DAL.EF;
using SiteMap.DAL.Entity;
using SiteMap.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SiteMap.DAL.Repositories
{
    public class Repository : IRepository
    {
        private readonly SiteMapContext _context;
        public Repository()
        {
            _context = new SiteMapContext();
        }

        public Sitemap GetSitemap(int id)
        {
            return _context.Sitemaps.FirstOrDefault(s => s.Id == id);
        }
        public Sitemap GetSitemap(string url)
        {
            return _context.Sitemaps.Include(x => x.Links).FirstOrDefault(s => s.HttpAddress == url);
        }

        public IEnumerable<Sitemap> GetSitemaps(int count, int skip)
        {
            return _context.Sitemaps.OrderBy(s => s.Id).Skip(skip).Take(count);
        }
        public int SitemapsCount(Func<Sitemap, bool> predicate = null)
        {
            return predicate == null ? _context.Sitemaps.Count() : _context.Sitemaps.Count(predicate);
        }

        public Sitemap AddSitemap(string url)
        {
            var sitemap = _context.Sitemaps.FirstOrDefault(s => s.HttpAddress == url);

            if (sitemap != null)
                sitemap.LastRequest = DateTime.Now;
            else
                _context.Sitemaps.Add(new Sitemap()
                {
                    HttpAddress = url,
                    LastRequest = DateTime.Now
                });

            _context.SaveChanges();

            return GetSitemap(url);
        }

        public void AddSitemapLinks(int sitemapId, IEnumerable<SitemapLink> links)
        {
            var sitemap = _context.Sitemaps.Include(s => s.Links).FirstOrDefault(s => s.Id == sitemapId);

            if (sitemap == null) return;

            foreach (var link in links)
            {
                var dbLink = sitemap.Links.FirstOrDefault(l => l.HttpAddress == link.HttpAddress);

                if (dbLink == null)
                {
                    _context.SitemapLinks.Add(link);
                }
                else
                {
                    if (link.ShortestTime <= dbLink.ShortestTime)
                        dbLink.ShortestTime = link.ShortestTime;
                    if (link.LongestTime >= dbLink.LongestTime)
                        dbLink.LongestTime = link.LongestTime;
                }
            }

            _context.SaveChanges();
        }
    }
}
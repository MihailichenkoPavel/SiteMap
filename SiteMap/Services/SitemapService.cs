using PagedList;
using SiteMap.DAL.Entity;
using SiteMap.DAL.Interfaces;
using SiteMap.DAL.Repositories;
using SiteMap.Interfaces;
using System.Collections.Generic;

namespace SiteMap.Services
{
    public class SitemapService : ISitemapService
    {
		private readonly IRepository _repository;
		private const int _pageSize = 10;

        public SitemapService()
        {
			_repository = new Repository();
        }

        public StaticPagedList<Sitemap> GetHistory(int page)
        {
            var sitemaps = _repository.GetSitemaps(_pageSize, (page - 1) * _pageSize);
            return new StaticPagedList<Sitemap>(sitemaps, page, _pageSize, _repository.SitemapsCount());
        }

        public List<SitemapLink> GetSitemapLink(string url)
        {
            var sitemap = _repository.GetSitemap(url);
            return sitemap.Links;
		}

        public List<string> GetParseLinks(IEnumerable<string> urls, string url)
        {
            var links = new List<string>();
			foreach (var u in urls)
			{
                string link = u.ToString();
                if (string.IsNullOrWhiteSpace(link))
					continue;

				if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(url))
				{
					if (link.EndsWith("/"))
						link = url + link;
				}

				if (link.StartsWith(url) && !links.Contains(link))
				{
					links.Add(link);
				}
			}

			return links;
		}
    }
}
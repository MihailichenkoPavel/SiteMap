using PagedList;
using SiteMap.DAL.Entity;
using SiteMap.DAL.Interfaces;
using SiteMap.DAL.Repositories;
using SiteMap.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

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

        private string CheckUrl(string url)
        {
            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1);
            if (url.Contains("https://"))
            {
                if (url.Contains(@"/sitemap.xml") == false)
                {
                    url += "/sitemap.xml";
                }
                else if (url.Contains("sitemap.xml") == false)
                {
                    url += "sitemap.xml";
                }
            }
            else if (url.Contains("http://"))
            {
                if (url.Contains(@"/sitemap.xml") == false)
                {
                    url += "/sitemap.xml";
                }
                else if (url.Contains("sitemap.xml") == false)
                {
                    url += "sitemap.xml";
                }
            }
            else if (url.Contains("https://") == false)
            {
                if (url.Contains(@"/sitemap.xml") == false)
                {
                    url = url.Insert(0, @"https://");
                    url += "/sitemap.xml";
                }
                else if (url.Contains("sitemap.xml") == false)
                {
                    url = url.Insert(0, @"http://");
                    url += "sitemap.xml";
                }
            }
            else if (url.Contains("http://") == false)
            {
                if (url.Contains(@"/sitemap.xml") == false)
                {
                    url = url.Insert(0, @"https://");
                    url += "/sitemap.xml";
                }
                else if (url.Contains("sitemap.xml") == false)
                {
                    url = url.Insert(0, @"http://");
                    url += "sitemap.xml";
                }
            }
            return url;
        }

        private void CheckRobotsTxt(string url)
        {
            url = url.Remove(url.IndexOf("sitemap.xml"));
            url += "robots.txt";
            WebClient web = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string text = web.DownloadString(url);
            string[] arrText = text.Split('\n');
            foreach (string n in arrText)
            {
                if (n.Contains("Sitemap: "))
                {
                    url = n.Remove(0, 9);
                }
            }
        }

        public List<string> ParseXml(string url)
        {
            var correctUrl = CheckUrl(url);
            var parsedUrl = new List<string>();
            XmlDocument obj = new XmlDocument();
            try
            {
                obj.Load(correctUrl);
            }
            catch
            {
                try
                {
                    CheckRobotsTxt(correctUrl);
                    obj.Load(correctUrl);
                }
                catch (Exception ex)
                {
                    var exception = ex.Message;
                }
            }
            XmlNodeList elemList = obj.GetElementsByTagName("loc");
            for (int i = 0; i < elemList.Count; i++)
            {
                parsedUrl.Add(elemList[i].InnerXml);
            }
            return parsedUrl;
        }
    }
}
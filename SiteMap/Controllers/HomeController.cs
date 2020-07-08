using SiteMap.DAL.Entity;
using SiteMap.DAL.Interfaces;
using SiteMap.DAL.Repositories;
using SiteMap.Interfaces;
using SiteMap.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SiteMap.Controllers
{
    public class HomeController : Controller
    {
		private readonly ISitemapService _sitemapService;
		private readonly HttpClient _client;
		private readonly IRepository _repository;

		public HomeController()
        {
			_sitemapService = new SitemapService();
			_client = new HttpClient();
			_repository = new Repository();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult History(int page = 1)
		{
			return View(_sitemapService.GetHistory(page));
		}

		public ActionResult Sitemap(string url)
		{
			var sitemapLinks = _sitemapService.GetSitemapLink(url);
			ViewBag.LinkName_List = string.Join(",", sitemapLinks.Select(c => string.Format("'{0}'", c.HttpAddress)).ToList());
			ViewBag.LongestTime_List = string.Join(",", sitemapLinks.Select(c => c.LongestTime).ToList());
			ViewBag.ShortestTime_List = string.Join(",", sitemapLinks.Select(c => c.ShortestTime).ToList());
			return View("CreateSitemap", sitemapLinks.OrderByDescending(u => u.LongestTime).ToList());
		}

		public async Task<ActionResult> CreateSitemap(string url)
		{
			var entireWatch = Stopwatch.StartNew();
			var links = _sitemapService.ParseXml(url);
			var sitemap = _repository.AddSitemap(url);
			var sitemapLinks = new List<SitemapLink>();
			var tasks = new List<Task>();
			foreach (var link in links)
			{
				var task = new Task(() =>
				{
                    var info = new SitemapLink
                    {
                        HttpAddress = link
                    };
                    var watch = Stopwatch.StartNew();
					var result = _client.GetAsync(link).Result;
					watch.Stop();
					info.ShortestTime = watch.ElapsedMilliseconds;
					info.LongestTime = watch.ElapsedMilliseconds;
					sitemapLinks.Add(info);
					info.Sitemap = sitemap;
				});
				tasks.Add(task);
				task.Start();
				task.Wait();
			}
			entireWatch.Stop();
			_repository.AddSitemapLinks(sitemap.Id, sitemapLinks);

			ViewBag.LinkName_List = string.Join(",", sitemapLinks.Select(c => string.Format("'{0}'", c.HttpAddress)).ToList());
			ViewBag.LongestTime_List = string.Join(",", sitemapLinks.Select(c => c.LongestTime).ToList());
			ViewBag.ShortestTime_List = string.Join(",", sitemapLinks.Select(c => c.ShortestTime).ToList());

			return View(sitemapLinks.OrderByDescending(u => u.LongestTime).ToList());
        }
	}
}
namespace SiteMap.DAL.Entity
{
    public class SitemapLink
    {
        public int Id { get; set; }
        public string HttpAddress { get; set; }
        public double ShortestTime { get; set; }
        public double LongestTime { get; set; }

        public virtual Sitemap Sitemap { get; set; }
    }
}
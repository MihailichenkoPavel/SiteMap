using System;
using System.Collections.Generic;

namespace SiteMap.DAL.Entity
{
    public class Sitemap
    {
        public int Id { get; set; }
        public string HttpAddress { get; set; }
        public DateTime LastRequest { get; set; }

        public virtual List<SitemapLink> Links { get; set; }
    }
}
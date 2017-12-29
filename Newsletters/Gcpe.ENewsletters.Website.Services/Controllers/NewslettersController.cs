using Gcpe.ENewsletters.Data.Entity;
using Gcpe.ENewsletters.Providers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcpe.ENewsletters.Controllers
{
    [Route("newsletters")]
    public class NewslettersController : Controller
    {
        protected readonly ENewslettersEntities db;
        protected readonly Newsroom newsroom;

        public NewslettersController(ENewslettersEntities db)
        {
            this.db = db;
            this.newsroom = new Newsroom(db);
        }
        
        

        [HttpGet]
        [Route(nameof(GetNewslettersPublicOnly))]
        public List<PublicNewsletterListings> GetNewslettersPublicOnly()
        {
            return newsroom.GetNewsletters_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetNewslettersPublicOnlyWithSubscribe))]
        public List<NewsletterList> GetNewslettersPublicOnlyWithSubscribe()
        {
            return newsroom.GetNewsletters_PublicOnly_WithSubscribe();
        }

        [HttpGet]
        [Route(nameof(GetNewslettersByMinistry))]
        public Tuple<string, string>[] GetNewslettersByMinistry(string newsletterIdsCsv)
        {
            return newsroom.GetNewslettersByMinistry(string.IsNullOrEmpty(newsletterIdsCsv) ? new int[0] : newsletterIdsCsv.Split(',').Select(e => int.Parse(e)).ToArray());
        }

        [HttpGet]
        [Route(nameof(GetNewsletter))]
        public Newsletter GetNewsletter(int newsletterId)
        {
            return newsroom.GetNewsletter(newsletterId);
        }

        [HttpGet]
        [Route(nameof(GetEditionsPublicOnly))]
        public List<EditionList> GetEditionsPublicOnly(int newsletterId)
        {
            return newsroom.GetEditions_PublicOnly(newsletterId);
        }

        [HttpGet]
        [Route(nameof(GetEditionsAllPublicOnly))]
        public List<EditionList> GetEditionsAllPublicOnly()
        {
            return newsroom.GetEditions_All_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetEditionBodyKey))]
        public Tuple<string, DateTime> GetEditionBodyKey(string newsletterKey, string editionKey)
        {
            return newsroom.GetEditionBody_Key(newsletterKey, editionKey);
        }

        [HttpGet]
        [Route(nameof(GetNewsletterResourcesByGuid))]
        public NewsletterResource GetNewsletterResourcesByGuid(Guid guid)
        {
            return newsroom.GetNewsletterResourcesByGuid(guid);
        }

        [HttpGet]
        [Route(nameof(GetArticlesAllPublicOnly))]
        public List<ArticleList> GetArticlesAllPublicOnly()
        {
            return newsroom.GetArticles_All_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetArticleBody))]
        public Tuple<string, DateTime> GetArticleBody(string articlePath)
        {
            return newsroom.GetArticleBody(articlePath);
        }

        [HttpGet]
        [Route(nameof(GetNewsUrlFromNewslettersUrl))]
        public string GetNewsUrlFromNewslettersUrl(string url)
        {
            return newsroom.GetNewsUrlFromNewslettersUrl(url);
        }
    }
}

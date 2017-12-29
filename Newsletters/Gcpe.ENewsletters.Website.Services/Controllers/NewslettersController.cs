using Gcpe.ENewsletters.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Gcpe.ENewsletters.Controllers
{
    [Route("newsletters")]
    public class NewslettersController : ApiController
    {
        private readonly Newsroom Newsroom = new Newsroom();

        [HttpGet]
        [Route(nameof(GetNewslettersPublicOnly))]
        public List<PublicNewsletterListings> GetNewslettersPublicOnly()
        {
            return Newsroom.GetNewsletters_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetNewslettersPublicOnlyWithSubscribe))]
        public List<NewsletterList> GetNewslettersPublicOnlyWithSubscribe()
        {
            return Newsroom.GetNewsletters_PublicOnly_WithSubscribe();
        }

        [HttpGet]
        [Route(nameof(GetNewslettersByMinistry))]
        public Tuple<string, string>[] GetNewslettersByMinistry(string newsletterIdsCsv)
        {
            return Newsroom.GetNewslettersByMinistry(string.IsNullOrEmpty(newsletterIdsCsv) ? new int[0] : newsletterIdsCsv.Split(',').Select(e => int.Parse(e)).ToArray());
        }

        [HttpGet]
        [Route(nameof(GetNewsletter))]
        public Newsletter GetNewsletter(int newsletterId)
        {
            return Newsroom.GetNewsletter(newsletterId);
        }

        [HttpGet]
        [Route(nameof(GetEditionsPublicOnly))]
        public List<EditionList> GetEditionsPublicOnly(int newsletterId)
        {
            return Newsroom.GetEditions_PublicOnly(newsletterId);
        }

        [HttpGet]
        [Route(nameof(GetEditionsAllPublicOnly))]
        public List<EditionList> GetEditionsAllPublicOnly()
        {
            return Newsroom.GetEditions_All_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetEditionBodyKey))]
        public Tuple<string, DateTime> GetEditionBodyKey(string newsletterKey, string editionKey)
        {
            return Newsroom.GetEditionBody_Key(newsletterKey, editionKey);
        }

        [HttpGet]
        [Route(nameof(GetNewsletterResourcesByGuid))]
        public NewsletterResource GetNewsletterResourcesByGuid(Guid guid)
        {
            return Newsroom.GetNewsletterResourcesByGuid(guid);
        }

        [HttpGet]
        [Route(nameof(GetArticlesAllPublicOnly))]
        public List<ArticleList> GetArticlesAllPublicOnly()
        {
            return Newsroom.GetArticles_All_PublicOnly();
        }

        [HttpGet]
        [Route(nameof(GetArticleBody))]
        public Tuple<string, DateTime> GetArticleBody(string articlePath)
        {
            return Newsroom.GetArticleBody(articlePath);
        }

        [HttpGet]
        [Route(nameof(GetNewsUrlFromNewslettersUrl))]
        public string GetNewsUrlFromNewslettersUrl(string url)
        {
            return Newsroom.GetNewsUrlFromNewslettersUrl(url);
        }
    }
}

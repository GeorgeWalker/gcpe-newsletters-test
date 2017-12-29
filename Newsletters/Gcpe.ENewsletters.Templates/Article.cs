using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;
    using Model;

    public class Article
    {

       public static string CreateArticle_PublicSite(int articleId, string baseUrl, string getFileLocation)
        {
            string html = CreateArticle(articleId, getFileLocation);

            //we need to do this as the dev/test/uat environments share a database and it stores absolute references
            html = Url.ReplaceEnvironmentURLs_ForPublicSite(html, baseUrl);
            html = Template.RemoveOldUrlsFromImages(html, baseUrl + Template.NEWSLETTERS_FOLDER);
            html = Template.ModifyUrlsInContents(html);

            return html;
        }

        public static string CreateArticle_AdminSite(int articleId, string basePublicUrl, string  baseAdminUrl, string getFileLocation)
        {
            string html = CreateArticle(articleId, getFileLocation);

            html = Url.ReplaceEnvironmentURLs_ForAdminSite(html, basePublicUrl, baseAdminUrl);

            return html;
        }
        
        private static string CreateArticle(int articleId, string getFileLocation)
        {
            string articleName = "";
            DateTime? articleDate;
            string articleContent;

            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                article a = (from x in db.article where x.articleid == articleId select x).FirstOrDefault();
                int editionID = a.editionid.Value;
                articleName = a.name;
                articleDate = a.aritcledate;
                articleContent = a.htmlcomponent.content; 
            }


            string strC = "<div>" + articleContent + "</div>";
            if (articleDate.HasValue)
                strC = "<h5>" + articleDate.Value.ToString("MMM d, yyyy") + "</h5>" + strC;

            if (articleName.Length > 0)
                strC = "<h1>" + articleName + "</h1>" + strC;


            string template = Gcpe.ENewsletters.Templates.Article.ArticleHtmlTemplate(articleId, getFileLocation);
            string html = template.Replace("[phleft]", strC).Replace("[phright]", "");

            return html;
        }

        public static string ArticleHtmlTemplate(int articleId, string getFileLocation)
        {
            string articleTemplate = string.Empty;
            string header = string.Empty;
            string bodyBackground = string.Empty;
            string footer = string.Empty;


            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                newslettertemplate template = (from a in db.article
                                               join e in db.edition on a.editionid equals e.editionid
                                               join nt in db.newslettertemplate on e.newslettertemplateid equals nt.newslettertemplateid
                                               where a.articleid == articleId
                                               select nt).FirstOrDefault();


                articleTemplate = Template.OneColumn_Online; //ALWAYS a one column
                header = Template.GetHeader(template.newslettertemplateid, template.bannerfileid, getFileLocation);
                bodyBackground = Template.GetBodyBackgroundForArticle(template.artcolorid, template.artfileid, template.backgroundcolorid, template.backgroundfileid);
                footer = Template.GetFooter(template, getFileLocation, ViewTarget.Browser); // Articles are always only viewed in a browser

            }

            return Template.FormatNewsletterTemplate(articleTemplate, header, bodyBackground, footer);

        }

        //private static string GetTemplate()
        //{
        //    return Newsletter.OneColumn_Online;

        //    //using (ENewslettersEntities db = new ENewslettersEntities())
        //    //{
        //    //    return (from h in db.htmlcomponents where h.htmlcomid == ArticleFormatID select h.content).FirstOrDefault();
        //    //}
        //}













    }

}

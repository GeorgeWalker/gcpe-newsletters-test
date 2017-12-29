using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;
    using System.Text.RegularExpressions;
    using Model;
    using HtmlAgilityPack;

    public class Edition
    {

        #region "Text Version"

        public static string CreateEdition_EmailVersion_Text(int editionId, string subject, string getFileLocation, string baseUrlForPublicSite)
        {
            string html = CreateTextEditionByEditionId(editionId, getFileLocation, baseUrlForPublicSite);
            html = CleanupEdition_ForPublicSite(html, baseUrlForPublicSite);

            html = StripHTMLTags(html);
            html = Template.RemoveOldUrlsFromImages(html, baseUrlForPublicSite + "newsletters/");
            return html;
        }

        public static string CreateEditionByEditionId_TextFormat_ForAdmin(int editionId, string getFileLocation, string baseUrlForPublicSite, string baseUrlForAdminSite)
        {
            string html = CreateTextEditionByEditionId(editionId, getFileLocation, baseUrlForPublicSite);
            html = CleanupEdition_ForAdminSite(html, baseUrlForPublicSite, baseUrlForAdminSite);
            //html = Url.EncodeUrls_AdminSite(html, true, false, baseUrlForPublicSite, baseUrlForAdminSite);

            html = StripHTMLTags(html);
            return html;
        }

        public static string CreateEditionByTemplateId_TextFormat_ForAdmin(int newsletterTemplateId, IList<BoxContent> bxContent, string getFileLocation, string baseUrlForPublicSite, string baseUrlForAdminSite)
        {

            string html = GetAllBoxesInNewsletter_TextVersion(
                newsletterTemplateId,
                bxContent,
                baseUrlForAdminSite,
                getFileLocation);



            html = CleanupEdition_ForAdminSite(html, baseUrlForPublicSite, baseUrlForAdminSite);
            //html = Url.EncodeUrls_AdminSite(html, true, false, baseUrlForPublicSite, baseUrlForAdminSite);

            html = StripHTMLTags(html); 
            
            return html;
        }

        private static string CreateTextEditionByEditionId(int editionId, string getFileLocation, string baseUrlForPublicSite)
        {
            string editionUrl =
                baseUrlForPublicSite + Url.GetEditionEncodedUrl(editionId.ToString(), true);

            IList<BoxContent> bxContent = Box.BoxContent(editionId);

            string txt = string.Empty;
            string subject = string.Empty;
            string name = string.Empty;

            int newsletterTemplateId;
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                var x = (from e in db.editions
                         where e.editionid == editionId
                         select new
                         {
                             e.subject,
                             e.name,
                             e.newslettertemplateid
                         }).FirstOrDefault();
                subject = x.subject;
                name = x.name;
                newsletterTemplateId = x.newslettertemplateid.Value;
            }

            txt += "Online Version: " + editionUrl + "\r\n";
            txt = subject + "\r\n" + name + "\r\n" +
                "--------------------------------------------------------------------------\r\n\r\n";

            txt += GetAllBoxesInNewsletter_TextVersion(
                newsletterTemplateId,
                bxContent,
                baseUrlForPublicSite,
                getFileLocation);

            

            return txt;
        }


        #endregion

     
        #region "HTML Version - For Public Site"

        /// <summary>
        /// Gets an HTML edition formatted for public
        /// </summary>
        /// <param name="editionId"></param>
        /// <param name="getFileLocation"></param>
        /// <param name="imagePathLocation"></param>
        /// <param name="cornerUrlFormat"></param>
        /// <returns></returns>
        public static Tuple<string, DateTime> CreateEditionByEditionId_HtmlFormat_ForPublic(int editionId, string getFileLocation, string imagePathLocation)
        {

            int newsletterTemplateId = Template.GetNewsletterTemplateId(editionId);

            string template = Template.NewsletterTemplate(newsletterTemplateId, getFileLocation, ViewTarget.Browser);

            IList<BoxContent> bxContentLeft = Box.BoxContent(editionId, 1); // All Boxes in the Left Column
            IList<BoxContent> bxContentRight = Box.BoxContent(editionId, 2); // All Boxes in the Right Column

            string html = AddContentToTemplate(newsletterTemplateId,
                template,
                bxContentLeft,
                bxContentRight,
                getFileLocation + Template.NEWSLETTERS_FOLDER,
                imagePathLocation);

            html = CleanupEdition_ForPublicSite(html, getFileLocation);
            html = Template.RemoveOldUrlsFromImages(html);
            html = Template.ModifyUrlsInContents(html);

            var maxDateBxContentLeft = bxContentLeft.Where(x => x != null).Max(x => x.BoxDate);
            var maxDateBxContentRight = bxContentRight.Where(x => x != null).Max(x => x.BoxDate);

            DateTime newestDateTime;
            if(maxDateBxContentLeft.HasValue && maxDateBxContentRight.HasValue)
            {
                newestDateTime = maxDateBxContentLeft.Value > maxDateBxContentRight.Value ? maxDateBxContentLeft.Value : maxDateBxContentRight.Value;
            } else if (maxDateBxContentLeft.HasValue && !maxDateBxContentRight.HasValue)
            {
                newestDateTime = maxDateBxContentLeft.Value;
            }
            else if (!maxDateBxContentLeft.HasValue && maxDateBxContentRight.HasValue)
            {
                newestDateTime = maxDateBxContentRight.Value;
            }
            else
            {   //default date if lastupdated/created field is null.
                newestDateTime = new DateTime(1900, 1, 1);
            }


            return new Tuple<string, DateTime> (html, newestDateTime);
        }

        #endregion

        #region "HTML Version - For Email Distribution"

        public static string CreateEdition_EmailVersion_InHtml(int editionId, 
            string subject, string baseUrlForPublicSite, string baseUrlForAdminSite, bool isTest, string publicImagePathLocation)
        {
            string editionPublicUrl =
                baseUrlForPublicSite + Url.GetEditionEncodedUrl(editionId.ToString(), true);

            string problemViewingNote = "<table><tr><td style=\"font-family: Verdana, Arial, Helvetica, sans-serif; font-size:10px;padding-bottom:4px;\">If you are having problems viewing this e-newsletter, go to the <a href=\"" + editionPublicUrl + "\">Online version</a>.</td></tr></table>";

            int newsletterTemplateId = Template.GetNewsletterTemplateId(editionId);

            string template = Template.NewsletterTemplate(newsletterTemplateId, baseUrlForPublicSite, ViewTarget.Email);
            
            IList<BoxContent> bxContentLeft = Box.BoxContent(editionId, 1); // All Boxes in the Left Column
            IList<BoxContent> bxContentRight = Box.BoxContent(editionId, 2); // All Boxes in the Right Column

            string bodyHtml = AddContentToTemplate(
                newsletterTemplateId, 
                template,
                bxContentLeft,
                bxContentRight,
                baseUrlForPublicSite,
                publicImagePathLocation, true);

            bodyHtml = CleanHtml_EmailPrep(bodyHtml);


            bodyHtml = Styles.EmbedStyleTagsInContent(bodyHtml);


            if (isTest)
            {
                bodyHtml = CleanupEdition_ForAdminSite(bodyHtml, baseUrlForPublicSite, baseUrlForAdminSite);
                bodyHtml = Url.EncodeUrls_AdminSite(bodyHtml, false, false, baseUrlForPublicSite, baseUrlForAdminSite);
                
                if (bodyHtml.Contains(baseUrlForPublicSite.TrimEnd('/') + "/getfile.aspx"))
                    bodyHtml = bodyHtml.Replace(baseUrlForPublicSite.TrimEnd('/') + "/getfile.aspx", baseUrlForAdminSite.TrimEnd('/') + "/pages/getfile.aspx");
            }

            else
            {
                bodyHtml = CleanupEdition_ForPublicSite(bodyHtml, baseUrlForPublicSite);

                //TODO: This was from the other project...is this already covered?
                bodyHtml = bodyHtml.Replace("=\"getfile.aspx", "=\"" + baseUrlForPublicSite + "getfile.aspx");
                bodyHtml = bodyHtml.Replace("(getfile.aspx", "(" + baseUrlForPublicSite + "getfile.aspx");
                bodyHtml = bodyHtml.Replace("=\"images/", "=\"" + baseUrlForPublicSite + "images/");
            }

            bodyHtml = problemViewingNote + bodyHtml;

            //Manage subscription footer            
            bodyHtml = bodyHtml.Insert(bodyHtml.LastIndexOf("</table>"), 
                                       string.Format(@"<tr style = ""background-color:#f4f4f4;"">
                                                                <td style = ""font-family: MyriadProRegular, Calibri, Arial, sans serif; font-size: 13px; width:100%; text-align:center;"">
                                                                    <a style = ""color: #7a6e67;"" href =  ""{0}/subscribe/manage""> Manage your subscription</a>
                                                                </td>
                                                       </tr>", baseUrlForPublicSite));

            bodyHtml = Template.RemoveOldUrlsFromImages(bodyHtml, baseUrlForPublicSite + "newsletters/");
            return Email.HtmlWrapper(subject, bodyHtml, baseUrlForPublicSite, isTest);
        }
     
        #endregion

        #region "HTML Version - For Admin Site"

        /// <summary>
        /// Gets an HTML edition formatted for admin site
        /// </summary>
        /// <param name="editionId"></param>
        /// <param name="viewAs"></param>
        /// <param name="getFileLocation"></param>
        /// <param name="imagePathUrl"></param>
        /// <param name="cornerUrlFormat"></param>
        /// <param name="baseUrlForPublicSite"></param>
        /// <param name="baseUrlForAdminSite"></param>
        /// <returns></returns>
        public static string CreateEditionByEditionId_AdminSite(int editionId,ViewTarget viewAs,string getFileLocation,string imagePathUrl, string baseUrlForPublicSite, string baseUrlForAdminSite)
        {
            int newsletterTemplateId = Template.GetNewsletterTemplateId(editionId);

            string template = Template.NewsletterTemplate(newsletterTemplateId, baseUrlForPublicSite, viewAs);

            IList<BoxContent> bxContentLeft = Box.BoxContent(editionId, 1); // All Boxes in the Left Column
            IList<BoxContent> bxContentRight = Box.BoxContent(editionId, 2); // All Boxes in the Right Column

            string html = AddContentToTemplate(
                newsletterTemplateId,
                template,
                bxContentLeft,
                bxContentRight,
                getFileLocation,
                imagePathUrl, (viewAs == ViewTarget.Email ? true : false));


            if (viewAs == ViewTarget.Email)
            {
                //TODO: check footer links in the emails, need to embed those styles.
                html = Styles.EmbedStyleTagsInContent(html);
                html =
                    "<style type=\"text/css\" media=\"all\">" +
                    " body, .body { " + Styles.body + "}" + //p, div
                    " table { font-size:inherit; } " +
                    " .edition-footer-links { min-height:42px; text-align:center; padding:10px 5px 10px 5px; font-family:Verdana; color:GrayText; text-align:center; } " +
                    " .edition-footer-links a {  line-height:1.5em; text-decoration: underline; color: GrayText; } " +
                    "</style>" +
                    Styles.Stylesheet_Print +
                    "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"backgroundTable\" style=\"font-size:12px;margin:0;padding:0;width:586px;\">" +
                    "<tr><td>" +
                    "<div style=\"width: 100%; background-color: lemonchiffon; padding: 5px;font-family: Verdana, Arial; font-size:12px; color: #404040;margin-bottom:10px;\">" +
                     "Please note: sending yourself a test edition by email is the best way to preview the email format." +
                    "</div>" +
                    html +
                    "</td></tr>" +
                    "</table>";     
            }
            

            html = CleanupEdition_ForAdminSite(html, baseUrlForPublicSite, baseUrlForAdminSite);

            html = Template.RemoveOldUrlsFromImages(html, baseUrlForPublicSite + "newsletters/");
   
            return html;
        }

        /// <summary>
        /// This one passes in the columns from session
        /// </summary>
        /// <param name="editionId"></param>
        /// <param name="baseUrl"></param>
        /// <param name="leftColumnContent"></param>
        /// <param name="rightColumnContent"></param>
        /// <returns></returns>
        public static string CreateEditionByTemplateId_ForAdmin(int newsletterTemplateId, IList<BoxContent> bxs, ViewTarget viewAs,string getFileLocation, string imagePathUrl, string baseUrlForPublicSite,string baseUrlForAdminSite)
        {
            string template = Template.NewsletterTemplate(newsletterTemplateId, baseUrlForPublicSite, viewAs);

            IList<BoxContent> bxContentLeft = bxs.Where(x => x.Column == 1).Select(x => x).OrderBy(x => x.Sort).ToList();
            IList<BoxContent> bxContentRight = bxs.Where(x => x.Column == 2).Select(x => x).OrderBy(x => x.Sort).ToList();

            string html = AddContentToTemplate(newsletterTemplateId, template, bxContentLeft, bxContentRight,getFileLocation, imagePathUrl);

            if (viewAs == ViewTarget.Email)
            {
                //TODO: check footer links in the emails, need to embed those styles.
                html = Styles.EmbedStyleTagsInContent(html);
                html =
                    "<style type=\"text/css\" media=\"all\">" +
                    " body, .body { " + Styles.body + "}" + //p, div
                    " table { font-size:inherit; } " +
                    " .edition-footer-links { min-height:42px; text-align:center; padding:10px 5px 10px 5px; font-family:Verdana; color:GrayText; text-align:center; } " +
                    " .edition-footer-links a {  line-height:1.5em; text-decoration: underline; color: GrayText; } " +
                    "</style>" +
                    Styles.Stylesheet_Print +
                    "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"backgroundTable\" style=\"font-size:12px;margin:0;padding:0;width:586px;\">" +
                    "<tr><td>" +
                    "<div style=\"width: 100%; background-color: lemonchiffon; padding: 5px;font-family: Verdana, Arial; font-size:12px; color: #404040;margin-bottom:10px;\">" +
                     "Please note: sending yourself a test edition by email is the best way to preview the email format." +
                    "</div>" +
                    html +
                    "</td></tr>" +
                    "</table>";
            }
            

            html = CleanupEdition_ForAdminSite(html, baseUrlForPublicSite, baseUrlForAdminSite);

            return html;
        }
    
        /// <summary>
        /// Preview template when creating a newsletter template
        /// </summary>
        /// <param name="htmlComponentTemplateId"></param>
        /// <param name="header"></param>
        /// <param name="background"></param>
        /// <param name="footer"></param>
        /// <param name="leftColumnContent"></param>
        /// <param name="rightColumnContent"></param>
        /// <returns></returns>
        public static string CreateEditionByTemplateTypeId_ForAdmin(int htmlComponentTemplateId, 
            int? newsletterTemplateId, 
            IList<BoxContent> bxs, 
            string header, 
            string background, string footer,
            string getFileLocation, string imagePathUrl, string baseUrlForAdminSite)
        {
            string template = Template.GetNewsletterTemplate(htmlComponentTemplateId, ViewTarget.Browser);
            template = Template.FormatNewsletterTemplate(template, header, background, footer);

            string html = string.Empty;

            if (newsletterTemplateId.HasValue)
            {
                IList<BoxContent> bxContentLeft = bxs.Where(x => x.Column == 1).Select(x => x).OrderBy(x => x.Sort).ToList();
                IList<BoxContent> bxContentRight = bxs.Where(x => x.Column == 2).Select(x => x).OrderBy(x => x.Sort).ToList();
                
                html = AddContentToTemplate(newsletterTemplateId.Value, template, bxContentLeft, bxContentRight, getFileLocation, imagePathUrl);
            }
            else
            {
                html = template;
            }
            return html;
        }
    

        #endregion


        private static string AddContentToTemplate(int newsletterTemplateId, string newsletterTemplate, IList<BoxContent> leftColBoxes, IList<BoxContent> rightColBoxes, string getFileLocation, string imagePathUrl, bool isEmail = false)
        {
            string leftColumn = GetAllBoxesInColumn(newsletterTemplateId, leftColBoxes, getFileLocation, imagePathUrl, isEmail);
            string rightColumn = GetAllBoxesInColumn(newsletterTemplateId, rightColBoxes, getFileLocation, imagePathUrl, isEmail);

            string html = newsletterTemplate;
            html = html.Replace("[phleft]", (leftColumn.Length == 0 ? "&nbsp;" : leftColumn));
            html = html.Replace("[phright]", (rightColumn.Length == 0 ? "&nbsp;" : rightColumn));
            return html;
        }


        private static string GetAllBoxesInNewsletter_TextVersion(int newsletterTemplateId, IList<BoxContent> bxs, string baseUrl, string getFileLocation)
        {
            IList<BoxContent> parentBoxes = bxs.Where(x => x.MasterBoxContentId.HasValue == false).Select(x => x).OrderBy(x => x.Column).ThenBy(x => x.Sort).ToList();

            string columnContent = "";
            foreach (BoxContent bx in parentBoxes)
            {
                if (bx.IsMarkDeleted)
                    continue;

                string bxContent = Gcpe.ENewsletters.Templates.Box.FillBoxWithContent_TextVersion(getFileLocation, baseUrl, baseUrl, bx);
                IList<BoxContent> bxContentSubRws = Gcpe.ENewsletters.Templates.Box.BoxReplicants(bx.BoxContentId);
                foreach (BoxContent rw in bxContentSubRws)
                    bxContent += "\r\n" + Gcpe.ENewsletters.Templates.Box.FillBoxWithContent_TextVersion(getFileLocation, baseUrl, baseUrl, rw);

                columnContent += bxContent +
                    "\r\n--------------------------------------------------------------------------\r\n\r\n";
            }

            return columnContent;
        }

        public static string GetAllBoxesInColumn(int newsletterTemplateId, 
            IList<BoxContent> bxs,
            string getFileLocation, 
            string imagePathUrl, 
            bool isEmail = false)
        {

            IList<BoxContent> parentBoxes = bxs.Where(x => x.MasterBoxContentId.HasValue == false).Select(x => x).OrderBy(x => x.Sort).ToList();

            string columnContent = "";
            foreach (BoxContent bx in parentBoxes)
            {
                if (bx.IsMarkDeleted)
                    continue;

                string bxContent = Box.FillBoxWithContent(bx, getFileLocation, imagePathUrl, isEmail);

                //---- sub box 
                IList<BoxContent> bxContentSubRws = Box.BoxReplicants(bx.BoxContentId);                 
                foreach (BoxContent rw in bxContentSubRws)
                    bxContent += "\n" + Box.FillBoxWithContent(rw, getFileLocation, imagePathUrl, isEmail);
                
                columnContent += Box.FormatBox(bx.BoxContentId,
                        bx.Style,
                        bxContent, isEmail);

                // space between boxes
                columnContent += "<img src=\"" + imagePathUrl + "clear.gif\" width=\"100%\" height=\"6px\">";
                //columnContent += "<table width=\"100%\" height=\"6px\" style=\"width:100%;height:6px;\"><tr><td></td></tr></table>";

            }
            return columnContent;
        }



        #region "Clean up Helpers"

        public static string CleanupEdition_ForAdminSite(string htmlContent, string baseUrlForPublicSite,string baseUrlForAdminSite)
        {
            string txt = htmlContent;
            txt = Gcpe.ENewsletters.Templates.Url.ReplaceEnvironmentURLs_ForAdminSite(txt, baseUrlForPublicSite, baseUrlForAdminSite);
            txt = CleanupSubscribeUnSubscribeUrls(txt, baseUrlForPublicSite);
            return txt;           
        }

        private static string CleanupSubscribeUnSubscribeUrls(string htmlContent, string baseUrl)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            foreach (var a in doc.DocumentNode.Descendants("a"))
            {
                var hrefAttribute = a.Attributes["href"];

                if (hrefAttribute == null || hrefAttribute.Value == null)
                    continue;

                if (hrefAttribute.Value.IndexOf("unsubscribe", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    hrefAttribute.Value = baseUrl + "/subscribe/manage";
                    continue;
                }

                if (hrefAttribute.Value.IndexOf("subscribe", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    hrefAttribute.Value = baseUrl + "/subscribe?newsletters=";
                    continue;
                }
            }

            return doc.DocumentNode.OuterHtml;
        }


        private static string CleanupEdition_ForPublicSite(string html, string baseUrl)
        {
            string strTmp = html;

            // Deal with environment specific images
            strTmp = Url.ReplaceEnvironmentURLs_ForPublicSite(strTmp, baseUrl + Template.NEWSLETTERS_FOLDER.TrimStart('/')); 

            strTmp = strTmp.Replace(baseUrl + "pages/getfile.aspx", baseUrl + "getfile.aspx");

            strTmp = CleanupSubscribeUnSubscribeUrls(strTmp, baseUrl);
 
            // Encode all the URLs (Articles & Images)
            // ie. Change getfile.aspx?guid=123 to /NewsletterName/EditionName/ArticleName
            strTmp = Url.EncodeUrls_PublicSite(strTmp, baseUrl + Template.NEWSLETTERS_FOLDER.TrimStart('/'));

            strTmp = strTmp.Replace("/newsletters//newsletters/", "/newsletters/");

            return strTmp;
        }


        private static string CleanHtml_EmailPrep(string content)
        {
            string txt = content;
            txt = txt.Replace("<p> </p>", "<p>&nbsp;</p>");

            //Go through and put newline tags in so htmlcode is not so long
            txt = txt.Replace("<span", "\n<span");
            return txt;
        }


        private static string StripHTMLTags(string content)
        {
            string txt = content;

            txt = txt.Replace("<p>", "\n\n");
            //strEmail = Regex.Replace(strEmail, "<br>|<br />", "\n", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "<[^>]+>", "", RegexOptions.IgnoreCase);

            txt = Regex.Replace(txt, "<img [^>]+>|<a [^>]+>[^>]+>", new MatchEvaluator(CapText), RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "<b>|<i>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "&nbsp;", " ", RegexOptions.IgnoreCase);
            return txt;
        }

        private static string CapText(Match m)
        {
            string x = m.ToString().Trim();
            string strName;
            string strID;
            if (x.IndexOf("<img") == 0)
            {
                x = Regex.Match(x, "src=\"[^\\\"| ]+\\\"", RegexOptions.IgnoreCase).ToString();
                x = Regex.Replace(x, "src=\\\"|\\\"", "");
                strID = Regex.Match(x, "guid=[^\\&]+\\&").ToString();
                strID = Regex.Replace(strID, "guid=|\\&", "");

                if (strID == "")
                    strName = "";
                else
                    strName = GetNameByGuid(strID);

                x = strName + ">> " + x;
            }
            else
                if (x.IndexOf("<a") == 0)
                {
                    strName = Regex.Match(x, ">[^<]+<").ToString();
                    strName = Regex.Replace(strName, ">|<", "", RegexOptions.IgnoreCase);

                    x = Regex.Match(x, "href=\"[^\\\"| ]+\\\"", RegexOptions.IgnoreCase).ToString();
                    x = Regex.Replace(x, "href=\\\"|\\\"", "");

                    x = strName + ">> " + x;
                }



            return x;
        }

        private static string GetNameByGuid(string id)
        {
            string name = string.Empty;

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                var file = (from f in db.files
                            where f.guid == new Guid(id)
                            select f).FirstOrDefault();
                if (file != null)
                    name = file.name;
            }

            return name;
        }



        #endregion



    }


}

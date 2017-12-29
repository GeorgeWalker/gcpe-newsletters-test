using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;
    using HtmlAgilityPack;
    using Model;
    using System.Text.RegularExpressions;

    public class Template
    {
        //private static int ArticleFormatID = 23; //Always a one column

        public const string BASE_URL_TO_REPLACE = "<!--REPLACE-WITH-PUBLIC-URL-->";
        public const string NEWSLETTERS_FOLDER = "/newsletters/";
        public const string BASE_NEWSLETTER_URL_TO_REPLACE = BASE_URL_TO_REPLACE + NEWSLETTERS_FOLDER;
        public const string BASE_CONTENT_URL_TO_REPLACE = BASE_URL_TO_REPLACE + "/Content/Images/";

        public static bool IsOneColumn(int newsletterTemplateHtmlComponentId)
        {
            switch (newsletterTemplateHtmlComponentId)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOneColumnByNewsletter(int newsletterTemplateId)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                int newsletterTemplateHtmlComponentId = (from nt in db.newslettertemplate 
                                                where nt.newslettertemplateid == newsletterTemplateId
                                                select nt.htmlcomid).FirstOrDefault();
                return IsOneColumn(newsletterTemplateHtmlComponentId);
            }
        }



        public static int GetNewsletterTemplateId(int editionId)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                return (from e in db.edition
                        where e.editionid == editionId
                        select e.newslettertemplateid.Value).FirstOrDefault();


            }
        }


        public static string NewsletterTemplate(int newsletterTemplateId, string getFileLocation, ViewTarget viewType)
        {
            string editionTemplate = string.Empty;
            string header = string.Empty;
            string bodyBackground = string.Empty;
            string footer = string.Empty;

            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {

                newslettertemplate nTemplate = (from nt in db.newslettertemplate 
                                                where nt.newslettertemplateid == newsletterTemplateId
                                                select nt).FirstOrDefault();

                // Carolynn had issues with this not loading the most recent copy
                // I believe this was because we are using half entity framework and half not
                // so forcing reload in case they changed the template type (for editions in draft you can still edit the template)
                db.Entry(nTemplate).Reload(); 

                editionTemplate = GetNewsletterTemplate(nTemplate.htmlcomid, viewType);
                header = Template.GetHeader(nTemplate.newslettertemplateid, nTemplate.bannerfileid, getFileLocation);
                bodyBackground = GetBodyBackground(nTemplate.backgroundcolorid, nTemplate.backgroundfileid);
                footer = GetFooter(nTemplate, getFileLocation, viewType);
            }

            return FormatNewsletterTemplate(editionTemplate, header, bodyBackground, footer);
        }

        public static string GetNewsletterTemplate(int htmlComponentTemplateId, ViewTarget viewType)
        {
            string template = string.Empty;

            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                template = (from nt in db.htmlcomponent
                      where nt.htmlcomid == htmlComponentTemplateId
                      select nt.content).FirstOrDefault();

                if (viewType == ViewTarget.Browser)
                {
                    Model.NewsletterTemplateTypes ntType = (Model.NewsletterTemplateTypes)htmlComponentTemplateId;
                    switch (ntType)
                    {
                        case Model.NewsletterTemplateTypes.Two_Column_Equal_TemplateId:
                            template = TwoColumn_Equal_Online;
                            break;
                        case Model.NewsletterTemplateTypes.Two_Column_LeftBigger_TemplateId:
                            template = TwoColumn_LeftBigger_Online;
                            break;
                        case Model.NewsletterTemplateTypes.Two_Column_RightBigger_TemplateId:
                            template = TwoColumn_RightBigger_Online;
                            break;
                        case Model.NewsletterTemplateTypes.One_Column_TemplateId:
                        default:
                            template = OneColumn_Online;
                            break;
                    }
                }

            }
            return template;
        }

        public static string FormatNewsletterTemplate(string newsletterTemplate, string header, string bodyBackground, string footer)
        {
            string htmlTemplate = newsletterTemplate;
            htmlTemplate = htmlTemplate.Replace("[phheader]", header);
            htmlTemplate = htmlTemplate.Replace("[background]", bodyBackground);
            htmlTemplate = htmlTemplate.Replace("[phfooter]", footer);
            htmlTemplate = htmlTemplate.Replace("[border]", string.Empty);
            return htmlTemplate;
        }




        public static string GetHeader(int newsletterTemplateId, int? bannerFileId, string getFileLocation)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                List<LinkArea> coords = (from h in db.NewsletterTemplatesBannerImage where h.newslettertemplateid == newsletterTemplateId select new LinkArea { Coordinates = h.coordinates, LinkUrl = h.url }).ToList();
                return GetHeader(bannerFileId, coords, getFileLocation);
            }
        }

        public static string GetHeader(int? bannerFileId, List<LinkArea> coords, string getFileLocation)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                file f = (from x in db.file where x.fileid == bannerFileId.Value select x).FirstOrDefault();

                string areaMap = string.Empty;
                foreach (LinkArea link in coords)
                    areaMap += string.Format("<area shape=\"rect\" coords=\"{0}\" href=\"http://{1}\" target=\"_blank\" alt=\"{2}\" />"
                        , link.Coordinates, link.LinkUrl, link.LinkUrl);

                return string.Format(""
                               + "     <img src=\"{0}getfile.aspx?guid={1}\" style=\"border:none;display:block;\" alt=\"\" usemap=\"#BCHome\" />"                               
                               + "     <map id=\"BCHome\" name=\"BCHome\">{2}</map>"
                               , getFileLocation
                               , f.guid
                               , areaMap);
            }
        }


        private static string GetBodyBackground(int? backgroundColorId, int? backgroundFileId)
        {
            if (backgroundColorId.HasValue)
                return Template.GetBackgroundColorStyle(backgroundColorId.Value);
            else if (backgroundFileId.HasValue)
                return Template.GetBackgroundImageStyle(backgroundFileId.Value);
            else
                return string.Empty;
        }

        public static string GetBodyBackgroundForArticle(int? articleBackgroundColorId, int? articleBackgroundFileId, int? editionBackgroundColorId, int? editionBackgroundFileId)
        {

            if (articleBackgroundColorId.HasValue)
                return Template.GetBackgroundColorStyle(articleBackgroundColorId.Value);
            else if (articleBackgroundFileId.HasValue)
                return Template.GetBackgroundImageStyle(articleBackgroundFileId.Value);
            else if (editionBackgroundColorId.HasValue)
                return Template.GetBackgroundColorStyle(editionBackgroundColorId.Value);
            else if (editionBackgroundFileId.HasValue)
                return Template.GetBackgroundImageStyle(editionBackgroundFileId.Value);
            else
                return string.Empty;

        }


        private static string GetBackgroundColorStyle(int colorId)
        {
            string style = string.Empty;
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                color clr = (from f in db.color where f.colorid == colorId select f).FirstOrDefault();
                string hex = Utility.GetHexFromRGB(clr.red.Value, clr.green.Value, clr.blue.Value);
                style = "background-color: #" + hex + ";";
            }
            return style;
        }
        private static string GetBackgroundImageStyle(int imageId)
        {
            string style = string.Empty;
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                Guid? fileId = (from f in db.file where f.fileid == imageId select f.guid).FirstOrDefault();
                style = String.Format("background-image:url(getfile.aspx?guid={0})", fileId.Value);
            }
            return style;
        }


        public static string GetFooter(newslettertemplate template, string getFileLocation, ViewTarget viewType)
        {
            string footer = "";
            if (template.footerfileid.HasValue)
                footer = Template.GetFooterWithImageWithEmbededLinks(
                    template.newslettertemplateid,
                    template.footerfileid.Value,
                    getFileLocation,
                    viewType);
            else
                footer = Template.GetFooterWithColor(template.newslettertemplateid,
                    template.footercolorid.Value,
                    template.footer_area_link,
                    viewType);

            return footer;
        }

        private static string GetFooterWithColor(int templateId, int? colorId, string arealink, ViewTarget viewType)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                string hexValue = "ffffff";

                if (colorId.HasValue)
                {
                    color c = (from x in db.color where x.colorid == colorId select x).FirstOrDefault();
                    hexValue = Utility.GetHexFromRGB(c.red.Value, c.green.Value, c.blue.Value);
                }
                
                List<FooterLink> footerLinks = (from x in db.NewsletterTemplatesFooterLink where x.newslettertemplateid == templateId orderby x.link_order, x.newslettertemplatefooterlinkid select new FooterLink { DisplayText = x.link_text, LinkUrl = x.link_url,LinkText=x.link_text }).ToList();

                return GetFooterWithColor(templateId, hexValue, arealink, footerLinks, viewType);
            }

        }


        public static string GetFooterWithColor(int? templateId, string hexValue, string arealink, List<FooterLink> footerLinks, ViewTarget viewType)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                /*int distributionListId = -1; //The only time there isn't a value for templateId is when creating an new template
                
                if(templateId.HasValue)
                    distributionListId = (from x in db.newsletters
                                          join y in db.newslettertemplates on x.newsletterid equals y.newsletterid
                                          where y.newslettertemplateid == templateId
                                          && x.distributionlistid.HasValue
                                          select x.distributionlistid.Value).FirstOrDefault();*/

                string linksHtml = Template.GetFooterLinkHtml(footerLinks, viewType);

                if (viewType == ViewTarget.Email)
                    return String.Format(""
                                   + "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:586px; height:84px;\">" + Environment.NewLine
                                   + "<tr>"
                                   + "<td style=\"height:42px; text-align:center; background-color:#{0};\">" + Environment.NewLine
                                   + "<a href=\"http://{1}\" style=\"color:white; text-decoration:none; font-family:Verdana; font-size:12px;\" target=\"_blank\">" + Environment.NewLine
                                   + "{1}</a></td>" + Environment.NewLine
                                   + "</tr>" + Environment.NewLine
                                   + "<tr>" + Environment.NewLine
                                   + "<td style=\"height:42px; text-align:center; vertical-align:middle; font-family:Verdana; font-size:12px; color:GrayText;\">" + Environment.NewLine
                                   + "{2}"
                                   + "</td></tr></table>",
                                   hexValue,
                                   arealink,
                                   linksHtml);
                else
                    return String.Format(""
                                   + "<div style=\"height:42px; width:100%; text-align:center; background-color:#{0}; line-height:42px;\">"
                                   + "     <a href=\"http://{1}\" target=\"_blank\">{1}</a>"
                                   + "</div>"
                                   + "<div class=\"edition-footer-links\">{2}</div>",
                                   hexValue,
                                   arealink,
                                   linksHtml);

            }

        }




        
        public static string GetFooterWithImageWithEmbededLinks(int templateId, int fileId, string getFileLocation, ViewTarget viewType)
        {

            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                List<LinkArea> imageLinks = (from x in db.NewsletterTemplatesFooterImage where x.newslettertemplateid == templateId select new LinkArea { LinkUrl = x.url, Coordinates = x.coordinates }).ToList();
                List<FooterLink> footerLinks = (from x in db.NewsletterTemplatesFooterLink where x.newslettertemplateid == templateId orderby x.link_order select new FooterLink { DisplayText = x.link_text, LinkUrl = x.link_url }).ToList();

                return GetFooterWithImageWithEmbededLinks(templateId, fileId, imageLinks, footerLinks, getFileLocation, viewType);
            }

        }


        public static string GetFooterWithImageWithEmbededLinks(int templateId, int fileId, List<LinkArea> imageLinks, List<FooterLink> footerLinks, string getFileLocation, ViewTarget viewType)
        {

            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                file f = (from x in db.file where x.fileid == fileId select x).FirstOrDefault();

                /*int distributionListId = (from x in db.newsletters
                                          join y in db.newslettertemplates on x.newsletterid equals y.newsletterid
                                          where y.newslettertemplateid == templateId
                                          && x.distributionlistid.HasValue
                                          select x.distributionlistid.Value).FirstOrDefault();*/

                string linksHtml = Template.GetFooterLinkHtml(footerLinks, viewType);

                string embeddedImageLinks = "";
                foreach (LinkArea fil in imageLinks)
                    embeddedImageLinks += "<area shape=\"rect\" coords=\"" + fil.Coordinates + "\" href=\"http://" + fil.LinkUrl + "\" target=\"_blank\" alt=\"" + fil.LinkUrl + "\" />" + Environment.NewLine;

                if (viewType == ViewTarget.Email)
                    return String.Format(""
                                    + "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:586px; height:{0}px;font-size:inherit\"> " + Environment.NewLine
                                    + "<tr>"
                                    + "<td style=\"height:{0}px; text-align:center;\"> " + Environment.NewLine
                                    + "<img src=\"{1}getfile.aspx?guid={2}\" border=\"0\" usemap=\"#EditionFooter\" /> " + Environment.NewLine
                                    + string.Format("<map id=\"EditionFooter\" name=\"EditionFooter\">{0}</map>", embeddedImageLinks) + Environment.NewLine
                                    + "</td>" + Environment.NewLine
                                    + "</tr>" + Environment.NewLine
                                    + "<tr> " + Environment.NewLine
                                    + "<td style=\"height:42px; text-align:center; vertical-align:middle; font-family:Verdana; font-size:12px; color:GrayText;\">" + Environment.NewLine
                                    // modified because footer was left aligned in test email -- now center aligned
                                    //+ "<td class=\"edition-footer-links\"> " + Environment.NewLine
                                    + linksHtml + Environment.NewLine
                                    + "</td>" + Environment.NewLine
                                    + "</tr></table>",
                                    f.height,
                                    getFileLocation,
                                    f.guid);
                else
                    return String.Format(""
                                    + "<div style=\"max-height:{0}px;\"> " + Environment.NewLine
                                    + "<img src=\"{1}getfile.aspx?guid={2}\" border=\"0\" usemap=\"#EditionFooter\" /> " + Environment.NewLine
                                    + string.Format("<map id=\"EditionFooter\" name=\"EditionFooter\">{0}</map>", embeddedImageLinks) + Environment.NewLine
                                    + "</div>" + Environment.NewLine
                                    + "<div class=\"edition-footer-links\"> " + Environment.NewLine
                                    + linksHtml + Environment.NewLine
                                    + "</div>" + Environment.NewLine
                                    ,
                                    f.height,
                                    getFileLocation,
                                    f.guid);
            }

        }





        public static string GetFooterLinkHtml(List<FooterLink> footerLinks, ViewTarget viewType)
        {
            string footerLinkUrls = "";
            string url = "";
            foreach (FooterLink fi in footerLinks)
            {
                if (fi.LinkUrl == Utility.SUBSCRIBE_SYSTEM_GENERATED)
                {
                    // no longer add subscribe to the footers
                    continue;
                }
                else if (fi.LinkUrl == Utility.UNSUBSCRIBE_SYSTEM_GENERATED)
                {
                    // no longer add unsubscribe to the footers
                    continue;
                }
                else if (fi.DisplayText == "Privacy Statement" && fi.LinkUrl == @"http://www.gov.bc.ca/com/priv/")
                {
                    // no longer add privacy statement if default value
                    continue;
                }
                else if (fi.DisplayText == "Contact Info" && (fi.LinkUrl == @"www.gov.bc.ca/contacts" || fi.LinkUrl == ""))
                {
                    // no longer add Contact info if default value or blank
                    continue;
                }
                else if (fi.LinkText == "Subscribe")
                {
                   continue;
                }
                else if (fi.LinkText == "Unsubscribe")
                {
                   continue;
                }

                else
                {
                    url = fi.LinkUrl;

                    int urlHasHttp = url.ToLower().IndexOf("http");
                    if (urlHasHttp < 0)
                        url = "http://" + url;

                }
                footerLinkUrls += "<a href=\"" + url + "\" target=\"_blank\" style=\"color:#808080;\">" + fi.DisplayText + "</a>" + Environment.NewLine;
                if (footerLinks[footerLinks.Count - 1] != fi && viewType == ViewTarget.Email)
                    footerLinkUrls += "&nbsp;&nbsp;|&nbsp;&nbsp;";
            }
            return footerLinkUrls;
        }

        public static string RemoveOldUrlsFromImages(string content, string baseUrlForPublicSite = BASE_NEWSLETTER_URL_TO_REPLACE)
        {
            var regexMatchGuid = new Regex(@".*([0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}).?", RegexOptions.Compiled);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            foreach (var a in doc.DocumentNode.Descendants("img"))
            {
                var srcAttribute = a.Attributes["src"];

                if (srcAttribute == null || srcAttribute.Value == null)
                    continue;

                Match guidMatch = regexMatchGuid.Match(srcAttribute.Value);
                if (guidMatch.Success)
                {
                    var guid = guidMatch.Groups[1].Value;
                    srcAttribute.Value = baseUrlForPublicSite + "image/" + guid;
                }
            }

            foreach (var a in doc.DocumentNode.Descendants("a"))
            {
                var srcAttribute = a.Attributes["href"];

                if (srcAttribute == null || srcAttribute.Value == null)
                    continue;

                Match guidMatch = regexMatchGuid.Match(srcAttribute.Value);
                if (guidMatch.Success)
                {
                    var guid = guidMatch.Groups[1].Value;
                    srcAttribute.Value = baseUrlForPublicSite + "file/" + guid;
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        public static string ModifyUrlsInContents(string bodyHtmlString)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(bodyHtmlString.ToString());

            var nodes = doc.DocumentNode
                .Descendants("a")
                .Where(n => n.Attributes.Any(a => a.Name == "target"))
                .ToArray();

            foreach (var node in nodes)
            {
                node.SetAttributeValue("target", "_self");
            }

            return doc.DocumentNode.InnerHtml;
        }

        #region "Newsletter Templates - Should these go in Database?"

        //private static string OneColumn_Email = 
        //    "<table style=\"border: 0px; padding: 0px; margin: 0; font-family:verdana,helvetica,arial,clean,sans-serif; color:#666666;font-size:12px;width:586px;border-collapse: collapse;\">"
        //     + "<tr><td style=\"width: 100%; margin: 0px; padding: 0px;\">"
        //     + "    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"padding:0px; font-size: inherit;\"><tr><td>" 
        //     + "    [phheader]" 
        //     + "    </td></tr></table>" 
        //     + "</td></tr>"
        //     + "<tr><td style=\"width:100%; height:100%; vertical-align:top;[background]\">"
        //     + "      <table style=\"width:100%;font-size:inherit;\"><tr><td style=\"padding:6px 6px 6px 6px;\">[phleft]</td></tr></table>" 
        //     + "</td></tr>"
        //     + "<tr style=\"position:relative;font-size: inherit; \"><td style=\"width: 100%; margin: 0px; padding: 0px;\">" 
        //     + "    [phfooter]"
        //     + "</td></tr>"
        //     + "</table>";

        public static string OneColumn_Online =
            "<style type=\"text/css\" media=\"all\">" + Styles.Stylesheet_Common + Styles.Stylesheet_OneColumn + "</style>" +
            Styles.Stylesheet_Print +
            "<div id=\"edition-wrapper\">" +
            "   <div class=\"edition-header\">[phheader]</div>" +
            "   <div class=\"edition-content\">[phleft]</div>" +
            "   <div class=\"edition-footer\">[phfooter]</div>" +
            "</div>";


        //private static string TwoColumn_Equal_Email = 
        //    "<table style=\"border: 0px; padding: 0px; margin: 0; font-family:verdana,helvetica,arial,clean,sans-serif; color:#666666;font-size:12px;width:586px;border-collapse: collapse;\">"
        //     + "<tr><td style=\"width: 100%; margin: 0px; padding: 0px;\" colspan=\"2\">"
        //     + "    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"padding:0px; font-size: inherit;\"><tr><td>" 
        //     + "    [phheader]" 
        //     + "    </td></tr></table>" 
        //     + "</td></tr>"
        //     + "<tr>" 
        //     + "<td style=\"width:50%; padding:6px 5px 6px 6px; vertical-align:top;[border];[background]\">[phleft]</td>"
        //     + "<td style=\"width:50%; padding:6 6 6 5; vertical-align:top;[background]\">[phright]</td>"
        //     + "</tr>"   
        //     + "<tr style=\"position:relative;\"><td colspan=\"2\">[phfooter]</td></tr>"
        //     + "</table>";


        private static string TwoColumn_Equal_Online =
            "<style type=\"text/css\" media=\"all\">" + Styles.Stylesheet_Common + Styles.Stylesheet_TwoColumn_Equal + "</style>" +
            Styles.Stylesheet_Print +
            "<div id=\"edition-wrapper\">" +
            "   <div class=\"edition-header\">[phheader]</div>" +
            "   <div class=\"edition-content\">" +
            "       <div class=\"edition-content-left\">[phleft]</div>" +
            "       <div class=\"edition-column-spacer\"></div>" +
            "       <div class=\"edition-content-right\">[phright]</div>" +
            "       <div class=\"clear\"></div>" +
            "   </div>" +
            "   <div class=\"edition-footer\">[phfooter]</div>" +
            "</div>";




        private static string TwoColumn_LeftBigger_Online =
            "<style type=\"text/css\" media=\"all\">" + Styles.Stylesheet_Common + Styles.Stylesheet_TwoColumn_LeftBigger + "</style>" +
            Styles.Stylesheet_Print +
            "<div id=\"edition-wrapper\">" +
            "   <div class=\"edition-header\">[phheader]</div>" +
            "   <div class=\"edition-content\">" +
            "       <div class=\"edition-content-left\">[phleft]</div>" +
            "       <div class=\"edition-column-spacer\"></div>" +
            "       <div class=\"edition-content-right\">[phright]</div>" +
            "       <div class=\"clear\"></div>" +
            "   </div>" +
            "   <div class=\"edition-footer\">[phfooter]</div>" +
            "</div>";


        private static string TwoColumn_RightBigger_Online =
            "<style type=\"text/css\" media=\"all\">" + Styles.Stylesheet_Common + Styles.Stylesheet_TwoColumn_RightBigger + "</style>" +
            Styles.Stylesheet_Print +
            "<div id=\"edition-wrapper\">" +
            "   <div class=\"edition-header\">[phheader]</div>" +
            "   <div class=\"edition-content\">" +
            "       <div class=\"edition-content-left\">[phleft]</div>" +
            "       <div class=\"edition-column-spacer\"></div>" +
            "       <div class=\"edition-content-right\">[phright]</div>" +
            "       <div class=\"clear\"></div>" +
            "   </div>" +
            "   <div class=\"edition-footer\">[phfooter]</div>" +
            "</div>";



        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Styles for Newsletters - Email or Browser
    /// EMAILS: Styles for are embeded <h1 style=""></h1>
    /// BROWSERS: Styles are shown via CSS h1 { ... } and then just <h1></h1> 
    /// MISC NOTES/DECISIONS:
    ///     Font-size:inherit is newer, but ignored by some browsers, so continue to use font-size:100%; 
    ///     Internet Explorer versions before IE9 do not support media queries
    /// </summary>
    public class Styles
    {

        //These are used to display the styles correctly as in the database only <h1> is stored
        public static string fontH1 = "color:#234075; font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size: 1.5em; font-weight:bold;margin-bottom:12px;margin-top:4px;";
        public static string fontH2 = "color:#234075; font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size:1.4em; font-weight:bold; margin-bottom:12px;margin-top:4px;";
        public static string fontH3 = "color:#234075; font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size:1.3em; font-weight:bold; margin-bottom:12px;margin-top:4px;";
        public static string fontH4 = "color:#234075; font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size:1.2em; font-weight:bold; margin-bottom:12px;margin-top:4px;";
        public static string fontH5 = "color:#234075; font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size:1.1em; font-weight:bold; margin-bottom:12px;margin-top:4px;";
        public static string fontH6 = "font-family: Verdana, Arial, Helvetica, sans-serif; line-height:1; font-size:1em; font-weight:bold; margin-bottom:12px;";
        public static string cutline = "font-size:100%;color:#333;font-family:verdana,helvetica,arial,clean,sans-serif;line-height:1.1;font-style:italic;";
        public static string body = "font-size:100%;color:#333;font-family:verdana,helvetica,arial,clean,sans-serif;line-height:1.4;";
        public static string boxTextNormal = "font-family:verdana,helvetica,arial,clean,sans-serif; color:#666;font-size: 90%; ";


        public static string Stylesheet_OneColumn =
            " @media screen and (max-width: 450px) { " +
            " } " +
            "";

        public static string Stylesheet_TwoColumn_Equal =
           " .edition-content-left { width:49.5%; vertical-align:top; float:left; }" +
            " .edition-content-right { width:49.5%; vertical-align:top; float:left;}" +
            " .edition-column-spacer {  width: 1%; height: 10px; float:left;  } " +
            " @media screen and (max-width: 450px) { " +
            "   .edition-content-left, .edition-content-right { width: 100%; } " +
            "   .edition-column-spacer { display:none;} " +
            " } " +
            "";

        public static string Stylesheet_TwoColumn_LeftBigger =
            " .edition-content-left { width: 55.5%; vertical-align:top; float:left; }" +
            " .edition-content-right { width: 43.5%; margin: 0; vertical-align:top; float:left; }" +
            " .edition-column-spacer {  width: 1%; height: 10px; float:left;  } " +
            " .edition-footer {  } " +
            " @media screen and (max-width: 450px) { " +
            "   .edition-content-left, .edition-content-right { width: 100%; } " +
            "   .edition-column-spacer { display:none;} " +
            " } " +
            "";

        public static string Stylesheet_TwoColumn_RightBigger =
            " .edition-content-left { width: 43.5%;  vertical-align:top; float:left; }" +
            " .edition-content-right { width: 55.5%; margin: 0; vertical-align:top; float:right; }" +
            " .edition-column-spacer {  width: 1%; height: 10px; float:left;  } " +
            " .edition-footer {  } " +
            " @media screen and (max-width: 450px) { " +
            "   .edition-content-left, .edition-content-right { width: 100%; } " +
            "   .edition-column-spacer { display:none;} " +
            " } " +
            "";



        //.boxFontH1, .body, .cutline etc are OLD styles we need to keep around
        public static string Stylesheet_Common =
            // Sets min and max width of newsletter and font size
            " #edition-wrapper, .edition-content { max-width:586px; min-width:300px;  } " +

            // banner/header style
            " #edition-wrapper img[usemap] { border: none; height: auto; max-width: 100%; width: auto; }" +
            " #edition-wrapper .edition-header { width: 100%;  } " +

            //content area
            " #edition-wrapper .edition-content { width:100%; height: auto; display:block; padding-left:1%; padding-right: 1%; padding-top:6px; padding-bottom: 6px; vertical-align:top; [background] }" +
            " #edition-wrapper .edition-content .print-box img, " +
            " #edition-wrapper .edition-content-right .print-box img, " +
            " #edition-wrapper .edition-content-left .print-box img  " +
            "    { max-width:100%!important; height:auto!important;  width:auto!important; } " +

            //Content boxes are too wide in preview
            " #edition-wrapper .edition-content { box-sizing: border-box; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; } " +

            " #edition-wrapper .print-box { overflow:hidden; } " +

            " .video-container {  }" + //#edition-wrapper  position: relative; overflow: hidden;
            " .video-container iframe { max-width: 100%; height: 100%;}" + //#edition-wrapper  position: absolute; top:0; left: 0; 

            " #edition-wrapper a { color:#0000f3;  } " + // added this in to override the admin site link colors text-decoration:underline;

            " #edition-wrapper .edition-footer { width: 100%; text-align:center; position:relative; font-size:12px; } " +
            " #edition-wrapper .edition-footer a { color:white; text-decoration:none; } " +
            " #edition-wrapper .edition-footer a:hover { text-decoration:underline; } " +
            " #edition-wrapper .edition-footer img { border:0px;padding:0px; margin:0px; } " +
            " #edition-wrapper .edition-footer-links { min-height:42px; text-align:center; padding:10px 5px 10px 5px; font-family:Verdana; color:GrayText; text-align:center; } " +
            " #edition-wrapper .edition-footer-links a {  line-height:1.5em; text-decoration: underline; color: #808080; padding-left: 10px; padding-right: 5px; border-left: 1px solid #808080; } " +
            " #edition-wrapper .edition-footer-links a:first-child { border-left: 0px; } " +
            " #edition-wrapper h1, #edition-wrapper .boxFontH1 { " + Styles.fontH1 + "}" +
            " #edition-wrapper h2, #edition-wrapper .boxFontH2 {" + Styles.fontH2 + "}" +
            " #edition-wrapper h3, #edition-wrapper .boxFontH3 {" + Styles.fontH3 + "}" +
            " #edition-wrapper h4, #edition-wrapper .boxFontH4 {" + Styles.fontH4 + "}" +
            " #edition-wrapper h5, #edition-wrapper .boxFontH5 {" + Styles.fontH5 + "}" +
            " #edition-wrapper h6, #edition-wrapper .boxFontH6 {" + Styles.fontH6 + "}" +
            " #edition-wrapper, #edition-wrapper .body { " + Styles.body + " font-size:11px!important;}" + //p, div
            " #edition-wrapper table { font-size:inherit; table-layout: fixed; } " +
            " #edition-wrapper .cutline {" + Styles.cutline + "}" +
            " #edition-wrapper .boxTextNormal {" + Styles.boxTextNormal + "}" +
            " #edition-wrapper .clear { clear: both; } " +
            " @media screen and (max-width: 450px) { " +
            "   #edition-wrapper, #edition-wrapper .body, p {  } " +
            "   .read-more { display:block; margin-top: 7px; } " +
            "   #edition-wrapper .edition-footer-links a { width: 100%; display:block; border-left:0px; border-bottom: 1px solid #e5e5e5; padding: 8px 3px; } " +
            "   #edition-wrapper .edition-footer-links { text-align: left; } " +
            " } " +
            "";


        public static string Stylesheet_Print =
                "<style type=\"text/css\" media=\"print\">"
                    + ".print-box{ position:relative!important; top: 0!important; }"
                    + ".box { position:absolute; float:none; }"
                + "</style>";




        public static string EmbedStyleTagsInContent(string content)
        {
            string temp = content;


            // This is for old content, before the WYSIWIG styles pointed to a css file class, now the styles are embedded
            temp = temp.Replace("class=\"boxFontH1\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH1 + "\"");
            temp = temp.Replace("class=\"boxFontH2\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH2 + "\"");
            temp = temp.Replace("class=\"boxFontH3\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH3 + "\"");
            temp = temp.Replace("class=\"boxFontH4\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH4 + "\"");
            temp = temp.Replace("class=\"boxFontH5\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH5 + "\"");
            temp = temp.Replace("class=\"boxFontH6\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH6 + "\"");
            temp = temp.Replace("class=\"body\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.body + "\"");
            temp = temp.Replace("class=\"cutline\"", "style=\"" + Gcpe.ENewsletters.Templates.Styles.cutline + "\"");

            // New editor only uses h1 instead of <h1 class="boxFont1"> so we need replace these too
            // We don't have cutline now so it's not there
            // ** Updated to search the whole tag in case the heading tag has a style and is not just <h1>

            //temp = temp.Replace("<h1>", "<h1 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH1 + "\">");
            //temp = temp.Replace("<h2>", "<h2 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH2 + "\">");
            //temp = temp.Replace("<h3>", "<h3 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH3 + "\">");
            //temp = temp.Replace("<h4>", "<h4 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH4 + "\">");
            //temp = temp.Replace("<h5>", "<h5 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH5 + "\">");
            //temp = temp.Replace("<h6>", "<h6 style=\"" + Gcpe.ENewsletters.Templates.Styles.fontH6 + "\">");

            List<string> header1Tag = FetchTagFromSource("h1", temp);
            List<string> header2Tag = FetchTagFromSource("h2", temp);
            List<string> header3Tag = FetchTagFromSource("h3", temp);
            List<string> header4Tag = FetchTagFromSource("h4", temp);
            List<string> header5Tag = FetchTagFromSource("h5", temp);
            List<string> header6Tag = FetchTagFromSource("h6", temp);

            temp = ReplaceTagStyle(header1Tag, "h1", Gcpe.ENewsletters.Templates.Styles.fontH1, temp);
            temp = ReplaceTagStyle(header2Tag, "h2", Gcpe.ENewsletters.Templates.Styles.fontH2, temp);
            temp = ReplaceTagStyle(header3Tag, "h3", Gcpe.ENewsletters.Templates.Styles.fontH3, temp);
            temp = ReplaceTagStyle(header4Tag, "h4", Gcpe.ENewsletters.Templates.Styles.fontH4, temp);
            temp = ReplaceTagStyle(header5Tag, "h5", Gcpe.ENewsletters.Templates.Styles.fontH5, temp);
            temp = ReplaceTagStyle(header6Tag, "h6", Gcpe.ENewsletters.Templates.Styles.fontH6, temp);



            return temp;

        }


        public static List<string> FetchTagFromSource(string tag, string htmlSource)
        {
            List<string> links = new List<string>();
            string regexImg = new Regex("<(" + tag + ")\\b[^>]*>").ToString();
            MatchCollection matchesImg = Regex.Matches(htmlSource, regexImg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImg)
            {
                links.Add(m.Value);
            }
            return links;
        }
        public static string ReplaceTagStyle(List<string> oldTags, string newTagPrefix, string newTagStyle, string htmlSource)
        {

            string temp = htmlSource;

            foreach (string tag in oldTags)
            {
                string fixedTag = tag;

                string regexStyle = @"(?<=style="").*?(?="")";
                Match matchIsStyleTag = Regex.Match(tag, regexStyle, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (matchIsStyleTag.Length > 0)
                {
                    string styleTag = newTagStyle + matchIsStyleTag.Value.ToString();
                    fixedTag = string.Format("<{0} style=\"{1}\">", newTagPrefix, styleTag);
                }
                else
                {
                    fixedTag = string.Format("<{0} style=\"{1}\">", newTagPrefix, newTagStyle);
                }

                temp = temp.Replace(tag, fixedTag);
            }

            return temp;

        }








    }

}

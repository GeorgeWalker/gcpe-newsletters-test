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
    using Gcpe.ENewsletters.Templates.BoxType;

    public class Box
    {

        public static IList<BoxTypeOption> GetAllBoxTypeOptions()
        {
            IList<BoxTypeOption> options = new List<BoxTypeOption>();
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Picture_And_Article_Box, Name = PictureAndArticleBox.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Picture_And_Article_Box_With_Edging, Name = PictureArticleBoxWithEdging.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Picture_With_Title_And_Date_Box, Name = PictureWithTitleAndDateBox.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Button_Box, Name = ButtonBox.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Button_Box_With_Transparent_Edges, Name = ButtonBoxWithTransparentEdges.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Ministers_Message_Box, Name = MinistersMessageBox.Description });
            options.Add(new BoxTypeOption() { ID = BoxTypeOptions.Header_Box, Name = HeaderBox.Description });
            return options;
        }


        public static BoxContent ExampleContent(int newsletterBoxId, string ImagePathUrl)
        {
            BoxTypeOptions bxType;
            BoxStyle bxStyle;
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var dbVals = (from newsbx in db.newsletterboxes

                              join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                              from backgroundcolor in cs.DefaultIfEmpty()

                              where newsbx.newsletterboxid == newsletterBoxId
                              select new
                              {
                                  BoxType = newsbx.boxtypeid.Value,
                                  Style = new BoxStyle
                                  {
                                      NewsletterBoxID = newsbx.newsletterboxid,
                                      IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                      CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid),
                                      Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                      Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                      Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255)
                                  }
                              }
                ).FirstOrDefault();
                bxType = (BoxTypeOptions)dbVals.BoxType;
                bxStyle = dbVals.Style;
            }
            BoxContent bxContent = new BoxContent();
            bxContent.Style = bxStyle;
            bxContent.IsMarkDeleted = false;
            bxContent.NewsletterBoxID = newsletterBoxId;
            bxContent.BoxTypeId = bxType;
            bxContent.MasterBoxContentId = null;


            //Title = "",
            //PictureName = "",
            //PictureAltText = "",
            //BoxDate = null,
            //HyperlinkTarget = "",
            //Content = "",
            //Hyperlink = "", 
            //ReadMoreLink = "" 


            switch (bxType)
            {
                case BoxTypeOptions.Button_Box:
                    ButtonBox.ExampleContent(bxContent, ImagePathUrl);
                    break;
                case BoxTypeOptions.Button_Box_With_Transparent_Edges:
                    ButtonBoxWithTransparentEdges.ExampleContent(bxContent, ImagePathUrl);
                    break;
                case BoxTypeOptions.Header_Box:
                    HeaderBox.ExampleContent(bxContent);
                    break;
                case BoxTypeOptions.Ministers_Message_Box:
                    MinistersMessageBox.ExampleContent(bxContent, ImagePathUrl);
                    break;
                case BoxTypeOptions.Picture_And_Article_Box:
                    PictureAndArticleBox.ExampleContent(bxContent, ImagePathUrl);
                    break;
                case BoxTypeOptions.Picture_And_Article_Box_With_Edging:
                    PictureArticleBoxWithEdging.ExampleContent(bxContent, ImagePathUrl);
                    break;
                case BoxTypeOptions.Picture_With_Title_And_Date_Box:
                    PictureWithTitleAndDateBox.ExampleContent(bxContent, ImagePathUrl);
                    break;
            }
            return bxContent;
        }




        public static string FillBoxWithContent(BoxContent bxContent,
            string GetFileLocation,
            string ImagePathUrl, bool isEmail = false)
        {
            switch (bxContent.BoxTypeId)
            {
                case BoxTypeOptions.Button_Box:
                    return ButtonBox.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl);

                case BoxTypeOptions.Button_Box_With_Transparent_Edges:
                    return ButtonBoxWithTransparentEdges.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl);

                case BoxTypeOptions.Header_Box:
                    return HeaderBox.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl, isEmail);

                case BoxTypeOptions.Ministers_Message_Box:
                    return MinistersMessageBox.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl, isEmail);

                case BoxTypeOptions.Picture_And_Article_Box:
                    return PictureAndArticleBox.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl, isEmail);

                case BoxTypeOptions.Picture_And_Article_Box_With_Edging:
                    return PictureArticleBoxWithEdging.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl, isEmail);

                case BoxTypeOptions.Picture_With_Title_And_Date_Box:
                    return PictureWithTitleAndDateBox.FillHtmlTemplate(bxContent, GetFileLocation, ImagePathUrl, isEmail);

                default:
                    return "";
            }

        }

        public static string FillBoxWithContent_TextVersion(
            string getFileLocation,
            string baseUrl,
            string imagePathUrl,
            BoxContent bxContent)
        {


            switch (bxContent.BoxTypeId)
            {
                case BoxTypeOptions.Button_Box:
                    return ButtonBox.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Button_Box_With_Transparent_Edges:
                    return ButtonBoxWithTransparentEdges.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Header_Box:
                    return HeaderBox.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Ministers_Message_Box:
                    return MinistersMessageBox.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Picture_And_Article_Box:
                    return PictureAndArticleBox.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Picture_And_Article_Box_With_Edging:
                    return PictureArticleBoxWithEdging.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                case BoxTypeOptions.Picture_With_Title_And_Date_Box:
                    return PictureWithTitleAndDateBox.FillTextTemplate(bxContent, getFileLocation, imagePathUrl, baseUrl);

                default:
                    return "";
            }

        }








        // The only change is that I had to add table-layout:fixed for allowing images to scale        /// <summary>
        /// Replaces [picture], [width], [height], [picturealttext]
        /// </summary>
        /// <param name="GetFileLocation"></param>
        /// <param name="ImagePathUrl"></param>
        /// <param name="PictureName"></param>
        /// <returns></returns>
        public static string FormatImageInfoInBox(BoxContent bxContent, string bxHtml, string GetFileLocation, string ImagePathUrl)
        {
            string htmlWithImageInfo = bxHtml;

            if (bxContent.PictureName == null)
                bxContent.PictureName = string.Empty;

            switch (bxContent.PictureName.ToLower().Trim())
            {
                case "/images/imagesizestandards.jpg":
                case "images/imagesizestandards.jpg":
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[picture]", ImagePathUrl + "imagesizestandards.jpg");
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[width]", "170");
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[height]", "100");
                    break;

                case "/images/ministerboxsizestandards.jpg":
                case "images/ministerboxsizestandards.jpg":
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[picture]", ImagePathUrl + "ministerboxsizestandards.jpg");
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[width]", "100");
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[height]", "125");
                    break;

                case "":
                    htmlWithImageInfo = htmlWithImageInfo
                        .Replace("<img src=\"[picture]\" alt=\"[picturealttext]\" />", "")
                        .Replace("<img src=\"[picture]\" style=\"border: 1;\" alt=\"[picturealttext]\" />", "")
                        .Replace("<img src=\"[picture]\" style=\"border: none;\" alt=\"[picturealttext]\" />", "")
                        .Replace("<img src=\"[picture]\" style=\"border: 1;\" alt=\"[picturealttext]\" width=\"[width]\" height=\"[height]\" />", "")
                        .Replace("[picture]", ImagePathUrl + "clear.gif");
                    break;

                default:

                    htmlWithImageInfo = htmlWithImageInfo.Replace("[picture]", Utility.MakePictureSrc(GetFileLocation, bxContent.PictureName.ToString()));


                    string widthheightinfo = bxContent.PictureName.ToString().Substring(bxContent.PictureName.ToString().IndexOf("&w=") + 3);
                    string widthinfo = string.Empty;

                    if (widthheightinfo.IndexOf("&h=") > -1)
                        widthinfo = widthheightinfo.Remove(widthheightinfo.IndexOf("&h="));


                    string heightinfo = string.Empty;

                    if (widthheightinfo.IndexOf("&h=") > -1)
                        heightinfo = widthheightinfo.Substring(widthheightinfo.IndexOf("&h=") + 3);


                    int width;
                    int height;
                    if (int.TryParse(widthinfo, out width) == false || int.TryParse(heightinfo, out height) == false)
                    {
                        htmlWithImageInfo = htmlWithImageInfo.Replace("[width]", "");
                        htmlWithImageInfo = htmlWithImageInfo.Replace("[height]", "");
                    }
                    else
                    {
                        htmlWithImageInfo = htmlWithImageInfo.Replace("[width]", widthinfo);
                        htmlWithImageInfo = htmlWithImageInfo.Replace("[height]", heightinfo);
                    }

                    htmlWithImageInfo = htmlWithImageInfo.Replace("alt=\"[picturealttext]\"", "alt=\"[picturealttext]\" title=\"[picturealttext]\"");
                    htmlWithImageInfo = htmlWithImageInfo.Replace("[picturealttext]", bxContent.PictureAltText);

                    break;
            }


            return htmlWithImageInfo;
        }


        public static string FormatContent(BoxContent bxContent, string html, string GetFileLocation, string ImagePathUrl, bool isEmail)
        {
            string bxHtml = html;

            if (bxContent.Content == null
                || bxContent.Content.ToString() == "")
                bxHtml = bxHtml.Replace("[content]", "Content area." + bxContent.BoxContentId.ToString());

            else
            {


                string strBoxContent = bxContent.Content.ToString();

                // Check for images, for the responsive we need to update them with a max-width if they specified the size.
                if (strBoxContent.IndexOf("<img") > 0)
                {
                    strBoxContent = FixImageForResponsive(strBoxContent);
                }
                if (strBoxContent.IndexOf("<iframe") > 0)
                {
                    if (isEmail)
                        strBoxContent = ReplaceVideosWithImages(strBoxContent, ImagePathUrl);
                    else
                        strBoxContent = FormatIframeForResponsive(strBoxContent);
                }

                if(strBoxContent.IndexOf("<td") > 0)
                {
                    strBoxContent = FixTablesForEmail(strBoxContent);
                }


                string strTemp = "";
                int intPos = 0;

                if (bxContent.ReadMoreLink == null
                    || bxContent.ReadMoreLink.Trim() == string.Empty)
                {


                    if (bxContent.BoxTypeId == BoxTypeOptions.Picture_With_Title_And_Date_Box)
                    {
                        intPos = strBoxContent.LastIndexOf("</p>");
                        if (intPos != -1)
                            strBoxContent = strBoxContent.Insert(intPos, "<br><hr style=\"border: 0px; height:1px; background-color: #999999; \" />");
                        else
                            strBoxContent += "<br><hr style=\"border: 0px; height:1px; background-color: #999999;\" />";
                    }


                }
                else
                {
                    strTemp = "&nbsp; <a target=\"_blank\" href=\"" + GetFileLocation + bxContent.ReadMoreLink.ToString().Trim() + "\" class=\"read-more\"><b><i>Read more</i></b></a>";

                    if (bxContent.BoxTypeId == BoxTypeOptions.Picture_With_Title_And_Date_Box)
                        strTemp += "<br><hr style=\"border: gray 1px solid;\" />";

                    if (strBoxContent.EndsWith("</p>"))
                    {
                        intPos = strBoxContent.LastIndexOf("</p>");
                        if (intPos != -1)
                            strBoxContent = strBoxContent.Insert(intPos, strTemp);
                        else
                            strBoxContent += strTemp;
                    }
                    else
                        strBoxContent += strTemp;


                }

                bxHtml = bxHtml.Replace("[content]", strBoxContent);
            }

            return bxHtml;
        }

        /// <summary>
        /// Prior to Nov 2014 Heading 1 color was color:#039 
        /// We have changed the main stylesheet but there are some colors embedded
        /// into older newsletters that need to do a full search and replace
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FixHeadingColor(string content)
        {
            string updatedContent = content.Replace("#039", "#234075").Replace("#003399", "#234075").Replace("rgb(0, 51, 153)", "#234075");

            return updatedContent;
        }

        /// <summary>
        /// We want images to scale down
        /// In some cases where someone used an image that was 300px, and they specified it to be 200px, then the auto-sizing was not working
        /// Here we are adding the max-width for the image
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string FixImageForResponsive(string content)
        {
            List<string> imageTagsInContent = FetchLinksFromSource(content);
            foreach (string img in imageTagsInContent)
            {
                bool hasStyleTag = img.ToLower().Contains("style=");
                string fixedImageTag = img;

                string regexImgWidth = @"(?<=width="").*?(?="")";
                string regexImgHeight = @"(?<=height="").*?(?="")";

                Match matchImgWidth = Regex.Match(img, regexImgWidth, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match matchImgHeight = Regex.Match(img, regexImgHeight, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (matchImgWidth.Length > 0)
                {
                    string width = matchImgWidth.Value.ToString().Replace("px", "");
                    string height = (matchImgHeight.Length > 0 ? matchImgHeight.Value : string.Empty);

                    // found one with a slash in it...odd.
                    //TODO: look into why sometimes the width has a / at the end of it!
                    string fixedWidth = width.Replace("/", "");


                    //fixedImageTag = "<div style=\"width:" + fixedWidth + "px;max-width:" + fixedWidth + "px!important;\">" + fixedImageTag + "</span>";
                    //WorldHost_e-News/WorldHost_e-News_September_2013/edition

                    if (hasStyleTag == false) // easy peasy
                        fixedImageTag = fixedImageTag
                            .Replace(width, fixedWidth)
                            .Replace("<img", "<img style=\"max-width:" + fixedWidth + "px!important;width: 100%;height: auto; \"")
                            .Replace("width=\"" + fixedWidth + "\"", "")
                            .Replace("height=\"" + height + "\"", "");


                }

                content = content.Replace(img, fixedImageTag);
            }

            return content;
        }

        private static string FixTablesForEmail(string content)
        {
            List<string> tableCells = FetchTdsromSource(content);

            foreach (string td in tableCells)
            {
                string fixedTdTag = td;

                string regexStyle = @"(?<=style="").*?(?="")";
                Match matchImgWidth = Regex.Match(td, regexStyle, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (matchImgWidth.Length > 0)
                {
                    string styleTag = matchImgWidth.Value.ToString() + ";font-family: Verdana, Arial; font-size:11px;color:#333333;";
                    fixedTdTag = string.Format("<td style=\"{0}\">", styleTag);
                }
                else
                {
                    fixedTdTag = string.Format("<td style=\"{0}\">", ";font-family: Verdana, Arial; font-size:11px;color:#333333;");
                }

                content = content.Replace(td, fixedTdTag);
            }

            return content;
        }

        /// <summary>
        /// We want images to scale down
        /// In some cases where someone used an image that was 300px, and they specified it to be 200px, then the auto-sizing was not working
        /// Here we are adding the max-width for the image
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string FormatIframeForResponsive(string content)
        {
            List<string> iframeTagsInContent = FetchIFrameFromSource(content);
            foreach (string img in iframeTagsInContent)
            {
                string format = "<div class=\"video-container\" style=\"max-width:{0};height:{1};\">{2}</div>";
                //TODO: What should be the default iframe size?
                string width = "100%";
                string height = "100%";
                string regexImgWidth = @"(?<=width="").*?(?="")";
                string regexImgHeight = @"(?<=height="").*?(?="")";
                Match matchImgWidth = Regex.Match(img, regexImgWidth, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match matchImgHeight = Regex.Match(img, regexImgHeight, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (matchImgWidth.Length > 0)
                    width = matchImgWidth.Value.ToString() + "px";
                if (matchImgHeight.Length > 0)
                    height = matchImgHeight.Value.ToString() + "px";

                content = content.Replace(img + "</iframe>", string.Format(format, width, height, img + "</iframe>"));
            }

            return content;
        }

        public static string ReplaceVideosWithImages(string content, string imagePathUrl)
        {
            List<string> iframeTagsInContent = FetchIFrameFromSource(content);
            foreach (string iframe in iframeTagsInContent)
            {
                string width = "100%";
                string height = "100%";
                string srcOfIframe = string.Empty;
                string videoId = string.Empty;

                string regexSrc = @"(?<=src="").*?(?="")";
                string regexImgWidth = @"(?<=width="").*?(?="")";
                string regexImgHeight = @"(?<=height="").*?(?="")";
                Match matchSrc = Regex.Match(iframe, regexSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match matchImgWidth = Regex.Match(iframe, regexImgWidth, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match matchImgHeight = Regex.Match(iframe, regexImgHeight, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (matchSrc.Length > 0)
                    srcOfIframe = matchSrc.Value.ToString().Replace("https://", "").Replace("http://", "").Replace("//", "").Replace("www.", "");
                if (matchImgWidth.Length > 0)
                    width = matchImgWidth.Value.ToString();
                if (matchImgHeight.Length > 0)
                    height = matchImgHeight.Value.ToString();
                Match regexMatch = Regex.Match(srcOfIframe, @"(?:youtu\.be\/|youtube.com\/(?:watch\?.*\bv=|embed\/|v\/)|ytimg\.com\/vi\/)(.+?)(?:[^-a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                if (regexMatch.Success)
                    videoId = regexMatch.Groups[1].Value;

                if (videoId != string.Empty)
                {
                    string link = string.Format("http://www.youtube.com/watch?v={0}", videoId);
                    string imageSrc = string.Format("<img src=\"http://img.youtube.com/vi/{0}/0.jpg\" width=\"{1}\" height=\"{2}\" />", videoId, width, height);

                    string linkTo = string.Format("<div style=\"color:#ffffff;\">Watch video >> <a href=\"{0}\" target=\"_blank\" style=\"color:#ffffff;\">{0}</a></div>", link);
                    string replacment = string.Format("<table cellpadding=\"0\" style=\"margin:0px;padding:0px;border-collapse:collapse;\">" +
                                            "<tr><td><a href=\"{0}\" target=\"_blank\">{1}</a></td></tr>" +
                                            "<tr>" +
                                            "<td style=\"background-color:#191919;width:100%;\">" +
                                            "<table><tr>" +
                                            "<td><a href=\"{0}\" target=\"_blank\"><img src=\"" + imagePathUrl + "youtubeplay.png\" width=\"46\" height=\"25\" /></a></td>" +
                                            "<td style=\"padding-top:5px; padding-bottom: 5px;padding-left:5px;padding-right:5px;\">{2}</td>" +
                                            "</tr></table>" +
                                            "</td>" +
                                            "</tr></table>",
                                            link, imageSrc, linkTo);

                    content = content.Replace(iframe, replacment);
                }


            }
            return content;
        }

        public static List<string> FetchLinksFromSource(string htmlSource)
        {
            List<string> links = new List<string>();
            string regexImg = @"<(img)\b[^>]*>";
            MatchCollection matchesImg = Regex.Matches(htmlSource, regexImg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImg)
            {
                links.Add(m.Value);
            }
            return links;
        }

        public static List<string> FetchIFrameFromSource(string htmlSource)
        {
            List<string> links = new List<string>();
            string regexImg = @"<(iframe)\b[^>]*>";
            MatchCollection matchesImg = Regex.Matches(htmlSource, regexImg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImg)
            {
                links.Add(m.Value);
            }
            return links;
        }

        public static List<string> FetchTdsromSource(string htmlSource)
        {
            List<string> links = new List<string>();
            string regexImg = @"<(td)\b[^>]*>";
            MatchCollection matchesImg = Regex.Matches(htmlSource, regexImg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImg)
            {
                links.Add(m.Value);
            }
            return links;
        }








        public static IList<BoxContent> BoxContent(int editionid, int col)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var items = (from b in db.boxcontents
                             join newsbx in db.newsletterboxes.Where(x => x.active == true) on b.newsletterboxid equals newsbx.newsletterboxid

                             join h in db.htmlcomponents on b.htmlcomid equals h.htmlcomid into hs
                             from hbox in hs.DefaultIfEmpty()

                             join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                             from backgroundcolor in cs.DefaultIfEmpty()

                             where b.editionid == editionid
                             && b.positioncolumn == col
                             orderby b.positionorder
                             select new BoxContent
                             {
                                 EditionId = b.editionid.Value,
                                 BoxContentId = b.boxcontentid,
                                 BoxTypeId = (Model.BoxTypeOptions)newsbx.boxtypeid,
                                 Column = (b.positioncolumn.HasValue == false ? 1 : b.positioncolumn.Value),
                                 IsMarkDeleted = false,
                                 Title = b.title,
                                 BoxDate = b.date,
                                 PictureName = b.picturename,
                                 PictureAltText = b.picturealttext,
                                 Hyperlink = b.hyperlink,
                                 HyperlinkTarget = b.hyperlinktarget,
                                 ReadMoreLink = b.readmorelink,
                                 MasterBoxContentId = b.masterboxcontentid,
                                 Content = (hbox == null ? String.Empty : hbox.content),
                                 NewsletterBoxID = newsbx.newsletterboxid,
                                 Sort = (b.positionorder.HasValue == false ? 0 : b.positionorder.Value),
                                 Style = new BoxStyle
                                 {
                                     NewsletterBoxID = newsbx.newsletterboxid,
                                     IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                     CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid),
                                     Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                     Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                     Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255)
                                 }
                             }).ToList();

                return items;
            }
        }

        public static IList<BoxContent> BoxContent(int editionid)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var items = (from b in db.boxcontents

                             join newsbx in db.newsletterboxes.Where(x => x.active == true) on b.newsletterboxid equals newsbx.newsletterboxid

                             join h in db.htmlcomponents on b.htmlcomid equals h.htmlcomid into hs
                             from hbox in hs.DefaultIfEmpty()

                             join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                             from backgroundcolor in cs.DefaultIfEmpty()

                             where b.editionid == editionid

                             orderby b.positioncolumn, b.positionorder
                             select new BoxContent
                             {
                                 EditionId = b.editionid.Value,
                                 BoxContentId = b.boxcontentid,
                                 BoxTypeId = (Model.BoxTypeOptions)newsbx.boxtypeid,
                                 Column = (b.positioncolumn.HasValue == false ? 1 : b.positioncolumn.Value),
                                 IsMarkDeleted = false,
                                 Title = b.title,
                                 BoxDate = b.date,
                                 PictureName = b.picturename,
                                 PictureAltText = b.picturealttext,
                                 Hyperlink = b.hyperlink,
                                 HyperlinkTarget = b.hyperlinktarget,
                                 ReadMoreLink = b.readmorelink,
                                 MasterBoxContentId = b.masterboxcontentid,
                                 Content = (hbox == null ? String.Empty : hbox.content),
                                 NewsletterBoxID = newsbx.newsletterboxid,
                                 Sort = (b.positionorder.HasValue == false ? 0 : b.positionorder.Value),
                                 Style = new BoxStyle
                                 {
                                     NewsletterBoxID = newsbx.newsletterboxid,
                                     IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                     CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid),
                                     Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                     Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                     Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255)
                                 }
                             }).ToList();

                return items;
            }

        }

        public static IList<BoxContent> BoxReplicants(int boxContentId)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var items = (from b in db.boxcontents

                             join newsbx in db.newsletterboxes.Where(x => x.active == true) on b.newsletterboxid equals newsbx.newsletterboxid

                             join h in db.htmlcomponents on b.htmlcomid equals h.htmlcomid into hs
                             from hbox in hs.DefaultIfEmpty()

                             join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                             from backgroundcolor in cs.DefaultIfEmpty()

                             where b.masterboxcontentid == boxContentId

                             orderby b.masterboxcontentid
                             select new BoxContent
                             {
                                 EditionId = b.editionid.Value,
                                 BoxContentId = b.boxcontentid,
                                 BoxTypeId = (Model.BoxTypeOptions)newsbx.boxtypeid,
                                 Column = (b.positioncolumn.HasValue == false ? 1 : b.positioncolumn.Value),
                                 IsMarkDeleted = false,
                                 Title = b.title,
                                 BoxDate = b.date,
                                 PictureName = b.picturename,
                                 PictureAltText = b.picturealttext,
                                 Hyperlink = b.hyperlink,
                                 HyperlinkTarget = b.hyperlinktarget,
                                 ReadMoreLink = b.readmorelink,
                                 MasterBoxContentId = b.masterboxcontentid,
                                 Content = (hbox == null ? String.Empty : hbox.content),
                                 NewsletterBoxID = newsbx.newsletterboxid,
                                 Sort = (b.positionorder.HasValue == false ? 0 : b.positionorder.Value),
                                 Style = new BoxStyle
                                 {
                                     NewsletterBoxID = newsbx.newsletterboxid,
                                     IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                     CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid),
                                     Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                     Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                     Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255)
                                 }
                             }).ToList();

                return items;
            }

        }



        public static IList<BoxStyle> BoxStylesByNewsletterTemplate(int newsletterTemplateId)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var items = (from newsbx in db.newsletterboxes.Where(x => x.active == true)
                             join ntb in db.NewsletterTemplatesBoxes on newsbx.newsletterboxid equals ntb.newsletterboxid
                             join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                             from backgroundcolor in cs.DefaultIfEmpty()
                             where ntb.newslettertemplateid == newsletterTemplateId
                             select new BoxStyle
                             {
                                 NewsletterBoxID = newsbx.newsletterboxid,
                                 IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                 Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                 Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                 Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255),
                                 CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid)
                             }).ToList();

                return items;
            }

        }

        public static BoxStyle BoxStyleByBox(int newsletterBoxId)
        {
            using (ENewslettersEntities db = TemplateDb.eNewslettersEntities)
            {
                var item = (from newsbx in db.newsletterboxes.Where(x => x.active == true)
                            join ntb in db.NewsletterTemplatesBoxes on newsbx.newsletterboxid equals ntb.newsletterboxid
                            //join bxtype in db.boxtypes on newsbx.boxtypeid equals bxtype.boxtypeid
                            join c in db.colors on newsbx.bgcolorid equals c.colorid into cs
                            from backgroundcolor in cs.DefaultIfEmpty()
                            where newsbx.newsletterboxid == newsletterBoxId
                            select new BoxStyle
                            {
                                NewsletterBoxID = newsbx.newsletterboxid,
                                IsTransparent = (newsbx.transparent == null ? false : (newsbx.transparent == 1 ? true : false)),
                                Red = (backgroundcolor != null ? backgroundcolor.red.Value : 255),
                                Green = (backgroundcolor != null ? backgroundcolor.green.Value : 255),
                                Blue = (backgroundcolor != null ? backgroundcolor.blue.Value : 255),
                                CornerTypeOption = (newsbx.cornertypeid == null ? BoxStyle.CornerTypeOptions.None : (BoxStyle.CornerTypeOptions)newsbx.cornertypeid)
                            }).FirstOrDefault();

                return item;
            }

        }


        /// <summary>
        /// This wraps the parent an child boxes into one box
        /// </summary>
        /// <param name="boxContentId"></param>
        /// <param name="bxStyle"></param>
        /// <param name="cornerUrlFormat"></param>
        /// <param name="bxContent"></param>
        /// <returns></returns>
        public static string FormatBox(int boxContentId,
            BoxStyle bxStyle,
            string bxContent, bool isEmail = false)
        {
            string strDD = "\n<div align=\"center\" id=\"Item_" + boxContentId.ToString() + "\" style=\"background-color: transparent;width:100%;\">\n";

            //if (bxStyle.CornerTypeOption != BoxStyle.CornerTypeOptions.None && !bxStyle.IsTransparent)
            //    strDD += BoxWithCorners(bxStyle, bxContent);
            //else
            //{
                if (isEmail) // Use a table
                    strDD += "<table style=\"background-color: " + bxStyle.BackgroundColorForStyle + "; width:100%;\"><tr><td style=\"padding:6px;\">" + bxContent + "</td></tr></table>";
                else
                    strDD += "<div style=\"background-color: " + bxStyle.BackgroundColorForStyle + "; width:100%;\"><div style=\"padding:6px;\">" + bxContent + "</div></div>";
            //}


            strDD += "</div>\n";



            return strDD;
        }


        private static string BoxWithCorners(
            BoxStyle bxStyle,
            string bxContent)
        {

            string backgroundColor = bxStyle.BackgroundColorForStyle;

            string roundedCornerCss = "-moz-border-radius:6px;-webkit-border-radius:6px; overflow: hidden;";
            if (bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.All || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Left || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Top || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.TopLeft)
                roundedCornerCss += "border-top-left-radius: 6px;";

            if (bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.All || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Right || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Top || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.TopRight)
                roundedCornerCss += "border-top-right-radius: 6px;";

            if (bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.All || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Bottom || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.BottomLeft || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Left)
                roundedCornerCss += "border-bottom-left-radius: 6px;";

            if (bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.All || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Bottom || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.BottomRight || bxStyle.CornerTypeOption == BoxStyle.CornerTypeOptions.Right)
                roundedCornerCss += "border-bottom-right-radius: 6px;";


            string templateWithCorners =
                "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse: collapse; background-color: transparent; font-size: 100%;width: 100%;table-layout: fixed; " + roundedCornerCss + " \">\n";

            templateWithCorners += "<tr style=\"font-size:0;\">"
                    + "<td width=\"6\" style=\"width:6px;height:6px;font-size:0;line-height:6px;background-color:{0};\"></td>"
                    + "<td height=\"6\" style=\"line-height:6px;background-color:{0};\"></td>"
                    + "<td width=\"6\" style=\"width:6px;height:6px;font-size:0;line-height:6px;background-color:{0};\"></td>"
                    + "</tr>";

            templateWithCorners += "<tr><td width=\"6\" style=\"width:6px; background-color:{0};\"></td>";
            templateWithCorners += "<td style=\"background-color:{0};\">{1}</td>";
            templateWithCorners += "<td width=\"6\" style=\"width: 6px; background-color:{0};\"></td></tr>";

            templateWithCorners += "<tr style=\"font-size:0;\">"
                + "<td width=\"6\" height=\"6\" style=\"width:6px;height:6px;font-size:6px;line-height:6px;background-color:{0};\"></td>"
                + "<td height=\"6\" style=\"height:6px;font-size:6px;background-color:{0};\"></td>"
                + "<td width=\"6\" height=\"6\" style=\"width:6px;height:6px;font-size:6px;line-height:6px;background-color:{0};\"></td>"
                + "</tr>";
            templateWithCorners += "</table>";

            string box = string.Format(templateWithCorners, backgroundColor, bxContent);

            return box;
        }








    }



}

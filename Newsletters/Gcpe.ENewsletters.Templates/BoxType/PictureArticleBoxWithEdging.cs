﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.BoxType
{
    using Model;

    public static class PictureArticleBoxWithEdging
    {
        public static string Description = "Picture and Article Box with Edging";

        private const string ImageTag = "<img src=\"[picture]\" alt=\"[picturealttext]\" width=\"[width]\" height=\"[height]\" />";

        private const string EmailTemplate =
                "<table style=\"background-color:[boxstyle]; width: 100%; margin: 0px;\" class=\"print-box\">" +
                "   <tr>" +
                "       <td style=\"text-align:center;\">" +
                "           <table>" +
                "               <tr>" +
                "                   <td>" + ImageTag + "</td>" +
                "               </tr>" +
                "               <tr>" +
                "                   <td style=\"text-align:left;width:100%;font-family: Verdana, Arial, Helvetica, sans-serif; color:#333333;font-size: 11px;\">[content]</td>" +
                "               </tr>" +
                "           </table>" +
                "       </td>" +
                "   </tr>" +
                "</table>";

        private const string HtmlTemplate =
                "<table style=\"background-color:[boxstyle]; width: 100%; margin: 0px;\" class=\"print-box\">" +
                "   <tr>" +
                "       <td style=\"text-align:center;\">" +
                "           <table>" +
                "               <tr>" +
                "                   <td>" + ImageTag + "</td>" +
                "               </tr>" +
                "               <tr>" +
                "                   <td style=\"text-align:left;width:100%;\">[content]</td>" +
                "               </tr>" +
                "           </table>" +
                "       </td>" +
                "   </tr>" +
                "</table>";

        private static string TextTemplate =
            "[picturealttext] >> [picture]\r\n" +
            "[content]\r\n" +
            "Read more>> [readmore]\r\n";

        public static void ExampleContent(BoxContent bx, string ImageFolderLocation)
        {
            bx.BoxTypeId = Gcpe.ENewsletters.Templates.Model.BoxTypeOptions.Picture_And_Article_Box_With_Edging;
            bx.Title = "";
            bx.PictureName = ImageFolderLocation + "ImageSizeStandards.jpg";
            bx.PictureAltText = "This is an Article Box Picture with Edging.";
            bx.BoxDate = DateTime.Today;
            bx.Content = "Velenim diat velese jjjid endre feuguer sim exe erat nulput augiam ipit con henim dio nse mag prat.";
        }

        public static string FillTextTemplate(BoxContent bxContent, string getFileLocation, string imagePathUrl, string baseUrl)
        {
            string bxText = TextTemplate;

            if (bxContent.PictureName == null ||
                bxContent.PictureName.ToString().Length == 0)
            {
                bxText = bxText.Replace("[picturealttext] >> [picture]\r\n", "");
            }
            else
            {
                string imageSrc = string.Empty;

                if (bxContent.PictureName.ToLower().Contains("imagesizestandards.jpg"))
                    imageSrc = imagePathUrl + "imagesizestandards.jpg";

                else if (bxContent.PictureName.Contains("ministerboxsizestandards.jpg"))
                    imageSrc = imagePathUrl + "ministerboxsizestandards.jpg";
                else
                    imageSrc = Utility.MakePictureSrc(getFileLocation, bxContent.PictureName.ToString());

                bxText = bxText.Replace("[picture]", imageSrc);
                bxText = bxText.Replace("[picturealttext]", bxContent.PictureAltText);
            }

            bxText = bxText.Replace("[title]", bxContent.Title);
            bxText = bxText.Replace("[hyperlink]", bxContent.Hyperlink);
            bxText = bxText.Replace("[hyperlinktarget]", bxContent.HyperlinkTarget);
            bxText = bxText.Replace("[date]", string.Format("{0:MMMM d, yyyy}", bxContent.BoxDate));


            if (bxContent.Content.ToString() == "")
                bxText = bxText.Replace("[content]", "Content area." + bxContent.BoxContentId.ToString());

            else
                bxText = bxText.Replace("[content]", bxContent.Content);



            if (bxContent.ReadMoreLink == null
                || bxContent.ReadMoreLink.Trim() == string.Empty)
            {
                bxText = bxText.Replace("Read more>> [readmore]", "");
            }
            else
            {
                string strTemp = getFileLocation + bxContent.ReadMoreLink.ToString().Trim();
                strTemp = Url.VirtualPathEncode1_PublicSite(strTemp, baseUrl);
                bxText = bxText.Replace("[readmore]", strTemp);
            }


            return bxText;
        }



        public static string FillHtmlTemplate(BoxContent bxContent, string GetFileLocation, string ImagePathUrl, bool isEmail)
        {
            string html = (isEmail == true ? EmailTemplate : HtmlTemplate);
            html = html.Replace("[boxstyle]", bxContent.Style.BackgroundColorForStyle);

            html = Box.FormatImageInfoInBox(bxContent, html, GetFileLocation, ImagePathUrl);


            html = Box.FormatContent(bxContent, html, GetFileLocation, ImagePathUrl, isEmail);
            html = Box.FixHeadingColor(html);

            return html;
        }
    }

}

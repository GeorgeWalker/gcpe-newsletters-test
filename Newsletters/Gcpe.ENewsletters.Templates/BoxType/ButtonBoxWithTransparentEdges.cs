using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.BoxType
{
    using Model;

    public static class ButtonBoxWithTransparentEdges
    {
        public static string Description = "Button Box with Transparent Edges";

        private const string ImageTag = "<img src=\"[picture]\" style=\"border: 1;\" alt=\"[picturealttext]\" width=\"[width]\" height=\"[height]\" />";


        private static string HtmlTemplate = "<table style=\"background-color: transparent; width:100%; text-align:center;table-layout: fixed;\"  class=\"print-box\">" +
            "<tr>" +
            "<td><a href=\"[hyperlink]\" target=\"[hyperlinktarget]\" style=\"border: 1;\">" + ImageTag + "</a></td>" +
            "</tr>" +
            "</table>";

        private static string TextTemplate =
            "[picturealttext] >> [picture]\r\n" +
            "Read more >> [hyperlink]\r\n";


        public static void ExampleContent(BoxContent bx, string ImageFolderLocation)
        {
            bx.BoxTypeId = Gcpe.ENewsletters.Templates.Model.BoxTypeOptions.Button_Box_With_Transparent_Edges;
            bx.Title = "";
            bx.PictureName = ImageFolderLocation + "ImageSizeStandards.jpg";
            bx.PictureAltText = "This is a button box with Transparent Edges.";
            bx.BoxDate = DateTime.Today;
            bx.HyperlinkTarget = "_self";
            bx.Content = "";
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

            bxText = bxText.Replace("[hyperlink]", bxContent.Hyperlink);


            return bxText;
        }


        public static string FillHtmlTemplate(BoxContent bxContent, string GetFileLocation, string ImagePathUrl)
        {
            string html = HtmlTemplate;
            html = Box.FormatImageInfoInBox(bxContent, html, GetFileLocation, ImagePathUrl);


            html = html.Replace("[hyperlink]", bxContent.Hyperlink);
            html = html.Replace("[hyperlinktarget]", bxContent.HyperlinkTarget);

            return html;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.BoxType
{

    using Model;

    public static class ButtonBox
    {

        public static string Description = "Button Box";

        private const string ImageTag = "<img src=\"[picture]\" style=\"border: none;\" alt=\"[picturealttext]\" width=\"[width]\" height=\"[height]\"  />";

        // Need to add style to TD for emails
        private static string HtmlTemplate = 
            "<table style=\"width:100%; text-align:center; table-layout: fixed;overflow:hidden;\" class=\"print-box\">" +
                       "<tr>" +
                       "<td>" +
                       "<a href=\"[hyperlink]\" target=\"[hyperlinktarget]\" style=\"border: 0;\">" +
                       ImageTag +
                       "</a>" +
                       "</td>" +
                       "</tr>" +
                       "</table>";

        private static string TextTemplate = 
            "[picturealttext] >> [picture]\r\n" +
            "Read more >>[hyperlink]\r\n";

        public static void ExampleContent(BoxContent bx, string ImageFolderLocation)
        {
            bx.BoxTypeId = Gcpe.ENewsletters.Templates.Model.BoxTypeOptions.Button_Box;
            bx.Title = "";
            bx.PictureName = ImageFolderLocation + "ImageSizeStandards.jpg";
            bx.PictureAltText = "This is a button box.";
            bx.HyperlinkTarget = "_self";
            bx.BoxDate = DateTime.Today;
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

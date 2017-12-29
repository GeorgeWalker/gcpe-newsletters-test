using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.BoxType
{

    using Model;

    public static class HeaderBox
    {
        public static string Description = "Header Box";

        private static string EmailTemplate = "<table style=\"width:100%; text-align:left; table-layout: fixed;\" class=\"print-box\">" +
                "   <tr>" +
                "       <td style=\"font-family: Verdana, Arial, Helvetica, sans-serif; color:#333333;font-size: 11px;\">[content]</td>" +
                "   </tr>" +
                "</table>\r\n";

        private static string HtmlTemplate = "<table style=\"width:100%; text-align:left; table-layout: fixed;\" class=\"print-box\">" +
                "   <tr>" +
                "       <td>[content]</td>" +
                "   </tr>" +
                "</table>\r\n";

        private static string TextTemplate = 
            "[content]\r\n" +
            "Read more>> [readmore]\r\n";

        public static void ExampleContent(BoxContent bx)
        {
            bx.BoxTypeId = Gcpe.ENewsletters.Templates.Model.BoxTypeOptions.Header_Box;
            bx.Title = "This is a header box";
            bx.PictureName = "";
            bx.PictureAltText = "";
            bx.BoxDate = DateTime.Today;
            bx.Content = "This is a header box";
        }

        public static string FillTextTemplate(BoxContent bxContent, string getFileLocation, string imagePathUrl, string baseUrl)
        {
            string bxText = TextTemplate;

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


        public static string FillHtmlTemplate(BoxContent bxContent, string GetFileLocation, string imagePathUrl, bool isEmail)
        {
            string html = (isEmail == true ? EmailTemplate : HtmlTemplate);
            html = Box.FormatContent(bxContent, html, GetFileLocation, imagePathUrl, isEmail);
            html = Box.FixHeadingColor(html);
            return html;
        }


    }


}

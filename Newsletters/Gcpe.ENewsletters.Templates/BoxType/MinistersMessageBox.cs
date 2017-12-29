using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.BoxType
{
    using Model;

    public static class MinistersMessageBox
    {
        public static string Description = "Minister's Message Box";

        private const string ImageTag = "<img src=\"[picture]\" alt=\"[picturealttext]\" title=\"[picturealttext]\" width=\"[width]\" height=\"[height]\" align=\"left\" style=\"margin-bottom: 10px; margin-right: 10px;\" />";
        private const string ImageTagEmail = "<img src=\"[picture]\" alt=\"[picturealttext]\" title=\"[picturealttext]\" width=\"[width]\" height=\"[height]\" align=\"left\" style=\"margin-bottom: 10px; margin: 5px;\" vspace=\"5\" hspace=\"5\" />"; // We leave in the style for the online preview.

        private static string EmailTemplate =
            "<table style=\"background-color:[boxstyle];width: 100%; margin: 0px; text-align:left;table-layout: fixed;\" class=\"print-box\">" +
              "<tr>" +
              " <td>" +
              "     <table style=\"background-color: transparent; width: 100%; height: 100%;table-layout: fixed;\">" +
              "         <tr>" +
              "             <td style=\"padding: 0px;font-size: 11px; text-align:left; \">" +
              "                 <h5>[title]</h5>" +
              "             </td>" +
              "         </tr>" +
              "         <tr>" +
              "             <td style=\"padding: 0px;font-family: Verdana, Arial, Helvetica, sans-serif; color:#333333;font-size: 11px;\">" +
              ImageTagEmail +
              "                 [content]" +
              "             </td>" +
              "         </tr>" +
              "     </table>" +
              " </td>" +
              "</tr>" +
              "</table>\r\n";

        
        private static string HtmlTemplate =
            "<div style=\"background-color:[boxstyle]; font-size: 100%;width: 100%;text-align:left;margin: 0;vertical-align:top;\" class=\"print-box\">" +
            "   <h5>[title]</h5>" +
            "   <div>" +
                    ImageTag + "[content]" +
            "   </div>" +
            "</div>" +
        "";

        private static string TextTemplate = "" +
            "[title]\r\n" +
            "[picturealttext] >> [picture]\r\n" +
            "[content]\r\n" +
            "Read more>> [readmore]\r\n";


        public static void ExampleContent(BoxContent bx, string ImageFolderLocation)
        {
            bx.BoxTypeId = Gcpe.ENewsletters.Templates.Model.BoxTypeOptions.Ministers_Message_Box;
            bx.Title = "Minister's Message Box";
            bx.PictureName = ImageFolderLocation + "MinisterBoxSizeStandards.jpg";
            bx.PictureAltText = "Photo of the Minister.";
            bx.BoxDate = DateTime.Today;
            bx.Content = "Delenisi erit liquip gguf y su stin verv ipis nons jh etum atet quisl dolor at wisent aci erostinit velenim diat velese jjjid endre feuguer sim exe erat nulput augiam ipit, con henim dionse mag nibh et prat.  Inibh esto odolesed tat praesti his autatinim velenis modo loreros augiam zzr jhbit irit, sustrud magnisl ipit lorer se magna faci hdl iquis ero enim ip et, df conulput am quis jdncf eu feuis acilluptat.  Utef praestrud exeros nonx ulla faciduismodo co dg nsecte tet se ming ex ercipisi.  Acipit sisi blaa feuiscidunt azr illiquat.<br><br>"
                                + "Acipit, sisi bla feuisc sd idunt am zzriliquat.<br><br>"
                                + "Until next month,<br><br>"
                                + "<b>Minister's Name</b><br>"
                                + "Name of Ministry<br><br>"
                                + "<a href=\"\" target=\"_blank\">link to Minister’s site</a>";
        }

        public static string FillTextTemplate(BoxContent bxContent, string getFileLocation, string imagePathUrl, string baseUrl)
        {
            string bxText = TextTemplate;

            bxText = bxText.Replace("[title]", bxContent.Title);

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



            if (bxContent.Content.ToString() == "")
                bxText = bxText.Replace("[content]", "Content area." + bxContent.BoxContentId.ToString());

            else
                bxText = bxText.Replace("[content]", bxContent.Content);
            
            //check if content has a table


            if (bxContent.ReadMoreLink == null
                || bxContent.ReadMoreLink.Trim() == string.Empty)
            {
                bxText = bxText.Replace("Read more>> [readmore]\r\n", "");
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
            html = html.Replace("[title]", bxContent.Title); 
            html = Box.FormatContent(bxContent, html, GetFileLocation, ImagePathUrl, isEmail);
            html = Box.FixHeadingColor(html);
            return html;
        }

    }

}

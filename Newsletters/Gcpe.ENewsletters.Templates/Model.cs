using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates.Model
{
    using Gcpe.ENewsletters.Data.Entity;

    public enum BoxTypeOptions
    {
        Picture_And_Article_Box = 1,
        Picture_And_Article_Box_With_Edging = 2,
        Picture_With_Title_And_Date_Box = 3,
        Button_Box = 4,
        Button_Box_With_Transparent_Edges = 5,
        Ministers_Message_Box = 6,
        Header_Box = 7
    }
    public class BoxTypeOption
    {
        public BoxTypeOptions ID;
        public string Name;
    }




    public enum NewsletterTemplateTypes {
        One_Column_TemplateId = 1,
        Two_Column_Equal_TemplateId = 2,
        Two_Column_LeftBigger_TemplateId = 3,
        Two_Column_RightBigger_TemplateId = 4
    }

    public enum ViewTarget
    {
        Browser,
        Email
    }

    public enum EmailFormat
    {
        Html,
        Text
    }

    public enum PathType
    {
        Edition = 0,
        Article = 1,
        Image = 2,
        File = 3,
        Corner = 4
    }

    public class Image
    {
        public string FileType;
        public string FileName;
        public byte[] File;
        public DateTime CreatedDate;
    }

    public class LinkArea
    {
        public string Coordinates;
        public string LinkUrl;
    }

    public class FooterLink
    {
        public string DisplayText;
        public string LinkUrl;
        public int Order;
        public string LinkText;
    }



    public class BoxContent
    {
        public int BoxContentId;
        public int? MasterBoxContentId;
        public int EditionId;
        public int NewsletterBoxID;

        public string Title;
        public DateTime? BoxDate;
        public string PictureName;
        public string PictureAltText;
        public string Hyperlink;
        public string HyperlinkTarget;
        public string ReadMoreLink;
        public string Content;

        public BoxTypeOptions BoxTypeId;
        public string BoxTypeName
        {
            get
            {
                switch (BoxTypeId)
                {
                    case BoxTypeOptions.Button_Box:
                        return BoxType.ButtonBox.Description;
                    case BoxTypeOptions.Button_Box_With_Transparent_Edges:
                        return BoxType.ButtonBoxWithTransparentEdges.Description;
                    case BoxTypeOptions.Header_Box:
                        return BoxType.HeaderBox.Description;
                    case BoxTypeOptions.Ministers_Message_Box:
                        return BoxType.MinistersMessageBox.Description;
                    case BoxTypeOptions.Picture_And_Article_Box:
                        return BoxType.PictureAndArticleBox.Description;
                    case BoxTypeOptions.Picture_And_Article_Box_With_Edging:
                        return BoxType.PictureArticleBoxWithEdging.Description;
                    case BoxTypeOptions.Picture_With_Title_And_Date_Box:
                        return BoxType.PictureWithTitleAndDateBox.Description;
                    default:
                        return "Box Type Not Found";
                }
            }
        }

        public bool IsMarkDeleted = false; 
        public int Sort;
        public int Column = 1;

        public BoxStyle Style;
    }


    public class BoxStyle
    {
        public enum BoxCornerType
        {
            None = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4
        }

        public enum CornerTypeOptions
        {
            All = 1,
            Bottom = 2,
            BottomLeft = 3,
            BottomRight = 4,
            Left = 5,
            None = 6,
            Right = 7,
            Top = 8,
            TopLeft = 9,
            TopRight = 10
        }

        public int NewsletterBoxID;      
        public bool IsTransparent;
        public CornerTypeOptions CornerTypeOption;

        public int Red;
        public int Blue;
        public int Green;        

        public string BackgroundColorForStyle
        {
            get {
                string strBGColor = "#ffffff";

                if (IsTransparent)
                    strBGColor = "transparent";
                else
                    strBGColor = "#" + Gcpe.ENewsletters.Templates.Utility.GetHexFromRGB(Red, Green, Blue);

                return strBGColor;
            }
        }


    }








}

using System;
using System.Linq;
using System.Web;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;
    using Model;

    public class Url
    {

        public static string TEXTURL_ENDCHARACTER = ">";

        /// <summary>
        /// Encodes the URL to an Edition for a Newsletter
        /// </summary>
        /// <param name="newsletterId"></param>
        /// <returns>An encoded URL, ie. LNG_in_BC/ </returns>
        public static string GetNewsletterEncodedUrl(int newsletterId)
        {
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int? folderId = (from n in db.newsletters
                                 where n.newsletterid == newsletterId
                                 select n.folderid).FirstOrDefault();

                folderId = (folderId.HasValue ? folderId.Value : -1);
                string strPath = "";
                strPath = Url.GetEncodedUrl(folderId.Value) + strPath;
                strPath = Url.ReplaceChars(strPath);
                return strPath;
            }
        }

        /// <summary>
        /// The encoded URL for an edition
        /// </summary>
        /// <param name="editionId"></param>
        /// <param name="isPublicVersion"></param>
        /// <returns>An encoded URL, ie. LNG_in_BC/February_2013/edition instead of getfile.aspx?editionid=123</returns>
        public static string GetEditionEncodedUrl(string editionId, bool isPublicVersion)
        {
            int eid = Int32.Parse(editionId);

            string strPath = "newsletters/";
            if (!isPublicVersion)
                strPath = "Internal/edition";

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                string guid = (from e in db.editions
                               where e.editionid == eid
                               select e.key).FirstOrDefault().ToString().ToLower();

                int newsletterID = (from e in db.editions
                                    where e.editionid == eid
                                    select e.newsletterid).FirstOrDefault().Value;

                string newsletterKey = (from e in db.newsletters
                                        where e.newsletterid == newsletterID
                                        select e.key).FirstOrDefault();

                strPath = strPath + newsletterKey + "/" + guid;
            }

            return strPath;
        }

        /// <summary>
        /// Encodes URL for a file or image
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns>An encoded URL, ie. LNG_in_BC/Image/guid instead of getfile.aspx?guid=123</returns>
        public static string GetFileEncodedUrl(PathType type, string id)
        {
            string strTmp = string.Empty;
            Guid g = new Guid(id);

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int folderid = -1;
                int? fromDb;
                if (id.Contains("."))
                    fromDb = (from f in db.files.Where(x => x.fname.Contains(id)) select f.folderid).FirstOrDefault();
                else
                    fromDb = (from f in db.files where f.guid == g select f.folderid).FirstOrDefault();

                if (fromDb.HasValue)
                    folderid = fromDb.Value;

                strTmp = Url.GetEncodedUrl(folderid) + type.ToString() + "/" + id;
                strTmp = Url.ReplaceChars(strTmp);
            }

            return strTmp;
        }

        public static string GetArticleEncodedUrl(string articleId, bool isPublicVersion)
        {

            int aId;
            if (int.TryParse(articleId, out aId) == false)
            {
                string strTmp = "";
                char[] chars = articleId.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] >= '0' && chars[i] <= '9')
                        strTmp += chars[i];
                    else
                        break;
                }

            }
            string strPath = "article";

            if (!isPublicVersion)
                strPath = "Internal/article";


            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int folderId = -1;
                string artKey = string.Empty;

                var art = (from a in db.articles
                           join e in db.editions on a.editionid equals e.editionid
                           where a.articleid == aId
                           select new
                           {
                               FolderId = e.folderid,
                               ArticleKey = a.key
                           }).FirstOrDefault();

                if (art.FolderId.HasValue)
                    folderId = art.FolderId.Value;

                if (art.ArticleKey != null)
                    artKey = art.ArticleKey;

                strPath = Url.GetEncodedUrlKeys(folderId) + artKey.Trim();
            }
            return strPath;
        }

        /// <summary>
        /// This makes up the path
        /// </summary>
        /// <param name="folderid"></param>
        /// <returns></returns>
        public static string GetEncodedUrl(int folderid)
        {
            string strTmp, strPath = "";
            int id = folderid;

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                while (id >= 0)
                {
                    folder fs = (from f in db.folders
                                 where f.folderid == id
                                 select f).FirstOrDefault();

                    if (fs != null)
                    {
                        id = (fs.parentfolderid.HasValue ? fs.parentfolderid.Value : -1); // Gets the parent folder id (edition, newsletter, usergroup)

                        //If there is no parent, we are on the usergroup so don't include it
                        if (id != -1)
                        {
                            strTmp = (fs.description == null ? string.Empty : fs.description);
                            strTmp = strTmp.Trim().Replace(' ', '_');
                            strTmp = strTmp.ToLower();
                            strTmp = HttpUtility.UrlEncode(strTmp);
                            if (strTmp.Trim().Length > 0)
                                strPath = strTmp.Trim() + "/" + strPath;
                        }
                    }
                    else
                        id = -1;
                }
            }


            return strPath;
        }

        public static string GetEncodedUrlKeys(int folderid)
        {
            string strPath = string.Empty;


            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                strPath = (from f in db.folders
                           join N in db.newsletters on f.parentfolderid equals N.folderid
                           join E in db.editions on f.folderid equals E.folderid
                           where f.folderid == folderid
                           select N.key + "/" + E.key + "/"
                             ).FirstOrDefault();
            }
            return strPath;
        }


        private static PathType FileTypeByGuid(string guid)
        {
            PathType pt = PathType.Image;

            if (guid != string.Empty)
                return pt;

            try
            {
                Guid g = new Guid(guid);

                using (ENewslettersEntities db = new ENewslettersEntities())
                {
                    int it = -1;
                    int? fromDb;
                    if (guid.Contains("."))
                        fromDb = (from f in db.files.Where(x => x.fname.Contains(guid)) select f.type).FirstOrDefault();
                    else
                        fromDb = (from f in db.files where f.guid == g select f.type).FirstOrDefault();

                    if (fromDb.HasValue)
                        it = fromDb.Value;

                    pt = it == 1 ? PathType.Image : PathType.File;
                }
            }
            catch
            {
                return pt;
            }


            return pt;
        }


        public static Model.Image GetFileByGuid(string guid)
        {
            Model.Image imgInfo = new Model.Image();
            try
            {
                Guid g = new Guid(guid);
                using (ENewslettersEntities db = new ENewslettersEntities())
                {
                    file imgFile = (from f in db.files where f.guid == g select f).FirstOrDefault();

                    if (imgFile != null)
                    {
                        imgInfo.File = imgFile.image;
                        imgInfo.FileName = imgFile.fname;
                        imgInfo.FileType = imgFile.ftype;
                        imgInfo.CreatedDate = imgFile.createdate ?? DateTime.MinValue;
                    }
                }
            }
            catch
            {
                //Nothing, just continue through and return an empty image class.
            }

            return imgInfo;
        }
        public static Model.Image GetFileByName(string name)
        {
            Model.Image imgInfo = new Model.Image();

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                file imgFile = (from f in db.files.Where(x => x.fname.Contains(name)) select f).FirstOrDefault();

                if (imgFile != null)
                {
                    imgInfo.File = imgFile.image;
                    imgInfo.FileName = imgFile.fname;
                    imgInfo.FileType = imgFile.ftype;
                }
            }

            return imgInfo;
        }








        /// <summary>
        /// Looks at the Virtual Path and looks up all the edition name, newsletter name (and sometimes org name)
        /// An Edition name is unique per newsletter
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetEditionIdFromEncodedUrl(string path)
        {

            int id = -1;

            // Old URLs used to have the ministry/organization/group file name in them, where as now they only use the 
            // e-newsletter and edition filenames

            //split array into 2 or 3
            string[] pathComponents = path.Split(new Char[] { '/' });

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int eId;
                string editionFolderName = string.Empty;
                string newsletterFolderName = string.Empty;
                string orgFolderName = string.Empty;

                if (pathComponents.Length == 5)
                {
                    editionFolderName = pathComponents[3];
                    newsletterFolderName = pathComponents[2];
                    orgFolderName = pathComponents[1];

                    // Has the Group File Name in the URL
                    eId = (from e in db.editions.Where(x => x.active != 99)
                           join editionfolder in db.folders on e.folderid equals editionfolder.folderid
                           join newsfolder in db.folders on editionfolder.parentfolderid equals newsfolder.folderid
                           join usergroupfolder in db.folders on newsfolder.parentfolderid equals usergroupfolder.folderid
                           where editionfolder.description == editionFolderName
                               && newsfolder.description == newsletterFolderName
                               && usergroupfolder.description == orgFolderName
                           select e.editionid).FirstOrDefault();
                }
                else
                {
                    editionFolderName = pathComponents[2];
                    newsletterFolderName = pathComponents[1];

                    // Does not have the Group File Name in the URL
                    eId = (from e in db.editions.Where(x => x.active != 99)
                           join editionfolder in db.folders on e.folderid equals editionfolder.folderid
                           join newsfolder in db.folders on editionfolder.parentfolderid equals newsfolder.folderid
                           where editionfolder.description == editionFolderName
                               && newsfolder.description == newsletterFolderName
                           select e.editionid).FirstOrDefault();

                }

                id = eId;

            }

            return id;

        }

        public static string GetNewsUrlFromNewslettersUrl(string path)
        {
            if (path.Contains("/edition"))
            {
                var id = GetEditionIdFromEncodedUrl(path);
                using (var db = new ENewslettersEntities())
                {
                    var edition = (from e in db.editions
                                   join n in db.newsletters on e.newsletterid equals n.newsletterid
                                   where e.editionid == id
                                   select new { n, e }).SingleOrDefault();

                    if (edition != null && edition.e != null && edition.n != null)
                        return "/newsletters/" + edition.n.key + "/" + edition.e.key;
                }
            }
            return null;
        }

        /// <summary>
        /// Looks at the Virtual Path and looks up all the article name, edition name, newsletter name and org name
        /// An Article name is unique per edition, newsletter
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetArticleIdFromEncodedUrl(string path)
        {

            int id = -1;


            // Old URLs used to have the ministry/organization/group file name in them, where as now they only use the 
            // e-newsletter and edition filenames

            //split array into 4
            string[] pathComponents = path.Split(new Char[] { '/' });

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int aId;
                string editionFolderName = string.Empty;
                string newsletterFolderName = string.Empty;
                string orgFolderName = string.Empty;
                string articleFolderName = string.Empty;

                if (pathComponents.Length == 6)
                {
                    editionFolderName = pathComponents[3];
                    newsletterFolderName = pathComponents[2];
                    orgFolderName = pathComponents[1];
                    articleFolderName = pathComponents[4];

                    // Has the Group File Name in the URL
                    aId = (from a in db.articles.Where(x => x.active != 99)
                           join e in db.editions.Where(x => x.active != 99) on a.editionid equals e.editionid
                           join editionfolder in db.folders on e.folderid equals editionfolder.folderid
                           join newsfolder in db.folders on editionfolder.parentfolderid equals newsfolder.folderid
                           join usergroupfolder in db.folders on newsfolder.parentfolderid equals usergroupfolder.folderid
                           where editionfolder.description == editionFolderName
                           && newsfolder.description == newsletterFolderName
                           && usergroupfolder.description == orgFolderName
                           && a.foldername == articleFolderName
                           select a.articleid).FirstOrDefault();
                }
                else
                {
                    editionFolderName = pathComponents[2];
                    newsletterFolderName = pathComponents[1];
                    articleFolderName = pathComponents[3];

                    // Does not have the Group File Name in the URL
                    aId = (from a in db.articles //.Where(x => x.active != 99)
                           join e in db.editions.Where(x => x.active != 99) on a.editionid equals e.editionid
                           join editionfolder in db.folders on e.folderid equals editionfolder.folderid
                           join newsfolder in db.folders on editionfolder.parentfolderid equals newsfolder.folderid
                           where editionfolder.description == editionFolderName
                           && newsfolder.description == newsletterFolderName
                           && a.foldername == articleFolderName
                           select a.articleid).FirstOrDefault();
                }

                id = aId;
            }


            return id;

        }
        /// <summary>
        /// get article id from unique keys - newsletterkey, editionkey, articlekey
        /// </summary>
        /// <param name="articlePath"></param>
        /// <returns></returns>

        public static int GetArticleIdFromEncodedPath(string articleKeypath)
        {

            int id = -1;
            string[] pathComponents = articleKeypath.Split(new Char[] { '/' });

            string newsletterKey = pathComponents[0];
            string editionKey = pathComponents[1];
            string articleKey = pathComponents[2];

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                int aId;
                aId = (from art in db.articles.Where(x => x.active != 99)
                       join ed in db.editions.Where(x => x.active != 99) on art.editionid equals ed.editionid
                       join newsletter in db.newsletters on ed.newsletterid equals newsletter.newsletterid
                       where art.key == articleKey
                       && ed.key == editionKey
                       && newsletter.key == newsletterKey
                       select art.articleid).FirstOrDefault();
                id = aId;
            }
            return id;
        }

        public static int GetNewsletterIdFromKey(string key)
        {
            int newsletterid;

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                newsletterid = (from newsletter in db.newsletters
                                where newsletter.key == key && newsletter.nlStatus != (int)NewsletterStatus.Deleted
                                select newsletter.newsletterid).SingleOrDefault();
            }

            return newsletterid;
        }

        public static int GetEditionIdFromKey(string newsletterKey, string editionKey)
        {
            int editionId;

            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                editionId = (from edition in db.editions
                             join newsletter in db.newsletters on edition.newsletterid equals newsletter.newsletterid
                             where newsletter.key == newsletterKey && edition.key == editionKey && edition.active != (int)EditionStatus.Deleted
                             select edition.editionid).SingleOrDefault();
            }

            return editionId;
        }

        public enum NewsletterStatus
        {
            New_Not_Saved = -1,
            Draft = 1,
            Pending = 2,
            Active = 3,
            In_Active = 4,
            Deleted = 99
        }

        public enum EditionStatus
        {
            Draft = 1,
            Appoval_Pending = 2,
            Approved = 3,
            Deleted = 99
        }

        #region "Encode"

        public static string EncodeUrls_PublicSite(string strHtml, string baseUrl)
        {
            string strTmp = HttpUtility.HtmlDecode(strHtml);
            strTmp = strTmp.Replace(baseUrl + "admin/getfile.aspx", "getfile.aspx").Replace(baseUrl + "getfile.aspx", "getfile.aspx");

            //HAVE TO ADD IN EXTRA CODE FOR THE SHARED DATABASE IN DEV/TEST/UAT
            //strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/" + "admin/getfile.aspx", "getfile.aspx").Replace("http://www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
            //strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/" + "admin/getfile.aspx", "getfile.aspx").Replace("http://uat.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
            //strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/" + "admin/getfile.aspx", "getfile.aspx").Replace("http://test.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
            //strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/" + "admin/getfile.aspx", "getfile.aspx").Replace("http://dev.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");


            string[] strS = { "getfile.aspx" };
            string[] strItems = strTmp.Split(strS, StringSplitOptions.None);
            if (strItems.Length < 2)
                return strHtml;

            int i;
            strTmp = strItems[0];
            for (i = 1; i < strItems.Length; i++)
            {
                strItems[i] = "getfile.aspx" + strItems[i];
                strItems[i] = HtmlPathEncode_PublicSite(strItems[i - 1], strItems[i], Template.NEWSLETTERS_FOLDER);
                strTmp += strItems[i];
            }
            return strTmp;
        }


        public static string EncodeUrls_AdminSite(string strHtml, bool isEmail, bool isDistributeVersion, string baseUrlPublicSite, string baseUrlAdminSite)
        {
            int i;
            string strTmp = HttpUtility.HtmlDecode(strHtml);

            if (isEmail) // Only Encode if sending in email
            {
                strTmp = strTmp.Replace(baseUrlAdminSite + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace(baseUrlPublicSite + "getfile.aspx", "getfile.aspx");

                strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://uat.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("https://uat.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://uat.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://test.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://dev.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");

                strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://uat.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("http://uat.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://uat.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://test.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://dev.www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");
                strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/admin/getfile.aspx", "getfile.aspx").Replace("http://www.enewsletters.gov.bc.ca/" + "getfile.aspx", "getfile.aspx");


                string[] strS = { "getfile.aspx" };
                string[] strItems = strTmp.Split(strS, StringSplitOptions.None);
                if (strItems.Length < 2)
                    return strHtml;

                strTmp = strItems[0];
                for (i = 1; i < strItems.Length; i++)
                {
                    strItems[i] = "getfile.aspx" + strItems[i];
                    strItems[i] = EncodeUrls_AdminSite(strItems[i - 1], strItems[i], isEmail, isDistributeVersion, baseUrlPublicSite, baseUrlAdminSite);
                    strTmp += strItems[i];
                }

            }
            else
            {
                strTmp = strTmp.Replace(baseUrlAdminSite + "getfile.aspx", baseUrlAdminSite + "pages/getfile.aspx");

            }




            return strTmp;
        }


        public static string VirtualPathEncode1_PublicSite(string path, string baseUrl)
        {
            path = HttpUtility.HtmlDecode(path);

            string strTmp = path.Replace(baseUrl, "");

            if (!strTmp.ToLower().StartsWith("getfile.aspx"))
                return path;

            string[] strItems = strTmp.Split('?');
            if (strItems.Length < 2)
                return path;

            strTmp = strItems[1];
            strItems = strTmp.Split('&');
            string[] strPara;

            strTmp = "";
            int j;
            for (int i = 0; i < strItems.Length; i++)
            {
                strPara = strItems[i].Split('=');
                if (strPara.Length < 2)
                    continue;
                switch (strPara[0].ToLower())
                {
                    case "guid":
                    case "fname":
                        strTmp = Url.GetEncodeUrlByType(Url.FileTypeByGuid(strPara[1]),
                            strPara[1],
                            baseUrl,
                            true);
                        return strTmp;
                    case "article":
                        strTmp = Url.GetEncodeUrlByType(PathType.Article, strPara[1], baseUrl, true);
                        for (j = 0; j < strItems.Length; j++)
                        {
                            if (j != i)
                                strTmp += "+" + strItems[j].Trim();
                        }

                        return strTmp;
                    case "edition":
                        strTmp = Url.GetEncodeUrlByType(PathType.Edition, strPara[1], baseUrl, true);
                        for (j = 0; j < strItems.Length; j++)
                        {
                            if (j != i)
                                strTmp += "+" + strItems[j].Trim();
                        }

                        return strTmp;
                    default:
                        break;
                }
            }
            return path;
        }
        private static string VirtualPathEncode1_AdminSite(string path, bool emailVersion, bool isDistributeVersion,
            string baseUrlPublicSite,
            string baseUrlAdminSite)
        {
            string baseUrl = string.Empty;
            if (emailVersion)
                baseUrl = baseUrlPublicSite;
            else
                baseUrl = baseUrlAdminSite;

            path = HttpUtility.HtmlDecode(path);

            string strTmp = path.Replace(baseUrlPublicSite, "");
            strTmp = strTmp.Replace(baseUrlAdminSite, "");

            if (!strTmp.ToLower().StartsWith("getfile.aspx"))
                return path;

            string[] strItems = strTmp.Split('?');
            if (strItems.Length < 2)
                return path;

            strTmp = strItems[1];
            strItems = strTmp.Split('&');
            string[] strPara;

            strTmp = "";
            int j;
            for (int i = 0; i < strItems.Length; i++)
            {
                strPara = strItems[i].Split('=');
                if (strPara.Length < 2)
                    continue;
                switch (strPara[0].ToLower())
                {
                    case "guid":
                    case "fname":
                        strTmp = Url.GetEncodeUrlByType(Url.FileTypeByGuid(strPara[1]), strPara[1], baseUrl, false);
                        for (j = 0; j < strItems.Length; j++)
                        {
                            if (j != i)
                                strTmp += "+" + strItems[j];
                        }
                        return strTmp;
                    case "article":

                        if (emailVersion && !isDistributeVersion)
                        {
                            //articles are not showing up right in here since we are not encoding them...
                            strTmp = path.Replace("getfile.aspx?article=", baseUrlAdminSite + "pages/getfile.aspx?articleid=");
                            return strTmp;
                        }

                        strTmp = Url.GetEncodeUrlByType(PathType.Article, strPara[1], baseUrl, false);
                        for (j = 0; j < strItems.Length; j++)
                        {
                            if (j != i)
                                strTmp += "+" + strItems[j].Trim();
                        }
                        return strTmp;

                    case "edition":
                        strTmp = Url.GetEncodeUrlByType(PathType.Edition, strPara[1], baseUrl, false);
                        for (j = 0; j < strItems.Length; j++)
                        {
                            if (j != i)
                                strTmp += "+" + strItems[j].Trim();
                        }

                        return strTmp;
                    default:
                        break;
                }
            }
            return path;
        }


        private static string HtmlPathEncode_PublicSite(string left, string current, string baseUrl)
        {
            string strQ = "\"";
            if (left.EndsWith("("))
                strQ = ")";
            else if (left.EndsWith("'"))
                strQ = "'";

            int pos = current.IndexOf(strQ);

            string strTmp;
            if (pos <= 0)
                strTmp = current;
            else
                strTmp = current.Substring(0, pos);

            string strEn = VirtualPathEncode1_PublicSite(strTmp, baseUrl);

            current = current.Replace(strTmp, strEn);

            return current;
        }
        private static string EncodeUrls_AdminSite(string left, string current,
            bool emailVersion,
            bool isDistributeVersion,
            string baseUrlPublicSite,
            string baseUrlAdminSite)
        {
            string strQ = "\"";
            if (left.EndsWith("("))
                strQ = ")";
            else if (left.EndsWith("'"))
                strQ = "'";


            int pos = current.IndexOf(strQ);

            string strTmp;
            if (pos <= 0)
                strTmp = current;
            else
                strTmp = current.Substring(0, pos);

            string strEn = VirtualPathEncode1_AdminSite(strTmp, emailVersion, isDistributeVersion, baseUrlPublicSite, baseUrlAdminSite);


            current = current.Replace(strTmp, strEn);

            return current;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">File, Image, Article or Edition</param>
        /// <param name="id">The id of the file, image, article or edition</param>
        /// <param name="baseUrl">The base of the url, ie. http://www.enewsletters.gov.bc.ca/ </param>
        /// <param name="isForPublicSite"></param>
        /// <returns></returns>
        public static string GetEncodeUrlByType(PathType type,
            string id,
            string baseUrl,
            bool isForPublicSite)
        {
            string strTmp = "";
            switch (type)
            {
                case PathType.File:
                case PathType.Image:
                    strTmp = Url.GetFileEncodedUrl(type, id);
                    break;
                case PathType.Article:
                    strTmp = Url.GetArticleEncodedUrl(id, isForPublicSite);
                    break;
                case PathType.Edition:
                default:
                    strTmp = Url.GetEditionEncodedUrl(id, isForPublicSite);
                    break;
            }

            if (!strTmp.Contains(baseUrl))
                strTmp = baseUrl + strTmp;

            strTmp = Url.ReplaceChars(strTmp);
            return strTmp;
        }



        #endregion


        #region "Decode"

        public static string DecodeUrl(string url, string getFileLocation, bool isPublicSite)
        {
            string[] strItems = url.Split('/');
            if (strItems.Length < 2)
                return url;


            string strTmp = url;

            if (strItems[strItems.Length - 1].ToLower() == "edition"
                || strItems[strItems.Length - 2].ToLower() == "edition") //the 2nd one is for historical urls that had the id at the end
                strTmp = Url.GetDecodedEditionVirtualPath(url, getFileLocation, isPublicSite);

            else if (strItems[strItems.Length - 1].ToLower() == "article"
                || strItems[strItems.Length - 2].ToLower() == "article") //the 2nd one is for historical urls that had the id at the end
                strTmp = Url.GetDecodedArtilceVirtualPath(url, getFileLocation, isPublicSite);

            else
                switch (strItems[strItems.Length - 2].ToLower())
                {
                    case "image":
                        strTmp = Url.GetDecodedFileVirtualPath(getFileLocation,
                            PathType.Image,
                            strItems[strItems.Length - 1]);
                        break;
                    case "file":
                        strTmp = Url.GetDecodedFileVirtualPath(getFileLocation,
                            PathType.File,
                            strItems[strItems.Length - 1]);
                        break;
                    case "corner":
                        strTmp = Url.GetDecodedFileVirtualPath(getFileLocation,
                            PathType.Corner,
                            strItems[strItems.Length - 1]);
                        break;
                    default:
                        break;
                }

            return strTmp;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetDecodedFileVirtualPath(string getFileLocation, PathType type, string id)
        {
            string strTmp = getFileLocation;
            id = id.Replace("+", "&");
            switch (type)
            {
                case PathType.File:
                case PathType.Image:
                    strTmp += "getfile.aspx?guid=" + id;
                    break;
                case PathType.Corner:
                    strTmp += "getfile.aspx?" + id;
                    break;

                default:
                    break;
            }
            return strTmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">the path after the base url</param>
        /// <returns>
        /// Returns the getfile.aspx with edition id from the url path
        /// Utility._Base_URL or Utility._Base_URL_Admin
        /// </returns>
        private static string GetDecodedEditionVirtualPath(string path, string getFileLocation, bool isPublicSite)
        {
            string strTmp;
            int id; //id is no longer passed, just the name so we need to find the id based on the path...

            if (!isPublicSite)
                path = path.Replace("Internal/", "");

            id = Url.GetEditionIdFromEncodedUrl(path);
            strTmp = getFileLocation + "getfile.aspx?editionid=" + id.ToString();

            if (!isPublicSite)
            {
                string[] strItems = path.Split('/');
                if (strItems[strItems.Length - 1].ToLower() == "txt")
                    strTmp += "&ishtml=0";
            }

            return strTmp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">the path after the base url</param>
        /// <returns>
        /// Returns the getfile.aspx with edit id from the url path
        /// </returns>
        private static string GetDecodedArtilceVirtualPath(string path, string getFileLocation, bool isPublicSite)
        {
            string strTmp;
            int id; //id is no longer passed, just the name so we need to find the id based on the path...

            if (!isPublicSite)
                path = path.Replace("Internal/", "");

            id = Url.GetArticleIdFromEncodedUrl(path);

            strTmp = getFileLocation + "getfile.aspx?articleid=" + id.ToString();


            return strTmp;
        }


        #endregion








        /// <summary>
        /// ** IF Any changes are made to this, you must update sql function ReplaceCharsForUrl
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceChars(string str)
        {
            string temp = str;

            temp = temp.Replace("%20", "_"); //Remove Spaces
            temp = temp.Replace(" ", "_"); //Remove Spaces
            temp = temp.Replace(",", "_"); //Remove Commas
            temp = temp.Replace("%2c", "_"); //Remove Commas
            temp = temp.Replace("'", ""); //Remove Single Quotes
            temp = temp.Replace("%2f", ""); //Remove Forward Slash
            temp = temp.Replace("%3f", ""); //Remove Question Mark
            temp = temp.Replace("__", "_");

            return temp;
        }




        public static string ReplaceCharsForUrl(string txt)
        {
            string nextTxt = txt.Trim();
            nextTxt = nextTxt.Replace(" ", "_");
            nextTxt = nextTxt.Replace("?", "");
            nextTxt = nextTxt.Replace("/", "");
            nextTxt = nextTxt.Replace("'", "");
            nextTxt = nextTxt.Replace(",", "_");
            nextTxt = nextTxt.Replace("__", "_");

            return nextTxt;
        }


        public static string ReplaceEnvironmentURLs_ForPublicSite(string toModify, string baseUrl)
        {
            string strTmp = toModify;

            // Deal with environment specific images
            strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/admin/", baseUrl);
            strTmp = strTmp.Replace("https://uat.enewsletters.gov.bc.ca/admin/", baseUrl);
            strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("http://uat.enewsletters.gov.bc.ca/", baseUrl);

            strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/admin/", baseUrl);
            strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/", baseUrl);

            strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/admin/", baseUrl);
            strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("https://dev.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("http://dev.enewsletters.gov.bc.ca/", baseUrl);

            strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/admin/", baseUrl);
            strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/", baseUrl);
            strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/", baseUrl);

            strTmp = strTmp.Replace("https://localhost/admin/", baseUrl);
            strTmp = strTmp.Replace("http://localhost/", baseUrl);

            strTmp = strTmp.Replace("https://localhost:4087/admin/", baseUrl);
            strTmp = strTmp.Replace("http://localhost:4087/", baseUrl);

            // For articles we are not using a hard coded URL anymore, so point it to the right direction.
            // ie. <a href="getfile.aspx?article=3235">
            //strTmp = strTmp.Replace("href=\"getfile.aspx", "href=\"" + baseUrl + "getfile.aspx");
            //strTmp = strTmp.Replace("href=\"getfile.aspx?guid=", "href=\"" + baseUrl + "/");
            strTmp = strTmp.Replace("getfile.aspx?guid=", baseUrl + "/");
            return strTmp;
        }
        //https://www.enewsletters.gov.bc.ca/getfile.aspx?guid=8d80cfb0-44b7-4c42-b683-bf7225a058a7&amp;w=312&amp;h=525
        //<img hspace="10" alt="Yavhel Velazquez, Manager, WorldHost Training Services" vspace="10" align="right" width="186" height="313" src="https://www.enewsletters.gov.bc.ca/getfile.aspx?guid=8d80cfb0-44b7-4c42-b683-bf7225a058a7&amp;w=312&amp;h=525" />


        ///// <summary>
        ///// we need to do this as the dev/test/uat environments share a database 
        ///// and the wysiwig stores absolute references
        ///// </summary>
        ///// <param name="toModify"></param>
        ///// <returns></returns>
        public static string ReplaceEnvironmentURLs_ForAdminSite(string toModify, string publicUrl, string adminUrl)
        {
            string strTmp = toModify;

            // Deal with environment specific images
            //TODO: This could be updated to use regular expressions 



            // https versions 
            strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("https://uat.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/admin/", adminUrl);

            // http version
            strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("http://uat.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/admin/", adminUrl);
            strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/admin/", adminUrl);

            // newer admin version
            strTmp = strTmp.Replace("http://uat.admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://uat.admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://test.admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://test.admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://dev.admin.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://dev.admin.enewsletters.gov.bc.ca/", adminUrl);


            // Let's try this with the admin url
            strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://uat.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://uat.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/", adminUrl);
            strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/", adminUrl);


            /// public sites with and without www
            //strTmp = strTmp.Replace("http://uat.www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("https://uat.www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("http://uat.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("https://uat.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("http://www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("https://www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("http://test.www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("https://test.www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("http://dev.www.enewsletters.gov.bc.ca/", publicUrl);
            //strTmp = strTmp.Replace("https://dev.www.enewsletters.gov.bc.ca/", publicUrl);


            strTmp = strTmp.Replace("https://localhost/admin/", adminUrl);
            strTmp = strTmp.Replace("http://localhost/", publicUrl);

            strTmp = strTmp.Replace("https://localhost:4088/admin/", adminUrl);
            strTmp = strTmp.Replace("https://localhost:4088/", adminUrl);
            //strTmp = strTmp.Replace("http://localhost:4087/", publicUrl);

            strTmp = strTmp.Replace(adminUrl + "getfile.aspx", adminUrl + "pages/getfile.aspx");
            strTmp = strTmp.Replace(publicUrl + "getfile.aspx", adminUrl + "pages/getfile.aspx");

            return strTmp;
        }


    }
}

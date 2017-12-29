using Gcpe.ENewsletters.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcpe.ENewsletters.Providers
{
    public class Newsroom
    {
        protected readonly ENewslettersEntities db;
        public Newsroom (ENewslettersEntities db)
        {
            this.db = db;
        }

        // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Newsroom" in code, svc and config file together.
        // NOTE: In order to launch WCF Test Client for testing this service, please select Newsroom.svc or Newsroom.svc.cs at the Solution Explorer and start debugging.
        public const string SUBSCRIBE_SOURCE_PUBLIC = "Public Signup Page";
        /// <summary>
        /// Newsletters that have at least one approved edition with a publish date less than or equal to the current date.
        /// </summary>
        /// <returns>List of newsletters</returns>
        public List<PublicNewsletterListings> GetNewsletters_PublicOnly()
        {
            List<PublicNewsletterListings> lst = new List<PublicNewsletterListings>();

            

                lst =
                    (
                    from x in
                        (
                        from nid in /* This gets a distinct list of newsletterIDs and the latest published date */
                            (
                                from e in db.editions.Where(f => f.active == (int)EditionStatus.Approved
                                    && f.displayPublic == true
                                    && f.originalPublishDt <= DateTime.Now
                                    && f.originalPublishDt != null
                                    )
                                orderby e.originalPublishDt descending
                                group e by new { NewsletterID = e.newsletterid } into g
                                select new
                                {
                                    NewsletterID = g.Key.NewsletterID.Value,
                                    LatestPublishDate = g.Max(e => e.originalPublishDt).Value /* latest publish date */
                                }
                            )
                        join latestEdition in db.editions on //nid.EditionID equals latestEdition.editionid
                            new { nid.NewsletterID, nid.LatestPublishDate }
                            equals new { NewsletterID = latestEdition.newsletterid.Value, LatestPublishDate = latestEdition.originalPublishDt.Value }
                            /* Now we grab that edition only by joining on both columns from nid */

                        join ef in db.folders on latestEdition.folderid equals ef.folderid
                        join n in db.newsletters.Where(f => f.nlStatus == (int)NewsletterStatus.Active || f.nlStatus == (int)NewsletterStatus.In_Active) on nid.NewsletterID equals n.newsletterid /* newsletter is active or inactive */
                        join nf in db.folders on n.folderid equals nf.folderid /* get the folder name for creating the URL */
                        join u in db.usergroups.Where(f => f.active == true) on n.ownerusergroupid equals u.usergroupid /* groups (ministries) that are active */

                        select new PublicNewsletterListings
                        {
                            MinistryID = u.usergroupid,
                            MinistryName = u.description,
                            MinistryNewsletterArchiveLink = u.newsletter_link.Trim(),
                            NewsletterID = n.newsletterid,
                            NewsletterName = n.name,
                            NewsletterHasSubscribeOption = n.subscribeyn,
                            NewsletterDefaultDistributionListID = n.newsletterid, // use newsletterid to make Repository.newslettersLookup work
                            LatestEditionName = latestEdition.name,
                            LatestEditionURL = nf.description + "/" + ef.description,
                            LatestEditionID = latestEdition.editionid,
                            LatestPublishDate = nid.LatestPublishDate,
                            NewsletterDescription = n.description,
                            NewsletterKey = n.key,
                            LatestEditionKey = latestEdition.key
                        }
                        )
                    orderby x.MinistryName, x.NewsletterName, x.LatestEditionName ascending /* Wrapper to apply sort */
                    select x
                    ).ToList();
            

            return lst;
        }

        public Tuple<string, string>[] GetNewslettersByMinistry(int[] newsletterIds)
        {
            
                var results = (from c in db.newsletters
                               where newsletterIds.Contains(c.newsletterid)
                               select new { key = c.key, name = c.name }
                        );

                var newsletters = new List<Tuple<string, string>>();
                foreach (var result in results)
                {
                    var tuple = new Tuple<string, string>(result.key, result.name);
                    newsletters.Add(tuple);
                }

                return newsletters.ToArray();

           
        }

        public Newsletter GetNewsletter(int newsletterId)
        {

            Newsletter item = new Newsletter();

            

                item =
                        (
                        from n in db.newsletters.Where(f => f.nlStatus == (int)NewsletterStatus.Active || f.nlStatus == (int)NewsletterStatus.In_Active) /* newsletter is active or inactive */
                        join nf in db.folders on n.folderid equals nf.folderid /* get the folder name for creating the URL */
                        join u in db.usergroups.Where(f => f.active == true) on n.ownerusergroupid equals u.usergroupid /* groups (ministries) that are active */
                        where n.newsletterid == newsletterId
                        select new Newsletter
                        {
                            MinistryName = u.description,
                            NewsletterID = n.newsletterid,
                            NewsletterName = n.name,
                            NewsletterHasSubscribeOption = n.subscribeyn,
                            //NewsletterDefaultDistributionListID = n.distributionlistid,
                            NewsletterCreateDate = n.createdate,
                        }
                    ).Single();
            

            return item;
        }

        public List<EditionList> GetEditions_PublicOnly(int newsletterId)
        {
            List<PublicNewsletterListings> lst = new List<PublicNewsletterListings>();

                return (from e in db.editions
                        join ef in db.folders on e.folderid equals ef.folderid
                        join n in db.newsletters on e.newsletterid equals n.newsletterid
                        join nf in db.folders on n.folderid equals nf.folderid
                        where e.newsletterid == newsletterId
                        && e.active == (int)EditionStatus.Approved
                        && e.displayPublic == true
                        && e.originalPublishDt <= DateTime.Now
                        orderby e.originalPublishDt descending
                        select new EditionList
                        {
                            EditionName = e.name,
                            EditionFolderName = ef.description,
                            NewsletterFolderName = nf.description
                        }).ToList();
            
        }

        public List<EditionList> GetEditions_All_PublicOnly()
        {
            List<PublicNewsletterListings> lst = new List<PublicNewsletterListings>();

                return (from e in db.editions
                        join ef in db.folders on e.folderid equals ef.folderid
                        join n in db.newsletters on e.newsletterid equals n.newsletterid
                        join nf in db.folders on n.folderid equals nf.folderid
                        where e.active == (int)EditionStatus.Approved
                        orderby e.originalPublishDt descending
                        select new EditionList
                        {
                            EditionID = e.editionid,
                            EditionName = e.name,
                            EditionFolderName = ef.description,
                            NewsletterFolderName = nf.description,
                            NewsletterID = e.newsletterid.Value,
                            NewsletterKey = n.key,
                            EditionKey = e.key,
                            NewsletterName = n.name,
                            DisplayPublic = e.displayPublic,
                            Status = e.active
                        }).ToList();
            
        }

        public string GetNewsUrlFromNewslettersUrl(string url)
        {
            return Templates.Url.GetNewsUrlFromNewslettersUrl(url);
        }

        public Tuple<string, DateTime> GetEditionBody_Key(string newsletterKey, string editionKey)
        {
            int id = Templates.Url.GetEditionIdFromKey(newsletterKey, editionKey);
            if (id <= 0)
                return null;

            Tuple<string, DateTime> editionBodyAndDate = Templates.Edition.CreateEditionByEditionId_HtmlFormat_ForPublic(id, Templates.Template.BASE_URL_TO_REPLACE, Templates.Template.BASE_CONTENT_URL_TO_REPLACE);

            editionBodyAndDate = new Tuple<string, DateTime>(editionBodyAndDate.Item1, editionBodyAndDate.Item2);
            return editionBodyAndDate;
        }

        public NewsletterResource GetNewsletterResourcesByGuid(Guid guid)
        {
            Templates.Model.Image img = Templates.Url.GetFileByGuid(guid.ToString());

            if (img.File != null && img.File.Length == 0)
                img = Templates.Url.GetFileByName(guid.ToString());

            if (img.File == null)
                return null;

            var model = new NewsletterResource();
            model.FileName = img.FileName;
            model.ContentType = img.FileType;
            model.Content = img.File.ToArray();
            model.Timestamp = img.CreatedDate;
            return model;
        }

        public Tuple<string, DateTime> GetArticleBody(string articlekeypath)
        {
            int id = Templates.Url.GetArticleIdFromEncodedPath(articlekeypath);
            string html = Templates.Article.CreateArticle_PublicSite(id, Templates.Template.BASE_URL_TO_REPLACE, articlekeypath);
            return new Tuple<string, DateTime>(html, new DateTime(1900, 1, 1));
        }

        public List<ArticleList> GetArticles_All_PublicOnly()
        {
            List<PublicNewsletterListings> lst = new List<PublicNewsletterListings>();
            
                return
                         (from art in db.articles.Where(x => x.active != (int)ArticleStatus.Deleted)
                          join ed in db.editions.Where(x => x.active != (int)EditionStatus.Deleted) on art.editionid equals ed.editionid
                          join nl in db.newsletters on ed.newsletterid equals nl.newsletterid
                          join f in db.folders on ed.folderid equals f.folderid
                          join f1 in db.folders on f.parentfolderid equals f1.folderid
                          select new ArticleList
                          {
                              ArticleID = art.articleid,
                              ArticleCreateDate = art.createdate,
                              EditionID = ed.editionid,
                              EditionKey = ed.key,
                              EditionName = ed.name,
                              NewsletterName = nl.name,
                              NewsletterKey = nl.key,
                              ArticleFolder = art.foldername,
                              EditionFolder = f.description,
                              NewsletterFolder = f1.description,
                              ArticleKey = art.key
                          }).ToList();
            
        }

        #region Subscribe

        /// <summary>
        /// A list of newsletters that are available to the public to subscribe to
        /// </summary>
        /// <returns>Newsletter Name and Default Distribution List Name</returns>
        public List<NewsletterList> GetNewsletters_PublicOnly_WithSubscribe()
        {
            
                return
                    (
                        from n in db.newsletters
                        join e in db.editions on n.newsletterid equals e.newsletterid
                        where
                        (
                            n.nlStatus == (int)NewsletterStatus.Active  /* newsletter is active or inactive */
                            ||
                            n.nlStatus == (int)NewsletterStatus.In_Active
                        )
                        && n.subscribeyn == true    /* newsletter subscribe option is set to true */
                        && e.active == (int)EditionStatus.Approved    /* only active editions */
                        && e.displayPublic == true  /* can be displayed publicly */
                        && e.originalPublishDt <= DateTime.Now /* published date is less than now */
                        group n by new { n.name, n.newsletterid } // use newsletterid to make Repository.newslettersLookup work
                            into g
                        orderby g.Key.name
                        select new NewsletterList
                        {
                            NewsletterName = g.Key.name,
                            NewsletterDefaultDistributionListId = g.Key.newsletterid
                        }
                    ).ToList();
            
        }

        /*
        /// <summary>
        /// Newsletters a person subscribed to at one time
        /// </summary>
        /// <param name="groupGuidId"></param>
        /// <returns>List of newsletters</returns>
        public List<NewsletterList> GetNewsletters_BySubscriptionRequest(string groupGuidId)
        {
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                return
                    (
                        from n in db.newsletters
                        join s in db.subscribers on n.distributionlistid equals s.distributionlistid
                        where (
                            n.nlStatus == (int)NewsletterStatus.Active
                            ||
                            n.nlStatus == (int)NewsletterStatus.In_Active
                        )
                        && n.subscribeyn == true // newsletter subscribe option is set to true
                        && s.groupGuid == groupGuidId
                        group n by new { n.name, n.distributionlistid }
                            into g
                        select new NewsletterList
                        {
                            NewsletterName = g.Key.name,
                            NewsletterDefaultDistributionListId = g.Key.distributionlistid
                        }
                    ).ToList();
            }
        }

        /// <summary>
        /// For a given email, returns all newsletters they are subscribed to
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<NewsletterList> GetNewsletters_BySubscriber(string email)
        {
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                return
                    (
                        from n in db.newsletters
                        join s in db.subscribers.Where(x => x.active == 1 && x.email == email) on n.distributionlistid equals s.distributionlistid
                        where (
                            n.nlStatus == (int)NewsletterStatus.Active
                            ||
                            n.nlStatus == (int)NewsletterStatus.In_Active
                        )

                        group n by new { n.name, n.key, n.distributionlistid }
                            into g
                        select new NewsletterList
                        {
                            NewsletterName = g.Key.name,
                            NewsletterDefaultDistributionListId = g.Key.distributionlistid,
                            Key = g.Key.key
                        }
                    ).ToList();
            }
        }

        /// <summary>
        /// Looks up the email in one of the records that is in the group
        /// </summary>
        /// <param name="groupGuidId"></param>
        /// <returns>Email of subscriber</returns>
        public string GetSubscriberEmail_BySubscriptionRequest(string groupGuidId)
        {
            string email = string.Empty;
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                email = (from s in db.subscribers
                         where s.groupGuid == groupGuidId
                         select s.email).FirstOrDefault();
            }
            return email;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="htmlFormat"></param>
        /// <param name="listOfDistributionListIds"></param>
        /// <returns></returns>
        public List<int> InsertSubscriberNoConfirmation(string email, bool htmlFormat, string listOfDistributionListIds, Guid newGroupId)
        {
            try
            {
                using (ENewslettersEntities db = new ENewslettersEntities())
                {
                    string listOfDistributionListIdsIntsString = null;
                    if (listOfDistributionListIds != null)
                    {
                        var newsletterKeys = listOfDistributionListIds.Split(',');
                        var listOfDistributionListIdsList = new List<string>();
                        foreach (var newsLetterKey in newsletterKeys)
                        {
                            var newsletter = db.newsletters.SingleOrDefault(n => n.key == newsLetterKey);
                            if (newsletter != null && newsletter.distributionlistid.HasValue)
                                listOfDistributionListIdsList.Add(newsletter.distributionlistid.Value.ToString());
                        }

                        listOfDistributionListIdsIntsString = string.Join(",", listOfDistributionListIdsList);
                    }

                    var newListIds = new List<int>();
                    bool addedNewSubscription = InsertSubscriber(email, htmlFormat, listOfDistributionListIdsIntsString, newGroupId, out newListIds);
                    return addedNewSubscription ? newListIds : new List<int>();
                }

            }
            catch (Exception e)
            {
                LogMessage(e.Message, e.StackTrace);
                throw;
            }
        }

        private static readonly string LogFile = System.Configuration.ConfigurationManager.AppSettings["LoggingFileName"];

        public static void LogMessage(string strMessage, string stacktrace)
        {
            if (String.IsNullOrEmpty(LogFile))
                return;

            try
            {
                string fileName = LogFile + DateTime.Now.ToString("-yyyyMMdd") + ".log";
                string timeString = DateTime.Now.ToString("HH:mm:ss-");

                lock (LogFile)
                {
                    System.IO.File.AppendAllText(fileName, timeString + " - " + strMessage + "\r\n");
                    System.IO.File.AppendAllText(fileName, timeString + " - " + stacktrace + "\r\n");
                }
            }
            catch (System.IO.IOException)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="htmlFormat"></param>
        /// <param name="listOfDistributionListIds"></param>
        /// <returns></returns>
        public void InsertSubscriberAndSendConfirmation(string email, bool htmlFormat, string listOfDistributionListIds)
        {
            var newGroupId = Guid.NewGuid();
            List<int> newListIds;
            bool addedNewSubscription = InsertSubscriber(email, htmlFormat, listOfDistributionListIds, newGroupId, out newListIds);

            if (addedNewSubscription)
                Emailer.SendNewSubscriptionEmail(email, newGroupId.ToString(), newListIds); // With Activate Link
            else
                Emailer.SendAlreadySubscribedEmail(email, listOfDistributionListIds); // They didn't subscribe to any new ones
        }

        private bool InsertSubscriber(string email, bool htmlFormat, string listOfDistributionListIds,
                                                            Guid newGroupId, out List<int> newListIds)
        {
            bool addedNewSubscription = false;

            int format = (int)EmailFormat.Html;
            if (!htmlFormat)
                format = (int)EmailFormat.Text;

            // First thing to do is to create a new list of distribution lists to subscribe the person to.
            // They may have chosen to subscribe to newsletters they have already subsribed to...
            //      so these ids are removed so we don't add duplicates.
            newListIds = listOfDistributionListIds == null ? new List<int>() : listOfDistributionListIds.Trim(',').Trim().Split(',').Select(Int32.Parse).ToList();
            List<int> alreadySubscribedTo;
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                alreadySubscribedTo = (from s in db.subscribers
                                       where s.email == email.Trim()
                                       && s.active == (int)SubscriptionStatus.Active
                                       select s.distributionlistid.Value).ToList();
            }

            var newRecords = new List<int>(newListIds);

            // add all the new records
            newRecords.RemoveAll(x => alreadySubscribedTo.Contains(x));
            if (newRecords.Any())
            {
                addedNewSubscription = true;
                using (ENewslettersEntities db = new ENewslettersEntities())
                {
                    foreach (int listId in newRecords)
                    {
                        var newOne = new subscriber();
                        newOne.email = email;
                        newOne.groupGuid = newGroupId.ToString();
                        newOne.guid = Guid.NewGuid().ToString(); //would be good to not have these be string cols
                        newOne.emailformat = format;
                        newOne.createdate = DateTime.Now;
                        newOne.distributionlistid = listId;
                        newOne.createsource = SUBSCRIBE_SOURCE_PUBLIC;
                        newOne.active = (int)SubscriptionStatus.Active;

                        db.subscribers.Add(newOne);
                    }

                    db.SaveChanges();
                }
            }

            newRecords = new List<int>(newListIds);
            // find all the records that should be removed
            alreadySubscribedTo.RemoveAll(x => newRecords.Contains(x));
            if (alreadySubscribedTo.Any())
            {
                using (var db = new ENewslettersEntities())
                {
                    foreach (int listId in alreadySubscribedTo)
                    {
                        var subscriber = db.subscribers
                                .SingleOrDefault(x => x.email == email && x.distributionlistid == listId &&
                                    (x.active == (int)SubscriptionStatus.Active));
                        if (subscriber != null)
                        {
                            subscriber.active = (int)SubscriptionStatus.In_Active;
                            subscriber.unsubscribeddate = DateTime.Now;
                        }
                    }

                    db.SaveChanges();
                }
            }

            return addedNewSubscription;
        }

        /// <summary>
        /// Activates newsletters subscriptions the person subscribed to at one time        
        /// We are going to activate only those subscriptions, which are not already Active. 
        /// If there are several pending under the same grooupGuidId (can happen after manage subscribers call) we only pick one of those records and active it.
        /// </summary>
        /// <param name="groupGuidId"></param>
        public void UpdateSubscription(string groupGuidId)
        {
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                var subscribersInGroup = (from s in db.subscribers
                                          where s.groupGuid == groupGuidId
                                          && s.active == (int)SubscriptionStatus.Pending_Confirmation
                                          group s by s.distributionlistid into distributionListGroup
                                          select distributionListGroup).ToList();

                if (subscribersInGroup.Any())
                {
                    var email = subscribersInGroup.First().First().email;
                    var distributionListIds = subscribersInGroup.Select(s => s.Key).ToList();

                    var alreadySubscribedToIds = (from s in db.subscribers
                                                  where s.email == email && distributionListIds.Contains(s.distributionlistid.Value) && s.active == (int)SubscriptionStatus.Active
                                                  select s.distributionlistid.Value).ToList();

                    foreach (var subGroup in subscribersInGroup)
                    {
                        if (!alreadySubscribedToIds.Contains(subGroup.Key.Value))
                        {
                            var sub = subGroup.First();
                            sub.active = (int)SubscriptionStatus.Active;
                            sub.activedate = DateTime.Now;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Internal Private Functions

        private static bool SubscriberExist_ByGuid(string subscriberGuid)
        {
            using (ENewslettersEntities db = new ENewslettersEntities())
            {
                return (from s in db.subscribers
                        join d in db.distributionlists on s.distributionlistid equals d.distributionlistid
                        join n in db.newsletters on d.distributionlistid equals n.distributionlistid
                        where s.guid == subscriberGuid.Trim()
                        && d.active == 1
                        && s.active == 1
                        && n.nlStatus == (int)NewsletterStatus.Active
                        select s.subscriberid).Any();
            }
        }
        */
        #endregion
    }
}
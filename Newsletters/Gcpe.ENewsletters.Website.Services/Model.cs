using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.ENewsletters
{
    // TODO: THIS WOULD BE GOOD TO BE IN A MODEL PROJECT

    public enum NewsletterStatus
    {
        New_Not_Saved = -1,
        Draft = 1,
        Pending = 2,
        Active = 3,
        In_Active = 4,
        Deleted = 99
    }
    public enum SubscriptionStatus
    {
        Pending_Confirmation = 0,
        Active = 1,
        In_Active = 2
    }
    public enum EditionStatus
    {
        Draft = 1,
        Appoval_Pending = 2,
        Approved = 3,
        Deleted = 99
    }

    public enum ArticleStatus
    {
        Draft = 1,
        Appoval_Pending = 2,
        Approved = 3,
        Deleted = 99
    }

    public enum EmailFormat
    {
        Html = 1, 
        Text = 2
    }

    public class NewsletterList
    {
        public string NewsletterName;
        public int? NewsletterDefaultDistributionListId;
        public string Key;
    }

    public class PublicNewsletterListings
    {
        public int? MinistryID;
        public string MinistryName;
        public string MinistryNewsletterArchiveLink; //This is for when a organization has an archive of newsletters somewhere else, this displays the URL to the archive

        public int NewsletterID;
        public int? NewsletterDefaultDistributionListID;
        public string NewsletterName;
        public bool NewsletterHasSubscribeOption;

        public string LatestEditionURL;
        public string LatestEditionName;
        public DateTime? LatestPublishDate;

        public string NewsletterDescription;
        public int LatestEditionID;
        public string LatestEditionKey;
        public string NewsletterKey;

    }

    public class Newsletter
    {
        public string MinistryName;
        public int NewsletterID;
        public int? NewsletterDefaultDistributionListID;
        public string NewsletterName;
        public bool NewsletterHasSubscribeOption;
        public DateTime? NewsletterCreateDate;
    }

    public class EditionList
    {
        public int NewsletterID;
        public string NewsletterFolderName;
        public string EditionFolderName;
        public string EditionName;
        public int EditionID;
        public string NewsletterKey;
        public string EditionKey;
        public string NewsletterName;
        public int? Status;
        public bool? DisplayPublic;
     }

    public class ArticleList
    {
        public int ArticleID;
        public string EditionName;
        public int EditionID;
        public string NewsletterKey;
        public string EditionKey;
        public string NewsletterName;
        public string ArticleBody;
        public DateTime? ArticleCreateDate;
        public string ArticleFolder;
        public string EditionFolder;
        public string NewsletterFolder;
        public string ArticleKey;
    }



}
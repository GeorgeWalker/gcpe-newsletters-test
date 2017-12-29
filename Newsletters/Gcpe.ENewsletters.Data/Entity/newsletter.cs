//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gcpe.ENewsletters.Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class newsletter
    {
        public newsletter()
        {
            this.approvers = new HashSet<approver>();
            this.NewsletterUserAuthors = new HashSet<NewsletterUserAuthor>();
            this.newslettertemplates = new HashSet<newslettertemplate>();
            this.NewsletterUserDistributors = new HashSet<NewsletterUserDistributor>();
        }
    
        public int newsletterid { get; set; }
        public string name { get; set; }
        public Nullable<int> folderid { get; set; }
        public Nullable<int> ownerusergroupid { get; set; }
        public Nullable<int> nlStatus { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> createuserid { get; set; }
        public Nullable<System.DateTime> updatedate { get; set; }
        public Nullable<int> updateuserid { get; set; }
        public bool subscribeyn { get; set; }
        public string description { get; set; }
        public string key { get; set; }
    
        public virtual ICollection<approver> approvers { get; set; }
        public virtual folder folder { get; set; }
        public virtual user user { get; set; }
        public virtual usergroup usergroup { get; set; }
        public virtual user user1 { get; set; }
        public virtual ICollection<NewsletterUserAuthor> NewsletterUserAuthors { get; set; }
        public virtual ICollection<newslettertemplate> newslettertemplates { get; set; }
        public virtual ICollection<NewsletterUserDistributor> NewsletterUserDistributors { get; set; }
    }
}
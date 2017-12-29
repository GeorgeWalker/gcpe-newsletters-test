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
    
    public partial class user
    {
        public user()
        {
            this.approvers = new HashSet<approver>();
            this.articles = new HashSet<article>();
            this.articles1 = new HashSet<article>();
            this.editions = new HashSet<edition>();
            this.editions1 = new HashSet<edition>();
            this.editions2 = new HashSet<edition>();
            this.files = new HashSet<file>();
            this.folders = new HashSet<folder>();
            this.newsletters = new HashSet<newsletter>();
            this.newsletters1 = new HashSet<newsletter>();
            this.NewsletterUserAuthors = new HashSet<NewsletterUserAuthor>();
            this.NewsletterUserDistributors = new HashSet<NewsletterUserDistributor>();
            this.PageChecks = new HashSet<PageCheck>();
            this.uploads = new HashSet<upload>();
            this.user1 = new HashSet<user>();
        }
    
        public int userid { get; set; }
        public int active { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string idir_id { get; set; }
        public Nullable<int> rolepabadmin { get; set; }
        public Nullable<int> roleministryadmin { get; set; }
        public Nullable<int> roleauthor { get; set; }
        public Nullable<int> roledistributor { get; set; }
        public Nullable<int> roleapprover { get; set; }
        public Nullable<int> rolepublic { get; set; }
        public System.DateTime createdate { get; set; }
        public int createuserid { get; set; }
        public Nullable<System.DateTime> updatedate { get; set; }
        public Nullable<int> updateuserid { get; set; }
        public Nullable<int> usergroupId { get; set; }
    
        public virtual ICollection<approver> approvers { get; set; }
        public virtual ICollection<article> articles { get; set; }
        public virtual ICollection<article> articles1 { get; set; }
        public virtual ICollection<edition> editions { get; set; }
        public virtual ICollection<edition> editions1 { get; set; }
        public virtual ICollection<edition> editions2 { get; set; }
        public virtual ICollection<file> files { get; set; }
        public virtual ICollection<folder> folders { get; set; }
        public virtual ICollection<newsletter> newsletters { get; set; }
        public virtual ICollection<newsletter> newsletters1 { get; set; }
        public virtual ICollection<NewsletterUserAuthor> NewsletterUserAuthors { get; set; }
        public virtual ICollection<NewsletterUserDistributor> NewsletterUserDistributors { get; set; }
        public virtual ICollection<PageCheck> PageChecks { get; set; }
        public virtual ICollection<upload> uploads { get; set; }
        public virtual ICollection<user> user1 { get; set; }
        public virtual user user2 { get; set; }
    }
}

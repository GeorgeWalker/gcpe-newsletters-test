﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gcpe.ENewsletters.Data.Entity
{
    using Microsoft.EntityFrameworkCore;
    using System;

    
    public partial class ENewslettersEntities : DbContext
    {

        //This constructor is required for .NET Core dependency injection
        public ENewslettersEntities(DbContextOptions<ENewslettersEntities> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // unknown foreign key reference
            modelBuilder.Ignore<user>();
            // following are all missing a primary key.
            modelBuilder.Ignore<NewsletterTemplatesBannerImage>();
            modelBuilder.Ignore<NewsletterTemplatesFooterImage>();
            modelBuilder.Ignore<approver>();
            modelBuilder.Ignore<htmlcomponent>();
            modelBuilder.Ignore<NewsletterTemplatesFooterLink>();
            modelBuilder.Ignore<publishstatu>();
            modelBuilder.Ignore<sysdiagram>();
            


        }

        public virtual DbSet<approver> approver { get; set; }
        public virtual DbSet<article> article { get; set; }
        public virtual DbSet<boxcontent> boxcontent { get; set; }
        public virtual DbSet<boxtype> boxtype { get; set; }
        public virtual DbSet<color> color { get; set; }
        public virtual DbSet<cornertype> cornertype { get; set; }
        public virtual DbSet<edition> edition { get; set; }
        public virtual DbSet<file> file { get; set; }
        public virtual DbSet<folder> folder { get; set; }
        public virtual DbSet<htmlcomponent> htmlcomponent { get; set; }
        public virtual DbSet<newsletter> newsletter { get; set; }
        public virtual DbSet<newsletterbox> newsletterbox { get; set; }
        public virtual DbSet<newslettertemplate> newslettertemplate { get; set; }
        public virtual DbSet<NewsletterTemplatesBannerImage> NewsletterTemplatesBannerImage { get; set; }
        public virtual DbSet<NewsletterTemplatesBox> NewsletterTemplatesBox { get; set; }
        public virtual DbSet<NewsletterTemplatesFooterImage> NewsletterTemplatesFooterImage { get; set; }
        public virtual DbSet<NewsletterTemplatesFooterLink> NewsletterTemplatesFooterLink { get; set; }
        public virtual DbSet<NewsletterUserAuthor> NewsletterUserAuthor { get; set; }
        public virtual DbSet<NewsletterUserDistributor> NewsletterUserDistributor { get; set; }
        public virtual DbSet<PageCheck> PageCheck { get; set; }
        public virtual DbSet<publishstatu> publishstatu { get; set; }
        public virtual DbSet<readcounter> readcounter { get; set; }
        public virtual DbSet<sysdiagram> sysdiagram { get; set; }
        public virtual DbSet<upload> upload { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<usergroup> usergroup { get; set; }
        public virtual DbSet<edition_history> edition_history { get; set; }
    }
}

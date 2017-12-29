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

        public virtual DbSet<approver> approvers { get; set; }
        public virtual DbSet<article> articles { get; set; }
        public virtual DbSet<boxcontent> boxcontents { get; set; }
        public virtual DbSet<boxtype> boxtypes { get; set; }
        public virtual DbSet<color> colors { get; set; }
        public virtual DbSet<cornertype> cornertypes { get; set; }
        public virtual DbSet<edition> editions { get; set; }
        public virtual DbSet<file> files { get; set; }
        public virtual DbSet<folder> folders { get; set; }
        public virtual DbSet<htmlcomponent> htmlcomponents { get; set; }
        public virtual DbSet<newsletter> newsletters { get; set; }
        public virtual DbSet<newsletterbox> newsletterboxes { get; set; }
        public virtual DbSet<newslettertemplate> newslettertemplates { get; set; }
        public virtual DbSet<NewsletterTemplatesBannerImage> NewsletterTemplatesBannerImages { get; set; }
        public virtual DbSet<NewsletterTemplatesBox> NewsletterTemplatesBoxes { get; set; }
        public virtual DbSet<NewsletterTemplatesFooterImage> NewsletterTemplatesFooterImages { get; set; }
        public virtual DbSet<NewsletterTemplatesFooterLink> NewsletterTemplatesFooterLinks { get; set; }
        public virtual DbSet<NewsletterUserAuthor> NewsletterUserAuthors { get; set; }
        public virtual DbSet<NewsletterUserDistributor> NewsletterUserDistributors { get; set; }
        public virtual DbSet<PageCheck> PageChecks { get; set; }
        public virtual DbSet<publishstatu> publishstatus { get; set; }
        public virtual DbSet<readcounter> readcounters { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<upload> uploads { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<usergroup> usergroups { get; set; }
        public virtual DbSet<edition_history> edition_history { get; set; }
    }
}

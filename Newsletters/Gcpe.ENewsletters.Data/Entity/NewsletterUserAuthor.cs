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
    
    public partial class NewsletterUserAuthor
    {
        public int NewsletterUserAuthorId { get; set; }
        public int newsletterid { get; set; }
        public int userid { get; set; }
        public Nullable<bool> roleauthor { get; set; }
    
        public virtual newsletter newsletter { get; set; }
        public virtual user user { get; set; }
    }
}

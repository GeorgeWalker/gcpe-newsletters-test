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
    
    public partial class upload
    {
        public int uploadid { get; set; }
        public string title { get; set; }
        public byte[] filedata { get; set; }
        public Nullable<bool> adminonly { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> createuserid { get; set; }
    
        public virtual user user { get; set; }
    }
}

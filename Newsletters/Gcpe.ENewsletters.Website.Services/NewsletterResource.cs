using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.ENewsletters
{

    public class NewsletterResource
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Tag
    {
        public Tag()
        {
            ArticleAndTags = new HashSet<ArticleAndTag>();
            Tagalsos = new HashSet<Tagalso>();
        }

        public string TagId { get; set; }
        public string TagName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual ICollection<ArticleAndTag> ArticleAndTags { get; set; }
        public virtual ICollection<Tagalso> Tagalsos { get; set; }
    }
}

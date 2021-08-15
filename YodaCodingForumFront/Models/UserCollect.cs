using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserCollect
    {
        public string CollectId { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }
        public string CollectclassId { get; set; }
        public string CollectStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual Article Article { get; set; }
        public virtual CollectClass Collectclass { get; set; }
        public virtual UserInfo User { get; set; }
    }
}

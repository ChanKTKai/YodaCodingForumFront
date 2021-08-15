using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserFollow
    {
        public string FollowId { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }
        public string FollowStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual Article Article { get; set; }
        public virtual UserInfo User { get; set; }
    }
}

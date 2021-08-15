using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserFollowTag
    {
        public string FollowtagId { get; set; }
        public string TagId { get; set; }
        public string UserId { get; set; }
        public string FollowtagStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserBlock
    {
        public string BlockId { get; set; }
        public string UserId { get; set; }
        public string BlockUserId { get; set; }
        public string BlockStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual UserInfo User { get; set; }
    }
}

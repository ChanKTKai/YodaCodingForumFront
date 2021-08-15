using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class CollectClass
    {
        public CollectClass()
        {
            UserCollects = new HashSet<UserCollect>();
        }

        public string CollectclassId { get; set; }
        public string UserId { get; set; }
        public string CollectclassName { get; set; }
        public string CollectclassStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual UserInfo User { get; set; }
        public virtual ICollection<UserCollect> UserCollects { get; set; }
    }
}

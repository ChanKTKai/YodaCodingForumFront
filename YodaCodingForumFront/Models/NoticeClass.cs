using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class NoticeClass
    {
        public NoticeClass()
        {
            UserNotices = new HashSet<UserNotice>();
        }

        public string NoticeclassId { get; set; }
        public string NoticeclassName { get; set; }
        public string NoticeclassRemark { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public string LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual ICollection<UserNotice> UserNotices { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserNotice
    {
        public string NoticeId { get; set; }
        public string NoticeclassId { get; set; }
        public string UserId { get; set; }
        public string NoticeContent { get; set; }
        public string NoticeStatus { get; set; }
        public string NoticeSource { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual NoticeClass Noticeclass { get; set; }
        public virtual UserInfo User { get; set; }
    }
}

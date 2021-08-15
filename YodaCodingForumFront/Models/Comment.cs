using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Comment
    {
        public Comment()
        {
            UserLikeComments = new HashSet<UserLikeComment>();
        }

        public string CommentId { get; set; }
        public string ArticleId { get; set; }
        public string CommentClass { get; set; }
        public string CommentContent { get; set; }
        public int CommentLike { get; set; }
        public string CommentStatus { get; set; }
        public string CommentAnswer { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual Article Article { get; set; }
        public virtual ICollection<UserLikeComment> UserLikeComments { get; set; }
    }
}

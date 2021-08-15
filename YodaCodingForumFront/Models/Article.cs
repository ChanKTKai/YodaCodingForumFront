using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Article
    {
        public Article()
        {
            ArticleAndTags = new HashSet<ArticleAndTag>();
            Comments = new HashSet<Comment>();
            UserCollects = new HashSet<UserCollect>();
            UserFollows = new HashSet<UserFollow>();
            UserLikes = new HashSet<UserLike>();
        }

        public string ArticleId { get; set; }
        public string ArticleType { get; set; }
        public string ArticleContent { get; set; }
        public int ArticleViews { get; set; }
        public int ArticleLike { get; set; }
        public string ArticleStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }
        public string ArticleTitle { get; set; }

        public virtual ICollection<ArticleAndTag> ArticleAndTags { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserCollect> UserCollects { get; set; }
        public virtual ICollection<UserFollow> UserFollows { get; set; }
        public virtual ICollection<UserLike> UserLikes { get; set; }
    }
}

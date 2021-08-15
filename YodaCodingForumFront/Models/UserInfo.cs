using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class UserInfo
    {
        public UserInfo()
        {
            Chatrooms = new HashSet<Chatroom>();
            CollectClasses = new HashSet<CollectClass>();
            ReceiveMessages = new HashSet<ReceiveMessage>();
            Reports = new HashSet<Report>();
            SendMessages = new HashSet<SendMessage>();
            UserBlocks = new HashSet<UserBlock>();
            UserCollects = new HashSet<UserCollect>();
            UserFollowers = new HashSet<UserFollower>();
            UserFollows = new HashSet<UserFollow>();
            UserImages = new HashSet<UserImage>();
            UserLikeComments = new HashSet<UserLikeComment>();
            UserLikes = new HashSet<UserLike>();
            UserNotices = new HashSet<UserNotice>();
            UserPoints = new HashSet<UserPoint>();
        }

        public string UserId { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string UserNickname { get; set; }
        public string UserSax { get; set; }
        public DateTime? UserBirthday { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserExperience { get; set; }
        public string UserProfession { get; set; }
        public string UserStatus { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public string LastupdateUser { get; set; }
        public int Version { get; set; }

        public virtual ICollection<Chatroom> Chatrooms { get; set; }
        public virtual ICollection<CollectClass> CollectClasses { get; set; }
        public virtual ICollection<ReceiveMessage> ReceiveMessages { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<SendMessage> SendMessages { get; set; }
        public virtual ICollection<UserBlock> UserBlocks { get; set; }
        public virtual ICollection<UserCollect> UserCollects { get; set; }
        public virtual ICollection<UserFollower> UserFollowers { get; set; }
        public virtual ICollection<UserFollow> UserFollows { get; set; }
        public virtual ICollection<UserImage> UserImages { get; set; }
        public virtual ICollection<UserLikeComment> UserLikeComments { get; set; }
        public virtual ICollection<UserLike> UserLikes { get; set; }
        public virtual ICollection<UserNotice> UserNotices { get; set; }
        public virtual ICollection<UserPoint> UserPoints { get; set; }
    }
}

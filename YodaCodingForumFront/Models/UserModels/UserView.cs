using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YodaCodingForumFront.Models.UserModels
{
    public class UserView
    {
        public List<myQuestion> myQuestion { set; get; }
        public List<likeQuestion> likeQuestion { set; get; }
        public List<collocetQuestion> collocetQuestion { set; get; }
        public List<followQuestion> followQuestion { set; get; }
        public List<usercollocetAndArticle> usercollocetAndArticle { set; get; }
        public List<userlikeAndArticle> userlikeAndArticle { set; get; }
        public List<userfollowAndArticle> userfollowAndArticle { set; get; }
        public List<followerList> follower { set; get; }
        public List<myarticleList> myarticle { set; get; }
        public userinfoList userinfo { set; get; }
        public List<UsaxType> UsaxTypeList { get;  set; }
    }
}

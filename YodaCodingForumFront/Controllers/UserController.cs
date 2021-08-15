using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YodaCodingForumFront.Models;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Mail;
using YodaCodingForumFront.Models.SearchModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using YodaCodingForumFront.Models.UserModels;
using System.IO;

namespace YodaCodingForumFront.Controllers
{
    public class UserController : Controller
    {
        private readonly ArticleDBContext _context;

        public UserController(ArticleDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult user(string account)
        {

            string userName = HttpContext.Request.Query["account"].ToString();
            string id = (from u in _context.UserInfos
                         where u.UserAccount == userName
                         select u.UserId).First();
            //文章或問答類型
            var typeQuery = from a in _context.Articles
                            select a.ArticleType;
            //我的文章
            var myarticleQuery = from u in _context.UserInfos
                                 join a in _context.Articles on u.UserAccount equals a.CreateUser
                                 where a.ArticleType == "A"
                                 where a.ArticleStatus == "N"
                                 where u.UserId == id
                                 orderby u.CreateDate descending
                                 select new myarticleList
                                 {
                                     aid = a.ArticleId,
                                     Title = a.ArticleTitle,
                                     Views = a.ArticleViews,
                                     Like = a.ArticleLike,
                                     Type = a.ArticleType,
                                     Status = a.ArticleStatus,
                                     CreateUser = a.CreateUser,
                                     CreateTime = a.CreateDate,
                                     commendCount = (from c in _context.Comments
                                                     where c.ArticleId == a.ArticleId
                                                     where c.CommentStatus == "N"
                                                     select c).Count(),
                                     tag = (from t in _context.Tags
                                            join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                            where tAnda.ArticleId == a.ArticleId
                                            select t.TagName).ToList(),
                                     userID = (from u in _context.UserInfos
                                               select u.UserId).Single()
                                 };
            //我like的文章
            var likeArticleQuery = from u in _context.UserLikes
                                   join a in _context.Articles on u.ArticleId equals a.ArticleId
                                   where a.ArticleType == "A"
                                   where a.ArticleStatus == "N"
                                   where u.UserId == id
                                   orderby u.CreateDate descending
                                   select new userlikeAndArticle
                                   {
                                       aid = a.ArticleId,
                                       Title = a.ArticleTitle,
                                       Views = a.ArticleViews,
                                       Like = a.ArticleLike,
                                       Type = a.ArticleType,
                                       Status = a.ArticleStatus,
                                       CreateUser = a.CreateUser,
                                       CreateTime = a.CreateDate,
                                       commendCount = (from c in _context.Comments
                                                       where c.ArticleId == a.ArticleId
                                                       where c.CommentStatus == "N"
                                                       select c).Count(),
                                       tag = (from t in _context.Tags
                                              join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                              where tAnda.ArticleId == a.ArticleId
                                              select t.TagName).ToList()
                                   };
            //我follow的文章
            var followArticleQuery = from u in _context.UserFollows
                                     join a in _context.Articles on u.ArticleId equals a.ArticleId
                                     where a.ArticleType == "A"
                                     where a.ArticleStatus == "N"
                                     where u.UserId == id
                                     orderby u.CreateDate descending
                                     select new userfollowAndArticle
                                     {
                                         aid = a.ArticleId,
                                         Title = a.ArticleTitle,
                                         Views = a.ArticleViews,
                                         Like = a.ArticleLike,
                                         Type = a.ArticleType,
                                         Status = a.ArticleStatus,
                                         CreateUser = a.CreateUser,
                                         CreateTime = a.CreateDate,
                                         commendCount = (from c in _context.Comments
                                                         where c.ArticleId == a.ArticleId
                                                         where c.CommentStatus == "N"
                                                         select c).Count(),
                                         tag = (from t in _context.Tags
                                                join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                                where tAnda.ArticleId == a.ArticleId
                                                select t.TagName).ToList()
                                     };
            //我collocet的文章
            var collocetArticleQuery = from u in _context.UserCollects
                                       join a in _context.Articles on u.ArticleId equals a.ArticleId
                                       where a.ArticleType == "A"
                                       where a.ArticleStatus == "N"
                                       where u.UserId == id
                                       orderby u.CreateDate descending
                                       select new usercollocetAndArticle
                                       {
                                           aid = a.ArticleId,
                                           Title = a.ArticleTitle,
                                           Views = a.ArticleViews,
                                           Like = a.ArticleLike,
                                           Type = a.ArticleType,
                                           Status = a.ArticleStatus,
                                           CreateUser = a.CreateUser,
                                           CreateTime = a.CreateDate,
                                           commendCount = (from c in _context.Comments
                                                           where c.ArticleId == a.ArticleId
                                                           where c.CommentStatus == "N"
                                                           select c).Count(),
                                           tag = (from t in _context.Tags
                                                  join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                                  where tAnda.ArticleId == a.ArticleId
                                                  select t.TagName).ToList()
                                       };
            //我的問答
            var myQuestionQuery = from u in _context.UserInfos
                                  join a in _context.Articles on u.UserAccount equals a.CreateUser
                                  where a.ArticleType == "Q"
                                  where (a.ArticleStatus == "A" || a.ArticleStatus == "F")
                                  where u.UserId == id
                                  orderby u.CreateDate descending
                                  select new myQuestion
                                  {
                                      aid = a.ArticleId,
                                      Title = a.ArticleTitle,
                                      Views = a.ArticleViews,
                                      Like = a.ArticleLike,
                                      Type = a.ArticleType,
                                      Status = a.ArticleStatus,
                                      CreateUser = a.CreateUser,
                                      CreateTime = a.CreateDate,
                                      commendCount = (from c in _context.Comments
                                                      where c.ArticleId == a.ArticleId
                                                      where c.CommentStatus == "N"
                                                      select c).Count(),
                                      tag = (from t in _context.Tags
                                             join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                             where tAnda.ArticleId == a.ArticleId
                                             select t).ToList(),
                                      userID = (from u in _context.UserInfos
                                                select u.UserId).Single()
                                  };
            //我like的問答
            var likeQuestionQuery = from u in _context.UserLikes
                                    join a in _context.Articles on u.ArticleId equals a.ArticleId
                                    where a.ArticleType == "Q"
                                    where (a.ArticleStatus == "A" || a.ArticleStatus == "F")
                                    where u.UserId == id
                                    orderby u.CreateDate descending
                                    select new likeQuestion
                                    {
                                        aid = a.ArticleId,
                                        Title = a.ArticleTitle,
                                        Views = a.ArticleViews,
                                        Like = a.ArticleLike,
                                        Type = a.ArticleType,
                                        Status = a.ArticleStatus,
                                        CreateUser = a.CreateUser,
                                        CreateTime = a.CreateDate,
                                        commendCount = (from c in _context.Comments
                                                        where c.ArticleId == a.ArticleId
                                                        where c.CommentStatus == "N"
                                                        select c).Count(),
                                        tag = (from t in _context.Tags
                                               join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                               where tAnda.ArticleId == a.ArticleId
                                               select t).ToList()
                                    };
            //我follow的問答
            var followQuestionQuery = from u in _context.UserFollows
                                      join a in _context.Articles on u.ArticleId equals a.ArticleId
                                      where a.ArticleType == "Q"
                                      where (a.ArticleStatus == "A" || a.ArticleStatus == "F")
                                      where u.UserId == id
                                      orderby u.CreateDate descending
                                      select new followQuestion
                                      {
                                          aid = a.ArticleId,
                                          Title = a.ArticleTitle,
                                          Views = a.ArticleViews,
                                          Like = a.ArticleLike,
                                          Type = a.ArticleType,
                                          Status = a.ArticleStatus,
                                          CreateUser = a.CreateUser,
                                          CreateTime = a.CreateDate,
                                          commendCount = (from c in _context.Comments
                                                          where c.ArticleId == a.ArticleId
                                                          where c.CommentStatus == "N"
                                                          select c).Count(),
                                          tag = (from t in _context.Tags
                                                 join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                                 where tAnda.ArticleId == a.ArticleId
                                                 select t).ToList()
                                      };

            //我collocet的問答
            var collocetQuestionQuery = from u in _context.UserCollects
                                        join a in _context.Articles on u.ArticleId equals a.ArticleId
                                        where a.ArticleType == "Q"
                                        where (a.ArticleStatus == "A" || a.ArticleStatus == "F")
                                        where u.UserId == id
                                        orderby u.CreateDate descending
                                        select new collocetQuestion
                                        {
                                            aid = a.ArticleId,
                                            Title = a.ArticleTitle,
                                            Views = a.ArticleViews,
                                            Like = a.ArticleLike,
                                            Type = a.ArticleType,
                                            Status = a.ArticleStatus,
                                            CreateUser = a.CreateUser,
                                            CreateTime = a.CreateDate,
                                            commendCount = (from c in _context.Comments
                                                            where c.ArticleId == a.ArticleId
                                                            where c.CommentStatus == "N"
                                                            select c).Count(),
                                            tag = (from t in _context.Tags
                                                   join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                                   where tAnda.ArticleId == a.ArticleId
                                                   select t).ToList()
                                        };

            //當下用戶id
            string currentLoginUserID = "";
            if (HttpContext.Session.GetString("userName") != null)
            {
                currentLoginUserID = (from u in _context.UserInfos
                                      where u.UserAccount == HttpContext.Session.GetString("userName")
                                      select u.UserId).First();
            }
            //user資料
            var userInfoQuery = (from uInfo in _context.UserInfos
                                 where uInfo.UserId == id
                                 select new userinfoList
                                 {
                                     Uid = uInfo.UserId,
                                     Uname = uInfo.UserName == null ? "未提供" : uInfo.UserName,
                                     UAccount = uInfo.UserAccount == null ? "未提供" : uInfo.UserAccount,
                                     UNickname = uInfo.UserNickname == null ? "未提供" : uInfo.UserNickname,
                                     USax = uInfo.UserSax,
                                     UBirthday = uInfo.UserBirthday.ToString() == null ? "未提供" : uInfo.UserBirthday.ToString(),
                                     UAddress = uInfo.UserAddress == null ? "未提供" : uInfo.UserAddress,
                                     UEmail = uInfo.UserEmail == null ? "未提供" : uInfo.UserEmail,
                                     UPhone = uInfo.UserPhone == null ? "未提供" : uInfo.UserPhone,
                                     UExperience = uInfo.UserExperience == null ? "未提供" : uInfo.UserExperience,
                                     UProfession = uInfo.UserProfession == null ? "未提供" : uInfo.UserProfession,
                                     UPassword = uInfo.UserPassword,
                                     UPoint = (from uPiont in _context.UserPoints
                                               where uPiont.UserId == id
                                               select uPiont.PointAmount
                                                 ).First(),
                                     UserHaveFollow = (from f in _context.UserFollowers
                                                       where f.FollowerUserId == id
                                                       where f.UserId == currentLoginUserID
                                                       where f.FollowerStatus == "T"
                                                       select f).Count()
                                 }).First();

            //取得使用者圖片圖片
            userInfoQuery.UImage = getUserImg(id, userInfoQuery.USax);

            //我的追蹤者
            var followerQuery = (from f in _context.UserFollowers
                                 join u in _context.UserInfos on f.FollowerUserId equals u.UserId
                                 where f.UserId == id
                                 where f.FollowerStatus == "T"
                                 select new followerList
                                 {
                                     FollowerUserIdID = f.FollowerUserId,
                                     FollowerUserAccount = u.UserAccount,
                                     UserID = f.UserId,
                                     Createuser = f.CreateUser,
                                     FollowerSex = u.UserSax
                                 }).ToList();

            //取得追蹤者圖片
            for (int i = 0; i < followerQuery.Count; i++)
            {
                string fid = followerQuery[i].FollowerUserIdID;
                string fsex = followerQuery[i].FollowerSex;
                followerQuery[i].FollowerImg = getUserImg(fid, fsex);
            }

            List<UsaxType> UsaxType = new List<UsaxType>();
            UsaxType.Add(new UsaxType() { code = "F", name = "男" });
            UsaxType.Add(new UsaxType() { code = "M", name = "女" });
            UsaxType.Add(new UsaxType() { code = "N", name = "不提供" });



            var UserVM = new UserView
            {
                myQuestion = myQuestionQuery.ToList(),
                likeQuestion = likeQuestionQuery.ToList(),
                collocetQuestion = collocetQuestionQuery.ToList(),
                followQuestion = followQuestionQuery.ToList(),
                userlikeAndArticle = likeArticleQuery.ToList(),
                userinfo = userInfoQuery,
                myarticle = myarticleQuery.ToList(),
                userfollowAndArticle = followArticleQuery.ToList(),
                usercollocetAndArticle = collocetArticleQuery.ToList(),
                follower = followerQuery,
                UsaxTypeList = UsaxType,

            };


            return View(UserVM);

        }

        //添加追蹤者(尚未完成)
        public ActionResult addFollower(string type, string userName, string targetID, string targetType, string userDo)
        {
            var userID = (from u in _context.UserInfos
                          where u.UserAccount == userName
                          select u.UserId).ToList()[0];
            var havefollow = (from ul in _context.UserFollowers
                              where ul.FollowerUserId == targetID
                              where ul.UserId == userID
                              select ul).ToList().Count();
            if (userDo == "up")
            {

                if (havefollow == 0)
                {
                    UserFollower userfollow = new UserFollower
                    {
                        UserId = userID,
                        FollowerUserId = targetID,
                        CreateUser = userName,
                        CreateDate = DateTime.Now,
                        LastupdateUser = userName,
                        LastupdateDate = DateTime.Now,
                    };
                    _context.UserFollowers.Add(userfollow);
                    _context.SaveChanges();
                    return Content("Success");
                }
                else
                {
                    var userfollowID = (from ul in _context.UserFollowers
                                        where ul.FollowerUserId == targetID
                                        where ul.UserId == userID
                                        select ul.FollowerId).ToList()[0];
                    var updateUserCollect = _context.UserFollowers.Find(userfollowID);
                    updateUserCollect.FollowerStatus = "T";
                    updateUserCollect.LastupdateDate = DateTime.Now;
                    updateUserCollect.Version++;
                    _context.SaveChanges();
                    return Content("Success");
                }
            }
            else
            {
                var userfollowID = (from ul in _context.UserFollowers
                                    where ul.FollowerUserId == targetID
                                    where ul.UserId == userID
                                    select ul.FollowerId).ToList()[0];
                var updateUserCollect = _context.UserFollowers.Find(userfollowID);
                updateUserCollect.FollowerStatus = "F";
                updateUserCollect.LastupdateDate = DateTime.Now;
                updateUserCollect.Version++;
                _context.SaveChanges();
                return Content("Success");
            };

        }

        //取消追蹤
        public IActionResult followerDelete(string id, string aid)
        {
            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(aid))
            {
                return Redirect("article/aritclewall");
            }
            string userAccount = (from u in _context.UserInfos
                                  where u.UserId == aid
                                  select u.UserAccount).First();
            if (userAccount != HttpContext.Session.GetString("userName"))
            {
                return Redirect("/article/articlewall");
            }
            var followerID = (from f in _context.UserFollowers
                              where f.FollowerUserId == id
                              where f.UserId == aid
                              select f.FollowerId).ToList()[0];
            var deletefollower = _context.UserFollowers.Find(followerID);
            deletefollower.FollowerStatus = "F";
            deletefollower.Version++;

            _context.SaveChanges();
            return Redirect($"/user/user?account={userAccount}");
        }


        public IActionResult userEdit()
        {

            string id = HttpContext.Request.Query["u"].ToString();
            string userAccount = (from u in _context.UserInfos
                                  where u.UserId == id
                                  select u.UserAccount).ToList().First();
            if (userAccount != HttpContext.Session.GetString("userName"))
            {
                return Redirect("/article/articlewall");
            }
            var userInfoQuery = (from uInfo in _context.UserInfos.DefaultIfEmpty()
                                 where uInfo.UserId == id
                                 select new userinfoList
                                 {
                                     Uid = uInfo.UserId,
                                     Uname = uInfo.UserName == null ? "未提供" : uInfo.UserName,
                                     UAccount = uInfo.UserAccount == null ? "未提供" : uInfo.UserAccount,
                                     UNickname = uInfo.UserNickname == null ? "未提供" : uInfo.UserNickname,
                                     USax = uInfo.UserSax == null ? "未提供" : uInfo.UserSax,
                                     UBirthday = uInfo.UserBirthday.ToString() == null ? "未提供" : uInfo.UserBirthday.ToString(),
                                     UAddress = uInfo.UserAddress == null ? "未提供" : uInfo.UserAddress,
                                     UEmail = uInfo.UserEmail == null ? "未提供" : uInfo.UserEmail,
                                     UPhone = uInfo.UserPhone == null ? "未提供" : uInfo.UserPhone,
                                     UExperience = uInfo.UserExperience == null ? "未提供" : uInfo.UserExperience,
                                     UProfession = uInfo.UserProfession == null ? "未提供" : uInfo.UserProfession,
                                     UPassword = uInfo.UserPassword,
                                     UPoint = (from uPiont in _context.UserPoints
                                               where uPiont.UserId == id
                                               select uPiont.PointAmount
                                                 ).First()
                                 }).First();
            List<UsaxType> UsaxType = new List<UsaxType>();
            UsaxType.Add(new UsaxType() { code = "F", name = "男" });
            UsaxType.Add(new UsaxType() { code = "M", name = "女" });
            UsaxType.Add(new UsaxType() { code = "N", name = "不提供" });
            var UserVM = new UserView
            {
                userinfo = userInfoQuery,
                UsaxTypeList = UsaxType
            };

            return View(UserVM);
        }

        //編輯使用者
        [HttpPost]
        public IActionResult userEdit(string userName, string userNickName, string userPassword, string userAdress, string userPhone, string userExperience, string userProfession, string userId, DateTime userBirthday, string userSax)
        {
            var userAccount = HttpContext.Session.GetString("userName");
            var newUserInfoUpdate = _context.UserInfos.Find(HttpContext.Request.Query["u"].ToString());
            newUserInfoUpdate.UserName = userName;
            newUserInfoUpdate.UserNickname = userNickName;
            newUserInfoUpdate.UserPassword = userPassword;
            newUserInfoUpdate.UserSax = userSax;
            newUserInfoUpdate.UserAddress = userAdress;
            newUserInfoUpdate.UserBirthday = userBirthday;
            newUserInfoUpdate.UserPhone = userPhone;
            newUserInfoUpdate.UserExperience = userExperience;
            newUserInfoUpdate.UserProfession = userProfession;
            newUserInfoUpdate.LastupdateUser = userAccount;
            newUserInfoUpdate.LastupdateDate = DateTime.Now;
            newUserInfoUpdate.Version++;
            _context.SaveChanges();
            return Redirect($"/user/user?account={userAccount}");
        }

        [HttpPost]
        public ActionResult UploadImage(IFormFile File)
        {
            //取得帳號和會員ID
            string userName = HttpContext.Session.GetString("userName");
            string userId = (from u in _context.UserInfos
                             where u.UserAccount == userName
                             select u.UserId).First();

            if (File.Length > 0)
            {
                //將圖片轉成Byte
                using (var ms = new MemoryStream())
                {
                    File.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    //確認會員是否上傳過圖片
                    int check = (from image in _context.UserImages
                                 where image.UserId == userId
                                 select image).Count();



                    //沒上傳過-->新增資料
                    if (check == 0)
                    {
                        var imageAdd = new UserImage()
                        {
                            UserId = userId,
                            Uimage = fileBytes,
                            CreateUser = userName,
                            LastupdateUser = userName,
                            CreateDate = DateTime.Now,
                            LastupdateDate = DateTime.Now
                        };

                        _context.UserImages.Add(imageAdd);
                        _context.SaveChanges();

                    }
                    //有新增過-->更新資料
                    else
                    {
                        var imageID = (from image in _context.UserImages
                                       where image.UserId == userId
                                       select image.ImageId).First();

                        var imageUpdate = _context.UserImages.Find(imageID);
                        imageUpdate.Uimage = fileBytes;
                        imageUpdate.LastupdateUser = userName;
                        imageUpdate.LastupdateDate = DateTime.Now;
                        imageUpdate.Version++;

                        _context.SaveChanges();

                    }
                }
            }

            return Redirect($"/User/User?account={userName}");
        }

        //取得使用者圖片
        public byte[] getUserImg(string uid, string usex)
        {
            int chk = (from i in _context.UserImages
                       where i.UserId == uid
                       select i).Count();

            //如使用者沒有設定圖片
            if (chk == 0)
            {
                string sax = usex;

                //則依照性別顯示系統設定圖片
                var sysImageQuery = (from img in _context.UserImages
                                     where img.UserId == "SYS"
                                     where img.ImageId == "Defult" + sax
                                     select img.Uimage).First();

                return (sysImageQuery);

            }
            //顯示使用者設定圖片
            else
            {
                var userImageQuery = (from img in _context.UserImages
                                      where img.UserId == uid
                                      select img.Uimage).First();

                return (userImageQuery);
            }

        }

    }

}

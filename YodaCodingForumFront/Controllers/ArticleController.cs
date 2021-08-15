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
using YodaCodingForumFront.Services;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace YodaCodingForumFront.Controllers
{
    public class ArticleController : Controller
    {
        private IMailService _imailservice;
        private IConfiguration _configuration;
        private readonly ArticleDBContext _context;

        public ArticleController(ArticleDBContext context, IMailService imailservice, IConfiguration iconfiguration)
        {
            _context = context;
            _imailservice = imailservice;
            _configuration = iconfiguration;
        }
        public IActionResult Index()
        {

            return View();
        }




        //登入帳號判斷
        public IActionResult checkLogin(string userName, string userPassword, string clickArticle)
        {

            if (!(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(userPassword)))
            {
                var queryList = (from u in _context.UserInfos
                                 where u.UserAccount == userName
                                 where u.UserPassword == userPassword
                                 select u).ToList();
                int s = queryList.Count;

                if (s == 1 && clickArticle == "article")
                {
                    HttpContext.Session.SetString("userName", userName);
                    HttpContext.Session.SetString("password", userPassword);
                    return Content("articleSuccess");
                }
                else if (s == 1 && clickArticle == "question")
                {
                    HttpContext.Session.SetString("userName", userName);
                    HttpContext.Session.SetString("password", userPassword);
                    return Content("questionSuccess");
                }
                else if (s == 1)
                {
                    if (queryList[0].UserStatus == "N")
                    {
                        return Content("unverified");
                    }

                    if (queryList[0].UserStatus == "F")
                    {
                        return Content("block");
                    }
                    HttpContext.Session.SetString("userName", userName);
                    HttpContext.Session.SetString("password", userPassword);
                    return Content("Success");
                }
                else
                {
                    return Content("False");
                }
            }
            else
            {
                return Content("empty");
            }

        }
        //登出
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("userName");
            HttpContext.Session.Remove("password");
            return Redirect("/article/articlewall");

        }
        //註冊會員頁面
        public ActionResult RegisterMember()
        {
            return View();
        }
        //註冊會員判斷
        public async Task<IActionResult> Register(string account, string password, string username, string nickname, string gender, string email)
        {

            Regex regexAccount = new Regex(@"^[a-zA-Z0-9]{6,20}$");
            Regex regexPassword = new Regex(@"^(?!.*[^\x21-\x7e])(?=.{6,20})(?=.*[a-zA-Z])(?=.*\d).*$");
            checkAccountRepeated newAccount = new checkAccountRepeated()
            {
                Account = ((from u in _context.UserInfos
                            where u.UserAccount == account
                            select u).ToList()).Count(),
                Nickname = ((from u in _context.UserInfos
                             where u.UserNickname == nickname
                             select u).ToList()).Count(),
                Email = ((from u in _context.UserInfos
                          where u.UserEmail == email
                          select u).ToList()).Count()
            };
            if (String.IsNullOrEmpty(account) || !(regexAccount.IsMatch(account)) || newAccount.Account == 1)
            {
                if (newAccount.Account == 1)
                {
                    return Content("AccountRepeated");
                }
                else
                {
                    return Content("AccountError");
                }

            }
            if (String.IsNullOrEmpty(password) || !(regexPassword.IsMatch(password)))
            {
                return Content("PasswordError");
            }

            if (String.IsNullOrEmpty(username))
            {
                return Content("UsernameError");
            }

            if (newAccount.Nickname == 1)
            {
                return Content("NicknameRepeated");

            }

            if (String.IsNullOrEmpty(email) || !(IsValid(email)) || newAccount.Email == 1)
            {
                if (newAccount.Email == 1)
                {
                    return Content("EmailRepeated");
                }
                else
                {
                    return Content("EmailError");
                }
            }



            //新增使用者
            UserInfo newMember = new UserInfo
            {
                UserAccount = account,
                UserPassword = password,
                UserName = username,
                UserNickname = nickname,
                UserSax = gender,
                UserEmail = email,
                UserStatus = "N",
                CreateUser = account,
                CreateDate = DateTime.Now,
                LastupdateUser = account,
                LastupdateDate = DateTime.Now
            };
            _context.UserInfos.Add(newMember);
            _context.SaveChanges();
            //找userID
            var newUserIdList = (from u in _context.UserInfos
                                 where u.UserAccount == account
                                 select u.UserId).ToList();
            //新增分數資料庫
            UserPoint newUserPoint = new UserPoint
            {
                UserId = newUserIdList[0],
                CreateUser = account,
                CreateDate = DateTime.Now,
                LastupdateUser = account,
                LastupdateDate = DateTime.Now
            };
            _context.UserPoints.Add(newUserPoint);
            _context.SaveChanges();

            var accountEncode = Encoding.UTF8.GetBytes(account);
            var validAccountEncode = WebEncoders.Base64UrlEncode(accountEncode);

            string url = $"{_configuration["AppUrl"]}/article/confrimEmail?id={account}&token={validAccountEncode}";
            await _imailservice.SendEmailAsync(email, "YodaCoding", "<h1>Welcome to Yoda Coding!</h1>" +
                $"<p>Plese confrim your account by <a href='{url}'>Click Me</a>.</p>");

            return Content("Success");


        }
        //註冊會員確認網址
        public ActionResult confrimEmail()
        {
            string account = HttpContext.Request.Query["id"].ToString();
            string token = HttpContext.Request.Query["token"].ToString();
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }
            var decodeToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodeToken);
            var userID = (from u in _context.UserInfos
                          where u.UserAccount == normalToken
                          select u.UserId).ToList()[0];
            var changeAccountStatus = _context.UserInfos.Find(userID);
            changeAccountStatus.UserStatus = "T";
            changeAccountStatus.LastupdateDate = DateTime.Now;
            changeAccountStatus.Version++;
            _context.SaveChanges();
            return View();
        }
        //文章列表
        public IActionResult articleWall()
        {

            //文章或問答類型
            var typeQuery = from a in _context.Articles
                            select a.ArticleType;

            //文章query
            var articleQuery = from a in _context.Articles
                               where a.ArticleType == "A"
                               where a.ArticleStatus == "N"
                               orderby a.CreateDate descending
                               select new indexArticleList
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
                               };

            var hotList = (from a in _context.Articles
                           join atag in _context.ArticleAndTags on a.ArticleId equals atag.ArticleId
                           where a.ArticleStatus != "D"
                           select new hotTagList
                           {
                               TagID = atag.TagId,
                               Tagname = (from t in _context.Tags
                                          where t.TagId == atag.TagId
                                          select t.TagName).First(),
                               TagUseCount = (from a in _context.Articles
                                              join atag2 in _context.ArticleAndTags on a.ArticleId equals atag2.ArticleId
                                              where a.ArticleStatus != "D"
                                              where atag2.TagId == atag.TagId
                                              select atag2.TagId).Count()
                           }).Distinct().OrderByDescending(order => order.TagUseCount).Take(20).ToList();

            //小測驗
            var questionQuerytitel = (from q in _context.Questions
                                      select new questionAndQptions
                                      {
                                          QuestionName = q.QuestionName,
                                          QuestionId = q.QuestionId
                                      }).ToList();

            var count = questionQuerytitel.Count();

            Random rnd = new Random();
            int n = rnd.Next(count);
            var questionQuerytitel1 = questionQuerytitel[n];
            var questionOptionlist = (from qo in _context.QuestionOptions
                                      where qo.QuestionId == questionQuerytitel1.QuestionId
                                      select new questionAndQptions
                                      {
                                          QuestionOption = qo.QoptionName,
                                          IscorrectAnswer = qo.IscorrectAnswer,
                                          QuestionId = qo.QoptionId
                                      }).ToList();

            var articleTypeVM = new ArticleGenreViewMode
            {
                Types = new SelectList(typeQuery.Distinct().ToList()),
                ArticleList = articleQuery.ToList(),
                //HotAritcle
                hotArticleList = (from hota in _context.Articles
                                  where hota.ArticleType == "A"
                                  where hota.ArticleStatus == "N"
                                  orderby hota.ArticleViews descending
                                  select new hotList
                                  {
                                      itemID = hota.ArticleId,
                                      itemTitle = hota.ArticleTitle
                                  }).Take(5).ToList(),
                //HotQuestion
                hotQuestionList = (from hotq in _context.Articles
                                   where hotq.ArticleType == "Q"
                                   where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                   orderby hotq.ArticleViews descending
                                   select new hotList
                                   {
                                       itemID = hotq.ArticleId,
                                       itemTitle = hotq.ArticleTitle
                                   }).Take(5).ToList(),
                hotTagList = hotList,
                questionList = questionOptionlist,
                questiontitel = questionQuerytitel1.QuestionName,
            };

            return View(articleTypeVM);
        }

        //發表文章
        public IActionResult articlepage()
        {
            if (HttpContext.Session.GetString("userName") == null)
            {
                return Redirect("/article/articlewall");

            }
            else
            {
                var tagList = (from t in _context.Tags
                               select t.TagName).ToList();
                return View(tagList);
            }
        }
        //接收發表文章資料
        [HttpPost]
        public ActionResult articlepage(string articleTitle, string textNoneMD, string[] eachTagName)
        {
            var currentTime = DateTime.Now;
            //Article
            Article newArticle = new Article
            {
                ArticleType = "A",
                ArticleContent = textNoneMD,
                ArticleViews = 0,
                ArticleLike = 0,
                ArticleStatus = "N",
                CreateUser = HttpContext.Session.GetString("userName"),
                LastupdateUser = HttpContext.Session.GetString("userName"),
                CreateDate = currentTime,
                LastupdateDate = currentTime,
                ArticleTitle = articleTitle
            };
            _context.Articles.Add(newArticle);
            _context.SaveChanges();

            //文章ID
            var articleID = (from a in _context.Articles
                             where a.CreateDate == currentTime
                             where a.CreateUser == HttpContext.Session.GetString("userName")
                             select a.ArticleId).First();

            string[] tagArray = eachTagName;
            List<string> tagList = new List<string>();
            foreach (var tag in tagArray)
            {
                tagList.Add(tag);
            }
            var tagListDistinct = tagList.Distinct();
            tagList = tagListDistinct.ToList();

            foreach (var tag in tagList)
            {
                var tagHave = (from t in _context.Tags
                               where t.TagName == tag
                               select t.TagName).Count();
                if (tagHave==1)
                {
                    var tagID = (from t in _context.Tags
                                 where t.TagName == tag
                                 select t.TagId).First();
                    ArticleAndTag articleAndTag = new ArticleAndTag
                    {
                        ArticleId = articleID,
                        TagId = tagID,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = currentTime,
                        LastupdateDate = currentTime
                    };
                    _context.ArticleAndTags.Add(articleAndTag);
                }
                else
                {
                    Tag newtag = new Tag
                    {
                        TagName = tag,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = currentTime,
                        LastupdateDate = currentTime
                    };
                    _context.Tags.Add(newtag);
                    _context.SaveChanges();
                    var tagID = (from t in _context.Tags
                                 where t.TagName == tag
                                 select t.TagId).First();
                    ArticleAndTag articleAndTag = new ArticleAndTag
                    {
                        ArticleId = articleID,
                        TagId = tagID,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = currentTime,
                        LastupdateDate = currentTime
                    };
                    _context.ArticleAndTags.Add(articleAndTag);
                }
            }


            _context.SaveChanges();
            return Redirect($"/article/article?a={articleID}");
        }
        //文章顯示頁面
        public IActionResult article()
        {
            string userAccount = HttpContext.Session.GetString("userName");
            string uid = "";
            if (userAccount != null)
            {
                uid = (from u in _context.UserInfos
                       where u.UserAccount == userAccount
                       select u.UserId).ToList()[0];

                var userSex = (from u in _context.UserInfos
                               where u.UserAccount == userAccount
                               select u.UserSax).ToList()[0];
                ViewBag.userImage = (from i in _context.UserImages
                                     where i.UserId == uid
                                     select i.UserId).Count() == 1 ?
                                     (from i in _context.UserImages
                                      where i.UserId == uid
                                      select i.Uimage).First() :
                                      (from i in _context.UserImages
                                       where i.ImageId == "Defult" + userSex
                                       select i.Uimage).First();
            }

            string id = HttpContext.Request.Query["a"].ToString();

            var plusArticleViews = _context.Articles.Find(id);
            plusArticleViews.ArticleViews++;
            _context.SaveChanges();


            //小測驗
            var questionQuerytitel = (from q in _context.Questions
                                      select new questionAndQptions
                                      {
                                          QuestionName = q.QuestionName,
                                          QuestionId = q.QuestionId
                                      }).ToList();

            var count = questionQuerytitel.Count();

            Random rnd = new Random();
            int n = rnd.Next(count);
            var questionQuerytitel1 = questionQuerytitel[n];
            var questionOptionlist = (from qo in _context.QuestionOptions
                                      where qo.QuestionId == questionQuerytitel1.QuestionId
                                      select new questionAndQptions
                                      {
                                          QuestionOption = qo.QoptionName,
                                          IscorrectAnswer = qo.IscorrectAnswer,
                                          QuestionId = qo.QoptionId
                                      }).ToList();


            var articleQuery = from a in _context.Articles
                               join ua in _context.UserInfos on a.CreateUser equals ua.UserAccount
                               where a.ArticleId == id
                               where a.ArticleType == "A"
                               select new articleDetail
                               {
                                   //ArticleDetail
                                   ArticleId = id,
                                   ArticleTitle = a.ArticleTitle,
                                   ArticleContent = a.ArticleContent,
                                   ArticleViews = a.ArticleViews,
                                   ArticleLike = a.ArticleLike,
                                   ArticleStatus = a.ArticleStatus,
                                   Nickname = ua.UserNickname,
                                   ArticleVersion = a.Version,
                                   CreateUser = a.CreateUser,
                                   CreateDate = a.CreateDate,
                                   LastupdateUser = a.LastupdateUser,
                                   LastupdateDate = a.LastupdateDate,
                                   ArticleCollections = new List<int>() {
                                                    (from ul in _context.UserLikes
                                                        where ul.ArticleId == id
                                                        where ul.UserId == uid
                                                        where ul.LikeStatus == "T"
                                                        select ul).Count(),
                                                       (from ul in _context.UserCollects
                                                        where ul.ArticleId == id
                                                        where ul.UserId == uid
                                                        where ul.CollectStatus == "T"
                                                        select ul).Count(),
                                                       (from ul in _context.UserFollows
                                                        where ul.ArticleId == id
                                                        where ul.UserId == uid
                                                        where ul.FollowStatus == "T"
                                                        select ul).Count()
                                                   },
                                   ArticleImage = (from i in _context.UserImages
                                                   where i.UserId == ua.UserId
                                                   select i.UserId).Count() == 1 ?
                                                  (from i in _context.UserImages
                                                   where i.UserId == ua.UserId
                                                   select i.Uimage).First() :
                                                  (from i in _context.UserImages
                                                   where i.ImageId == "Defult" + ua.UserSax
                                                   select i.Uimage).First(),
                                   //ArticleTag
                                   ArticleTagList = (from t in _context.Tags
                                                     join tAnda in _context.ArticleAndTags on t.TagId equals tAnda.TagId
                                                     where tAnda.ArticleId == a.ArticleId
                                                     orderby t.CreateDate descending
                                                     select t).ToList(),
                                   //ArticleResponseCount
                                   ArticleResponseCount = (from r in _context.Responses
                                                           where r.ParentId == a.ArticleId
                                                           where r.ResponseStatus != "D"
                                                           select r).Count(),
                                   //ArticleResponse
                                   ArticleResponseList = (from r in _context.Responses
                                                          where r.ParentId == a.ArticleId
                                                          where r.ResponseStatus != "D"
                                                          orderby r.CreateDate
                                                          join u in _context.UserInfos on r.CreateUser equals u.UserAccount
                                                          select new Response_class
                                                          {
                                                              ResponseAccount = r.CreateUser,
                                                              ResponseContent = r.ResponseContent,
                                                              ResponseID = r.ResponseId,
                                                              ResponseNickname = u.UserNickname,
                                                              ResponseParentID = r.ParentId,
                                                              ResponseStatus = r.ResponseStatus,
                                                              CreateDate = r.CreateDate,
                                                              ResponseParentClass = r.ParentClass,
                                                              ResponseImage = (from i in _context.UserImages
                                                                               where i.UserId == u.UserId
                                                                               select i.UserId).Count() == 1 ?
                                                                              (from i in _context.UserImages
                                                                               where i.UserId == u.UserId
                                                                               select i.Uimage).First() :
                                                                               (from i in _context.UserImages
                                                                                where i.ImageId == "Defult" + u.UserSax
                                                                                select i.Uimage).First(),
                                                          }).ToList(),

                                   //ArticleComment
                                   ArticleCommentList = (from c in _context.Comments
                                                         where c.ArticleId == a.ArticleId
                                                         where c.CommentStatus != "D"
                                                         join u in _context.UserInfos on c.CreateUser equals u.UserAccount
                                                         orderby c.CreateDate
                                                         select new ArticleComment
                                                         {
                                                             CommentAccount = c.CreateUser,
                                                             CreateDate = c.CreateDate,
                                                             CommentId = c.CommentId,
                                                             CommentVersion = c.Version,
                                                             LastUpdateDate = c.LastupdateDate,
                                                             CommentContent = c.CommentContent,
                                                             CommentStatus = c.CommentStatus,
                                                             CommentLike = c.CommentLike,
                                                             CommentNickname = u.UserNickname,
                                                             CommentCollections = (from ul in _context.UserLikeComments
                                                                                   where ul.CommentId == c.CommentId
                                                                                   where ul.UserId == uid
                                                                                   where ul.LikecommentStatus == "T"
                                                                                   select ul).Count(),
                                                             //ArticleCommentResponseCount
                                                             CommentResponseCount = (from r in _context.Responses
                                                                                     where r.ParentId == c.CommentId
                                                                                     where r.ResponseStatus != "D"
                                                                                     select r).Count(),
                                                             CommentImage = (from i in _context.UserImages
                                                                             where i.UserId == u.UserId
                                                                             select i.UserId).Count() == 1 ?
                                                                             (from i in _context.UserImages
                                                                              where i.UserId == u.UserId
                                                                              select i.Uimage).First() :
                                                                              (from i in _context.UserImages
                                                                               where i.ImageId == "Defult" + u.UserSax
                                                                               select i.Uimage).First(),
                                                             //ArticleCommentResponse
                                                             CommentResponseList = (from r in _context.Responses
                                                                                    where r.ParentId == c.CommentId
                                                                                    where r.ResponseStatus != "D"
                                                                                    orderby r.CreateDate
                                                                                    join u in _context.UserInfos on r.CreateUser equals u.UserAccount
                                                                                    select new Response_class
                                                                                    {
                                                                                        ResponseAccount = r.CreateUser,
                                                                                        ResponseContent = r.ResponseContent,
                                                                                        ResponseID = r.ResponseId,
                                                                                        ResponseNickname = u.UserNickname,
                                                                                        ResponseParentID = r.ParentId,
                                                                                        ResponseStatus = r.ResponseStatus,
                                                                                        CreateDate = r.CreateDate,
                                                                                        ResponseParentClass = r.ParentClass,
                                                                                        ResponseImage = (from i in _context.UserImages
                                                                                                         where i.UserId == u.UserId
                                                                                                         select i.UserId).Count() == 1 ?
                                                                                                        (from i in _context.UserImages
                                                                                                         where i.UserId == u.UserId
                                                                                                         select i.Uimage).First() :
                                                                                                         (from i in _context.UserImages
                                                                                                          where i.ImageId == "Defult" + u.UserSax
                                                                                                          select i.Uimage).First(),
                                                                                    }).ToList(),
                                                         }).ToList(),
                                   //HotAritcle
                                   hotArticleList = (from hota in _context.Articles
                                                     where hota.ArticleType == "A"
                                                     where hota.ArticleStatus == "N"
                                                     orderby hota.ArticleViews descending
                                                     select new hotList
                                                     {
                                                         itemID = hota.ArticleId,
                                                         itemTitle = hota.ArticleTitle
                                                     }).Take(5).ToList(),
                                   //HotQuestion
                                   hotQuestionList = (from hotq in _context.Articles
                                                      where hotq.ArticleType == "Q"
                                                      where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                                      orderby hotq.ArticleViews descending
                                                      select new hotList
                                                      {
                                                          itemID = hotq.ArticleId,
                                                          itemTitle = hotq.ArticleTitle
                                                      }).Take(5).ToList(),
                                   questionList = questionOptionlist,
                                   questiontitel = questionQuerytitel1.QuestionName
                               };





            //ArticleToModel
            var article = articleQuery.ToList().First();

            //熱門Tag 

            var hotTagList = (from a in _context.Articles
                              join atag in _context.ArticleAndTags on a.ArticleId equals atag.ArticleId
                              where a.ArticleStatus != "D"
                              select new hotTagList
                              {
                                  TagID = atag.TagId,
                                  Tagname = (from t in _context.Tags
                                             where t.TagId == atag.TagId
                                             select t.TagName).First(),
                                  TagUseCount = (from a in _context.Articles
                                                 join atag2 in _context.ArticleAndTags on a.ArticleId equals atag2.ArticleId
                                                 where a.ArticleStatus != "D"
                                                 where atag2.TagId == atag.TagId
                                                 select atag2.TagId).Count()
                              }).Distinct().OrderByDescending(order => order.TagUseCount).Take(20).ToList();
            ViewBag.hotagList = hotTagList;

            //RelatedArticle
            //查詢所有文章與其Tag名稱
            var tagQuery = from t in _context.Tags
                           join at in _context.ArticleAndTags on t.TagId equals at.TagId
                           join a in _context.Articles on at.ArticleId equals a.ArticleId
                           where a.ArticleStatus == "N"
                           where a.ArticleType == "A"
                           orderby a.ArticleLike descending
                           select new relatedArticle
                           {
                               ArticleId = a.ArticleId,
                               TagName = t.TagName

                           };

            //建立清單存放相關文章ID
            List<string> relatedArticleIDList = new List<string>();
            //建立清單存放相關文章ID及文章標題
            List<relatedArticle> relatedArticleList = new List<relatedArticle>();



            //將與本篇文章TagName一樣的放進相關文章裡
            foreach (var tagName in article.ArticleTagList)
            {
                var eachtagQuery = tagQuery.Where(tag => tag.TagName.Contains(tagName.TagName));

                foreach (var relatedarticle in eachtagQuery.ToList())
                {
                    relatedArticleIDList.Add(relatedarticle.ArticleId);
                }
            }
            var relatedArticleDistinct = relatedArticleIDList.Distinct();
            var relatedArticleDistinctList = relatedArticleDistinct.ToList();
            relatedArticleDistinctList.Remove(article.ArticleId);

            if (relatedArticleDistinctList.Count > 0)
            {
                foreach (var relatedarticle in relatedArticleDistinctList)
                {
                    var relatedArticleQuery = from a in _context.Articles
                                              where a.ArticleId == relatedarticle
                                              select new relatedArticle
                                              {
                                                  ArticleId = a.ArticleId,
                                                  TagName = a.ArticleTitle
                                              };
                    relatedArticleList.Add(relatedArticleQuery.ToList()[0]);
                }
                ViewBag.relatedArticleList = relatedArticleList;

            }
            else
            {
                ViewBag.relatedArticleList = null;
            }

            

            return View(article);
        }

        //文章編輯頁面
        public IActionResult articleEdit()
        {
            var articleID = HttpContext.Request.Query["a"].ToString();
            var articleEditDetail = (from a in _context.Articles
                                    where a.ArticleId == articleID
                                    select new ArticleEditDetail
                                    {
                                        ArticleId = a.ArticleId,
                                        ArticleContent = a.ArticleContent,
                                        ArticleTitle = a.ArticleTitle,
                                        ArticleAccount = a.CreateUser,
                                        tagList = (from t in _context.Tags
                                                   select t.TagName).ToList(),
                                        TagNum = (from at in _context.ArticleAndTags
                                                 where at.ArticleId == a.ArticleId
                                                 select at.TagId).Count(),
                                        ArticleTagList = (from t in _context.ArticleAndTags
                                                          where t.ArticleId == articleID
                                                          join tname in _context.Tags on t.TagId equals tname.TagId
                                                          select tname.TagName).ToList()
                                    }).First();

            if (articleEditDetail.ArticleAccount == HttpContext.Session.GetString("userName"))
            {
                
                return View(articleEditDetail);
            }
            else
            {
                return Redirect("/article/articlewall");
            }
        }
        //接收文章更新資料
        [HttpPost]
        public IActionResult articleEdit(string textNoneMD, string articleTitle, string[] eachTagName)
        {
            string articleID = HttpContext.Request.Query["a"].ToString();
            var articleConntent = _context.Articles.Find(articleID);
            articleConntent.ArticleContent = textNoneMD;
            articleConntent.ArticleTitle = articleTitle;
            articleConntent.LastupdateUser = HttpContext.Session.GetString("userName");
            articleConntent.LastupdateDate = DateTime.Now;
            articleConntent.Version++;

            //清除所有tag
            var articleAndTagList = (from at in _context.ArticleAndTags
                                 where at.ArticleId == articleID
                                 select at).ToList();
            foreach (var articleAndTag in articleAndTagList)
            {
                _context.ArticleAndTags.Remove(articleAndTag);
            }
            _context.SaveChanges();

            //Tag
            string[] tagArray = eachTagName;
            List<string> tagList = new List<string>();
            foreach (var tag in tagArray)
            {
                tagList.Add(tag);
            }
            var tagListDistinct = tagList.Distinct();
            tagList = tagListDistinct.ToList();

            foreach (var tag in tagList)
            {
                var tagHave = (from t in _context.Tags
                               where t.TagName == tag
                               select t.TagName).Count();
                if (tagHave == 1)
                {
                    var tagID = (from t in _context.Tags
                                 where t.TagName == tag
                                 select t.TagId).First();
                    ArticleAndTag articleAndTag = new ArticleAndTag
                    {
                        ArticleId = articleID,
                        TagId = tagID,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = DateTime.Now,
                        LastupdateDate = DateTime.Now
                    };
                    _context.ArticleAndTags.Add(articleAndTag);
                }
                else
                {
                    Tag newtag = new Tag
                    {
                        TagName = tag,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = DateTime.Now,
                        LastupdateDate = DateTime.Now
                    };
                    _context.Tags.Add(newtag);
                    _context.SaveChanges();
                    var tagID = (from t in _context.Tags
                                 where t.TagName == tag
                                 select t.TagId).First();
                    ArticleAndTag articleAndTag = new ArticleAndTag
                    {
                        ArticleId = articleID,
                        TagId = tagID,
                        CreateUser = HttpContext.Session.GetString("userName"),
                        LastupdateUser = HttpContext.Session.GetString("userName"),
                        CreateDate = DateTime.Now,
                        LastupdateDate = DateTime.Now
                    };
                    _context.ArticleAndTags.Add(articleAndTag);
                }
            }
            _context.SaveChanges();
            return Redirect($"/Article/article?a={articleID}");
        }

        //留言編輯頁面
        public IActionResult commentEdit()
        {
            var commentQuery = from c in _context.Comments
                               where c.CommentId == HttpContext.Request.Query["c"].ToString()
                               join a in _context.Articles on c.ArticleId equals a.ArticleId
                               select new commemtDelete
                               {
                                   ArticleTitle = a.ArticleTitle,
                                   ArticleId = a.ArticleId,
                                   Commemt = c
                               };
            var commentList = commentQuery.ToList();
            var comment = new commemtDelete();
            foreach (var item in commentList)
            {
                comment = item;
            }


            if (comment.Commemt.CreateUser == HttpContext.Session.GetString("userName"))
            {
                return View(comment);
            }
            else
            {
                return Redirect("/article/articlewall");
            }
        }
        //接收留言更新資料
        [HttpPost]
        public IActionResult commentEdit(string textNoneMD, string articleId)
        {
            var newCommemtUpdate = _context.Comments.Find(HttpContext.Request.Query["c"].ToString());
            newCommemtUpdate.CommentContent = textNoneMD;
            newCommemtUpdate.LastupdateUser = HttpContext.Session.GetString("userName");
            newCommemtUpdate.LastupdateDate = DateTime.Now;
            newCommemtUpdate.Version++;
            _context.SaveChanges();
            return Redirect($"/article/article?a={articleId}");
        }
        //文章刪除
        public IActionResult articleDelete(string id)
        {
            var deleteArticle = _context.Articles.Find(id);
            deleteArticle.ArticleStatus = "D";
            deleteArticle.LastupdateUser = HttpContext.Session.GetString("userName");
            deleteArticle.Version++;

            var articleAccountList = (from a in _context.Articles
                                      where a.ArticleId == id
                                      select a.CreateUser).ToList();
            var articleAccount = articleAccountList[0];
            string articleAuthor = HttpContext.Session.GetString("userName") ?? "noAuthor";
            if (articleAccount == articleAuthor)
            {
                _context.SaveChanges();
                return Redirect("/article/articlewall");
            }
            else
            {
                return Redirect("/article/articlewall");
            }

        }
        //留言刪除
        public IActionResult commentDelete(string id, string aid)
        {
            var deletecomment = _context.Comments.Find(id);
            deletecomment.CommentStatus = "D";
            deletecomment.LastupdateUser = HttpContext.Session.GetString("userName");
            deletecomment.Version++;

            var commentAccountList = (from a in _context.Comments
                                      where a.CommentId == id
                                      select a.CreateUser).ToList();
            var commentAccount = commentAccountList[0];
            string commentAuthor = HttpContext.Session.GetString("userName") ?? "noAuthor";
            if (commentAccount == commentAuthor)
            {
                _context.SaveChanges();
                return Redirect($"/article/article?a={aid}");
            }
            else
            {
                return Redirect("/article/articlewall");
            }

        }
        //文章回覆刪除
        public IActionResult a_responseDelete(string id, string aid)
        {
            var deleteresponse = _context.Responses.Find(id);
            deleteresponse.ResponseStatus = "D";
            deleteresponse.LastupdateUser = HttpContext.Session.GetString("userName");
            deleteresponse.Version++;

            var responseAccountList = (from a in _context.Responses
                                       where a.ResponseId == id
                                       select a.CreateUser).ToList();
            var responseAccount = responseAccountList[0];
            string responseAuthor = HttpContext.Session.GetString("userName") ?? "noAuthor";
            if (responseAccount == responseAuthor)
            {
                _context.SaveChanges();
                return Redirect($"/article/article?a={aid}");
            }
            else
            {
                return Redirect("/article/articlewall");
            }
        }
        //留言回覆刪除
        public IActionResult c_responseDelete(string id, string aid)
        {
            var deleteresponse = _context.Responses.Find(id);
            deleteresponse.ResponseStatus = "D";
            deleteresponse.LastupdateUser = HttpContext.Session.GetString("userName");
            deleteresponse.Version++;

            var responseAccountList = (from a in _context.Responses
                                       where a.ResponseId == id
                                       select a.CreateUser).ToList();
            var responseAccount = responseAccountList[0];
            string responseAuthor = HttpContext.Session.GetString("userName") ?? "noAuthor";
            if (responseAccount == responseAuthor)
            {
                _context.SaveChanges();
                return Redirect($"/article/article?a={aid}");
            }
            else
            {
                return Redirect("/article/articlewall");
            }
        }

        //留言回覆接收資料判斷
        public ActionResult postDirect(string postUserName, string postCotent, string postID, string postType, string commentId)
        {
            if (postUserName == "Guest")
            {
                return Content("pleaseLogin");
            }
            else
            {
                if (!String.IsNullOrEmpty(postCotent))
                {
                    switch (postType)
                    {
                        case "ar":
                            Response newArticleResponse = new Response
                            {
                                ParentId = postID,
                                ParentClass = "A",
                                ResponseContent = postCotent,
                                CreateUser = postUserName,
                                CreateDate = DateTime.Now,
                                LastupdateUser = postUserName,
                                LastupdateDate = DateTime.Now
                            };
                            _context.Responses.Add(newArticleResponse);
                            _context.SaveChanges();
                            return Content("arSuccess");
                        case "ac":
                            Comment newArticleComment = new Comment
                            {
                                ArticleId = postID,
                                CommentClass = "C",
                                CommentContent = postCotent,
                                CreateUser = postUserName,
                                CreateDate = DateTime.Now,
                                LastupdateUser = postUserName,
                                LastupdateDate = DateTime.Now

                            };
                            _context.Comments.Add(newArticleComment);
                            _context.SaveChanges();
                            return Content("acSuccess");
                        case "cr":
                            Response newCommentResponse = new Response
                            {
                                ParentId = commentId,
                                ParentClass = "C",
                                ResponseContent = postCotent,
                                CreateUser = postUserName,
                                CreateDate = DateTime.Now,
                                LastupdateUser = postUserName,
                                LastupdateDate = DateTime.Now
                            };
                            _context.Responses.Add(newCommentResponse);
                            _context.SaveChanges();
                            return Content("crSuccess");
                        default:
                            return Content("False");
                    }
                }
                else
                {
                    switch (postType)
                    {
                        case "ar":
                            return Content("arFalse");
                        case "ac":
                            return Content("acFalse");
                        case "cr":
                            return Content("crFalse");
                        default:
                            return Content("False");
                    }
                }
            }
        }

        //搜尋頁面
        public IActionResult Search(string type, string searchString)
        {

            //小測驗
            var questionQuerytitel = (from q in _context.Questions
                                      select new questionAndQptions
                                      {
                                          QuestionName = q.QuestionName,
                                          QuestionId = q.QuestionId
                                      }).ToList();

            var count = questionQuerytitel.Count();

            Random rnd = new Random();
            int n = rnd.Next(count);
            var questionQuerytitel1 = questionQuerytitel[n];
            var questionOptionlist = (from qo in _context.QuestionOptions
                                      where qo.QuestionId == questionQuerytitel1.QuestionId
                                      select new questionAndQptions
                                      {
                                          QuestionOption = qo.QoptionName,
                                          IscorrectAnswer = qo.IscorrectAnswer,
                                          QuestionId = qo.QoptionId
                                      }).ToList();

            //搜尋
            var typeQuery = from a in _context.Articles
                            select a.ArticleType;

            //搜尋類別為Tag:名稱包含查詢條件的文章/問答。
            if (type == "Tag")
            {

                var articleQuery = from a in _context.Articles
                                   where a.ArticleStatus != "D"
                                   orderby a.ArticleLike descending
                                   select new indexArticleList
                                   {
                                       aid = a.ArticleId,
                                       Title = a.ArticleTitle,
                                       Views = a.ArticleViews,
                                       Like = a.ArticleLike,
                                       Type = a.ArticleType,
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
                //Tag與文章關聯query
                var tagquery = from t in _context.Tags
                               join ta in _context.ArticleAndTags on t.TagId equals ta.TagId
                               select new articleAndTagForSearch
                               {
                                   articleId = ta.ArticleId,
                                   tagName = t.TagName
                               };

                //增加where條件:tagName符合查詢內容
                if (!String.IsNullOrEmpty(searchString))
                {
                    tagquery = tagquery.Where(t => t.tagName.Contains(searchString));
                }

                //將tagquery的articleId取出變成一個陣列,並去除重複資料
                List<string> aid = new List<string>();
                foreach (var a in tagquery)
                {
                    aid.Add(a.articleId);
                }
                aid.Distinct();

                //若articleQuery內的aid存在,存入一個新的陣列
                List<string> alist = new List<string>();
                foreach (var item in articleQuery)
                {
                    if (aid.Contains(item.aid))
                    {
                        alist.Add(item.aid);
                    }
                }

                //articleQuery增加條件:aid存在陣列
                articleQuery = articleQuery.Where(a => alist.Contains(a.aid));

                var articleTypeVM = new ArticleGenreViewMode
                {
                    Types = new SelectList(typeQuery.Distinct().ToList()),
                    ArticleList = articleQuery.ToList(),
                    //HotAritcle
                    hotArticleList = (from hota in _context.Articles
                                      where hota.ArticleType == "A"
                                      where hota.ArticleStatus == "N"
                                      orderby hota.ArticleViews descending
                                      select new hotList
                                      {
                                          itemID = hota.ArticleId,
                                          itemTitle = hota.ArticleTitle
                                      }).Take(5).ToList(),
                    //HotQuestion
                    hotQuestionList = (from hotq in _context.Articles
                                       where hotq.ArticleType == "Q"
                                       where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                       orderby hotq.ArticleViews descending
                                       select new hotList
                                       {
                                           itemID = hotq.ArticleId,
                                           itemTitle = hotq.ArticleTitle
                                       }).Take(5).ToList(),
                    hotTagList = (from ar in _context.Articles
                                  join atag in _context.ArticleAndTags on ar.ArticleId equals atag.ArticleId
                                  where ar.ArticleStatus != "D"
                                  select new hotTagList
                                  {
                                      TagID = atag.TagId,
                                      Tagname = (from t in _context.Tags
                                                 where t.TagId == atag.TagId
                                                 select t.TagName).First(),
                                      TagUseCount = (from a in _context.Articles
                                                     join atag2 in _context.ArticleAndTags on a.ArticleId equals atag2.ArticleId
                                                     where a.ArticleStatus != "D"
                                                     where atag2.TagId == atag.TagId
                                                     select atag2.TagId).Count()
                                  }).Distinct().OrderByDescending(order => order.TagUseCount).Take(20).ToList(),
                    questionList = questionOptionlist,
                    questiontitel = questionQuerytitel1.QuestionName
                };

                return View(articleTypeVM);
            }

            //搜尋類別為文章OR問答
            else
            {
                var articleQuery = from a in _context.Articles
                                   where a.ArticleStatus != "D"
                                   orderby a.ArticleLike descending
                                   select new indexArticleList
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

                //增加查詢條件:符合查詢類型
                if (!String.IsNullOrEmpty(type))
                {
                    articleQuery = articleQuery.Where(t => t.Type == type);
                }

                //增加查詢條件:文章標題包含搜尋內容
                if (!String.IsNullOrEmpty(searchString))
                {
                    articleQuery = articleQuery.Where(t => t.Title.Contains(searchString));
                }

                var articleTypeVM = new ArticleGenreViewMode
                {
                    Types = new SelectList(typeQuery.Distinct().ToList()),
                    ArticleList = articleQuery.ToList(),
                    //HotAritcle
                    hotArticleList = (from hota in _context.Articles
                                      where hota.ArticleType == "A"
                                      where hota.ArticleStatus == "N"
                                      orderby hota.ArticleViews descending
                                      select new hotList
                                      {
                                          itemID = hota.ArticleId,
                                          itemTitle = hota.ArticleTitle
                                      }).Take(5).ToList(),
                    //HotQuestion
                    hotQuestionList = (from hotq in _context.Articles
                                       where hotq.ArticleType == "Q"
                                       where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                       orderby hotq.ArticleViews descending
                                       select new hotList
                                       {
                                           itemID = hotq.ArticleId,
                                           itemTitle = hotq.ArticleTitle
                                       }).Take(5).ToList(),
                    hotTagList = (from ar in _context.Articles
                                  join atag in _context.ArticleAndTags on ar.ArticleId equals atag.ArticleId
                                  where ar.ArticleStatus != "D"
                                  select new hotTagList
                                  {
                                      TagID = atag.TagId,
                                      Tagname = (from t in _context.Tags
                                                 where t.TagId == atag.TagId
                                                 select t.TagName).First(),
                                      TagUseCount = (from a in _context.Articles
                                                     join atag2 in _context.ArticleAndTags on a.ArticleId equals atag2.ArticleId
                                                     where a.ArticleStatus != "D"
                                                     where atag2.TagId == atag.TagId
                                                     select atag2.TagId).Count()
                                  }).Distinct().OrderByDescending(order => order.TagUseCount).Take(20).ToList(),
                    questionList = questionOptionlist,
                    questiontitel = questionQuerytitel1.QuestionName
                };

                return View(articleTypeVM);
            }
        }

        //檢舉
        [HttpPost]
        public IActionResult reportForm(string id, string aid, string reportReason, string reportRemark, string reportContent)
        {
            //判斷文章還是問題
            var articleType = (from a in _context.Articles
                               where a.ArticleId == aid
                               select a.ArticleType).ToList()[0];
            //檢舉者身分
            var userID = (from u in _context.UserInfos
                          where u.UserAccount == HttpContext.Session.GetString("userName")
                          select u.UserId).ToList()[0];
            //判斷主因是否有打
            if (String.IsNullOrEmpty(reportRemark))
            {
                reportRemark = null;
            }
            //判斷是哪一個類型的檢舉
            int ArticleFindCount = (from a in _context.Articles
                                    where a.ArticleId == id
                                    select a).ToList().Count();
            int CommentFindCount = (from a in _context.Comments
                                    where a.CommentId == id
                                    select a).ToList().Count();
            int ResponseFindCount = (from a in _context.Responses
                                     where a.ParentId == id
                                     select a).ToList().Count();
            var ReportTargetType = "";

            if (ArticleFindCount == 1)
            {
                if (articleType == "A")
                {

                    ReportTargetType = "A";
                }
                else
                {
                    ReportTargetType = "Q";
                }
            }
            else if (CommentFindCount == 1)
            {
                ReportTargetType = "C";
            }

            else
            {
                ReportTargetType = "R";
            }


            Report newReport = new Report()
            {
                UserId = userID,
                ReportTargetId = id,
                ReportTargetType = ReportTargetType,
                ReasonCode = reportReason,
                ReportRemarks = reportRemark,
                ReportContents = reportContent,
                CreateUser = HttpContext.Session.GetString("userName"),
                CreateDate = DateTime.Now,
                LastupdateUser = HttpContext.Session.GetString("userName"),
                LastupdateDate = DateTime.Now
            };
            _context.Reports.Add(newReport);
            _context.SaveChanges();

            //判斷文章還是問題回路由
            return Redirect($"/article/reportSuccess/{articleType}/{aid}");
        }
        //檢舉成功頁面
        public IActionResult reportSuccess(string id, string aid)
        {
            List<string> content = new List<string>();
            content.Add(id);
            content.Add(aid);
            return View(content);
        }

        public ActionResult userCollection(string type, string userName, string targetID, string targetType, string userDo)
        {
            //UserId
            var userID = (from u in _context.UserInfos
                          where u.UserAccount == userName
                          select u.UserId).First();
            switch (type)
            {
                case "like":
                    if (targetType == "A")
                    {

                        var havelike = (from ul in _context.UserLikes
                                        where ul.ArticleId == targetID
                                        where ul.UserId == userID
                                        select ul).ToList().Count();
                        if (userDo == "up")
                        {
                            var articleLikePlus = _context.Articles.Find(targetID);
                            articleLikePlus.ArticleLike++;

                            if (havelike == 0)
                            {
                                UserLike userlike = new UserLike
                                {
                                    UserId = userID,
                                    ArticleId = targetID,
                                    CreateUser = userName,
                                    CreateDate = DateTime.Now,
                                    LastupdateUser = userName,
                                    LastupdateDate = DateTime.Now,
                                };
                                _context.UserLikes.Add(userlike);
                                _context.SaveChanges();
                                return Content("Success");
                            }
                            else
                            {
                                var userlikeID = (from ul in _context.UserLikes
                                                  where ul.ArticleId == targetID
                                                  where ul.UserId == userID
                                                  select ul.LikeId).ToList()[0];
                                var updateUserlike = _context.UserLikes.Find(userlikeID);
                                updateUserlike.LikeStatus = "T";
                                updateUserlike.LastupdateDate = DateTime.Now;
                                updateUserlike.Version++;
                                _context.SaveChanges();
                                return Content("Success");
                            }
                        }
                        else
                        {
                            var articleLikePlus = _context.Articles.Find(targetID);
                            articleLikePlus.ArticleLike--;


                            var userlikeID = (from ul in _context.UserLikes
                                              where ul.ArticleId == targetID
                                              where ul.UserId == userID
                                              select ul.LikeId).ToList()[0];
                            var updateUserlike = _context.UserLikes.Find(userlikeID);
                            updateUserlike.LikeStatus = "F";
                            updateUserlike.LastupdateDate = DateTime.Now;
                            updateUserlike.Version++;
                            _context.SaveChanges();
                            return Content("Success");
                        }
                    }
                    else
                    {
                        var havelike = (from ul in _context.UserLikeComments
                                        where ul.CommentId == targetID
                                        where ul.UserId == userID
                                        select ul).ToList().Count();
                        if (userDo == "up")
                        {
                            var articleLikePlus = _context.Comments.Find(targetID);
                            articleLikePlus.CommentLike++;

                            if (havelike == 0)
                            {
                                UserLikeComment userlike = new UserLikeComment
                                {
                                    UserId = userID,
                                    CommentId = targetID,
                                    CreateUser = userName,
                                    CreateDate = DateTime.Now,
                                    LastupdateUser = userName,
                                    LastupdateDate = DateTime.Now,
                                };
                                _context.UserLikeComments.Add(userlike);
                                _context.SaveChanges();
                                return Content("Success");
                            }
                            else
                            {
                                var userlikeID = (from ul in _context.UserLikeComments
                                                  where ul.CommentId == targetID
                                                  where ul.UserId == userID
                                                  select ul.LikecommentId).ToList()[0];
                                var updateUserlike = _context.UserLikeComments.Find(userlikeID);
                                updateUserlike.LikecommentStatus = "T";
                                updateUserlike.LastupdateDate = DateTime.Now;
                                updateUserlike.Version++;
                                _context.SaveChanges();
                                return Content("Success");
                            }
                        }
                        else
                        {
                            var articleLikePlus = _context.Comments.Find(targetID);
                            articleLikePlus.CommentLike--;

                            var userlikeID = (from ul in _context.UserLikeComments
                                              where ul.CommentId == targetID
                                              where ul.UserId == userID
                                              select ul.LikecommentId).ToList()[0];
                            var updateUserlike = _context.UserLikeComments.Find(userlikeID);
                            updateUserlike.LikecommentStatus = "F";
                            updateUserlike.LastupdateDate = DateTime.Now;
                            updateUserlike.Version++;
                            _context.SaveChanges();
                            return Content("Success");
                        }
                    }
                case "collect":
                    var havecollect = (from ul in _context.UserCollects
                                       where ul.ArticleId == targetID
                                       where ul.UserId == userID
                                       select ul).ToList().Count();
                    if (userDo == "up")
                    {

                        if (havecollect == 0)
                        {
                            UserCollect usercollect = new UserCollect
                            {
                                UserId = userID,
                                ArticleId = targetID,
                                CreateUser = userName,
                                CreateDate = DateTime.Now,
                                LastupdateUser = userName,
                                LastupdateDate = DateTime.Now,
                            };
                            _context.UserCollects.Add(usercollect);
                            _context.SaveChanges();
                            return Content("Success");
                        }
                        else
                        {
                            var usercollectID = (from ul in _context.UserCollects
                                                 where ul.ArticleId == targetID
                                                 where ul.UserId == userID
                                                 select ul.CollectId).ToList()[0];
                            var updateUserCollect = _context.UserCollects.Find(usercollectID);
                            updateUserCollect.CollectStatus = "T";
                            updateUserCollect.LastupdateDate = DateTime.Now;
                            updateUserCollect.Version++;
                            _context.SaveChanges();
                            return Content("Success");
                        }
                    }
                    else
                    {
                        var usercollectID = (from ul in _context.UserCollects
                                             where ul.ArticleId == targetID
                                             where ul.UserId == userID
                                             select ul.CollectId).ToList()[0];
                        var updateUserCollect = _context.UserCollects.Find(usercollectID);
                        updateUserCollect.CollectStatus = "F";
                        updateUserCollect.LastupdateDate = DateTime.Now;
                        updateUserCollect.Version++;
                        _context.SaveChanges();
                        return Content("Success");
                    };
                case "follow":
                    if (userDo == "up")
                    {
                        if (targetType == "A")
                        {

                            var havefollow = (from ul in _context.UserFollows
                                              where ul.ArticleId == targetID
                                              where ul.UserId == userID
                                              select ul).ToList().Count();

                            if (havefollow == 0)
                            {
                                UserFollow userfollow = new UserFollow
                                {
                                    UserId = userID,
                                    ArticleId = targetID,
                                    CreateUser = userName,
                                    CreateDate = DateTime.Now,
                                    LastupdateUser = userName,
                                    LastupdateDate = DateTime.Now,
                                };
                                _context.UserFollows.Add(userfollow);
                                _context.SaveChanges();
                                return Content("Success");
                            }
                            else
                            {
                                var userfollowID = (from ul in _context.UserFollows
                                                    where ul.ArticleId == targetID
                                                    where ul.UserId == userID
                                                    select ul.FollowId).ToList()[0];
                                var updateUserCollect = _context.UserFollows.Find(userfollowID);
                                updateUserCollect.FollowStatus = "T";
                                updateUserCollect.LastupdateDate = DateTime.Now;
                                updateUserCollect.Version++;
                                _context.SaveChanges();
                                return Content("Success");
                            }
                        }
                        else
                        {
                            var havefollow = (from ul in _context.UserFollowers
                                              where ul.FollowerUserId == targetID
                                              where ul.UserId == userID
                                              select ul).ToList().Count();

                            if (havefollow == 0)
                            {
                                UserFollower userfollower = new UserFollower
                                {
                                    UserId = userID,
                                    FollowerUserId = targetID,
                                    CreateUser = userName,
                                    CreateDate = DateTime.Now,
                                    LastupdateUser = userName,
                                    LastupdateDate = DateTime.Now,
                                };
                                _context.UserFollowers.Add(userfollower);
                                _context.SaveChanges();
                                return Content("followUser");
                            }
                            else
                            {
                                var userfollowerID = (from ul in _context.UserFollowers
                                                    where ul.FollowerUserId == targetID
                                                    where ul.UserId == userID
                                                    select ul.FollowerId).First();
                                var updateUserfollower = _context.UserFollowers.Find(userfollowerID);
                                updateUserfollower.FollowerStatus = "T";
                                updateUserfollower.LastupdateDate = DateTime.Now;
                                updateUserfollower.Version++;
                                _context.SaveChanges();
                                return Content("followUser");
                            }
                        }
                    }
                    else
                    {
                        if (targetType =="A")
                        {

                        var userfollowID = (from ul in _context.UserFollows
                                            where ul.ArticleId == targetID
                                            where ul.UserId == userID
                                            select ul.FollowId).ToList()[0];
                        var updateUserCollect = _context.UserFollows.Find(userfollowID);
                        updateUserCollect.FollowStatus = "F";
                        updateUserCollect.LastupdateDate = DateTime.Now;
                        updateUserCollect.Version++;
                        _context.SaveChanges();
                        return Content("Success");
                        }
                        else
                        {
                            var userfollowerID = (from ul in _context.UserFollowers
                                                  where ul.FollowerUserId == targetID
                                                  where ul.UserId == userID
                                                  select ul.FollowerId).First();
                            var updateUserfollower = _context.UserFollowers.Find(userfollowerID);
                            updateUserfollower.FollowerStatus = "F";
                            updateUserfollower.LastupdateDate = DateTime.Now;
                            updateUserfollower.Version++;
                            _context.SaveChanges();
                            return Content("followUser");
                        }
                    };
                default:
                    return Content("False");
            }
        }

        public ActionResult tagcheck(string tagAlsoName)
        {
            if (String.IsNullOrEmpty(tagAlsoName) || String.IsNullOrWhiteSpace(tagAlsoName))
            {

                return Content("tagEmpty");
            }
            else
            {
                var haveTagAlso = (from tagalso in _context.Tagalsos
                                   where tagalso.TagalsoName.Contains(tagAlsoName)
                                   select tagalso.TagId).Count();
                if (haveTagAlso > 0)
                {

                    var tagId = (from tagalso in _context.Tagalsos
                                 where tagalso.TagalsoName == tagAlsoName
                                 select tagalso.TagId).First();
                    var tagName = (from t in _context.Tags
                                   where t.TagId == tagId
                                   select t.TagName).First();
                    return Content($"{tagName}");
                }
                else
                {
                    return Content($"{tagAlsoName}");

                }
            }
        }


        public class ArticleEditDetail {
            public string ArticleId { get; set; }
            public string ArticleTitle { get; set; }
            public int TagNum { get; set; }
            public string ArticleContent { get; set; }
            public string ArticleAccount { get; set; }
            public List<string> tagList { get; set; }
            public List<string> ArticleTagList { get; set; }
        }
        public class EditTag
        {
            public string ArticleId { get; set; }
            public string TagId { get; set; }
            public string TagName { get; set; }
            public string ArticleTagId { get; set; }
        };
        public class ArticleComment
        {
            public string CommentAccount { get; set; }
            public int CommentVersion { get; set; }
            public DateTime LastUpdateDate { get; set; }
            public DateTime CreateDate { get; set; }
            public string CommentId { get; set; }
            public string CommentContent { get; set; }
            public string CommentStatus { get; set; }
            public string CommentNickname { get; set; }
            public int CommentCollections { get; set; }
            public int CommentLike { get; set; }
            public string CommentAnswer { get; set; }
            public int CommentResponseCount { get; set; }
            public List<Response_class> CommentResponseList { get; set; }
            public byte[] CommentImage { get; set; }

        }
        public class Response_class
        {
            public string ResponseAccount { get; set; }
            public DateTime CreateDate { get; set; }
            public string ResponseID { get; set; }
            public string ResponseContent { get; set; }
            public string ResponseStatus { get; set; }
            public string ResponseNickname { get; set; }
            public string ResponseParentID { get; set; }
            public string ResponseParentClass { get; set; }
            public byte[] ResponseImage { get; set; }
        }


        public class commemtDelete
        {
            public string ArticleTitle { get; set; }
            public string ArticleId { get; set; }
            public Comment Commemt { get; set; }
        };
        public class articleDetail
        {
            public string ArticleId { get; set; }
            public string ArticleTitle { get; set; }
            public string ArticleContent { get; set; }
            public int ArticleViews { get; set; }
            public int ArticleLike { get; set; }
            public List<int> ArticleCollections { get; set; }
            public string ArticleStatus { get; set; }
            public string Nickname { get; set; }
            public int ArticleVersion { get; set; }
            public string CreateUser { get; set; }
            public DateTime CreateDate { get; set; }
            public string LastupdateUser { get; set; }
            public DateTime LastupdateDate { get; set; }
            public byte[] ArticleImage { get; set; }
            public List<Tag> ArticleTagList { get; set; }
            public int ArticleResponseCount { get; set; }
            public List<Response_class> ArticleResponseList { get; set; }
            public List<ArticleComment> ArticleCommentList { get; set; }
            public List<hotList> hotArticleList { get; set; }
            public List<hotList> hotQuestionList { get; set; }
            public string questiontitel { set; get; }
            public List<questionAndQptions> questionList { set; get; }

        };

        public class relatedArticle
        {
            public string ArticleId { get; set; }
            public string TagName { get; set; }
        };

        public class hotList
        {
            public string itemID { get; set; }
            public string itemTitle { get; set; }
        };

        public class hotTagList
        {
            public string Tagname { get; set; }
            public string TagID { get; set; }
            public int TagUseCount { get; set; }
        };

        public class checkAccountRepeated
        {
            public int Account { get; set; }
            public int Nickname { get; set; }
            public int Email { get; set; }

        }



        //判斷email格式
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public class questionAndQptions
        {
            public string QuestionName { set; get; }
            public string QuestionOption { set; get; }
            public string IscorrectAnswer { set; get; }
            public string QuestionId { set; get; }
            public string QuestionType { set; get; }
        }

    }
}

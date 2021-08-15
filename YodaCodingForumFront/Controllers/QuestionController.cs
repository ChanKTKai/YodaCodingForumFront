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
using static YodaCodingForumFront.Controllers.ArticleController;

namespace YodaCodingForumFront.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ArticleDBContext _context;

        public QuestionController(ArticleDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        //文章列表
        public IActionResult questionWall()
        {

            //文章或問答類型
            var typeQuery = from a in _context.Articles
                            select a.ArticleType;

            //文章query
            var articleQuery = from a in _context.Articles
                               where a.ArticleType == "Q"
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
                questiontitel = questionQuerytitel1.QuestionName
            };

            return View(articleTypeVM);
        }

        //發表文章
        public IActionResult questionpage()
        {
            if (HttpContext.Session.GetString("userName") == null)
            {
                return Redirect("/question/questionwall");

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
        public IActionResult questionpage(string articleTitle, string textNoneMD, string[] eachTagName)
        {
            var currentTime = DateTime.Now;
            //Article
            Article newArticle = new Article
            {
                ArticleType = "Q",
                ArticleContent = textNoneMD,
                ArticleViews = 0,
                ArticleLike = 0,
                ArticleStatus = "A",
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
            return Redirect($"/question/question?q={articleID}");
        }
        //文章顯示頁面
        public IActionResult question()
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

            string id = HttpContext.Request.Query["q"].ToString();

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
                               where a.ArticleId == id
                               where a.ArticleType == "Q"
                               join ua in _context.UserInfos on a.CreateUser equals ua.UserAccount
                               select new articleDetail
                               {
                                   //ArticleDetail
                                   ArticleId = id,
                                   ArticleTitle = a.ArticleTitle,
                                   ArticleContent = a.ArticleContent,
                                   ArticleViews = a.ArticleViews,
                                   ArticleLike = a.ArticleLike,
                                   ArticleStatus = a.ArticleStatus,
                                   ArticleVersion = a.Version,
                                   Nickname = ua.UserNickname,
                                   CreateUser = a.CreateUser,
                                   CreateDate = a.CreateDate,
                                   LastupdateUser = a.LastupdateUser,
                                   LastupdateDate = a.LastupdateDate,
                                   ArticleImage = (from i in _context.UserImages
                                                   where i.UserId == ua.UserId
                                                   select i.UserId).Count() == 1 ?
                                                  (from i in _context.UserImages
                                                   where i.UserId == ua.UserId
                                                   select i.Uimage).First() :
                                                  (from i in _context.UserImages
                                                   where i.ImageId == "Defult" + ua.UserSax
                                                   select i.Uimage).First(),
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
                                                         join cu in _context.UserInfos on c.CreateUser equals cu.UserAccount
                                                         where c.ArticleId == a.ArticleId
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
                                                             CommentAnswer = c.CommentAnswer,
                                                             CommentLike = c.CommentLike,
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
                                                                             where i.UserId == cu.UserId
                                                                             select i.UserId).Count() == 1 ?
                                                                             (from i in _context.UserImages
                                                                              where i.UserId == cu.UserId
                                                                              select i.Uimage).First() :
                                                                              (from i in _context.UserImages
                                                                               where i.ImageId == "Defult" + cu.UserSax
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
                                                      where hotq.ArticleStatus == "F"|| hotq.ArticleStatus == "A"
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
                           where a.ArticleType == "Q"
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



            //將與本篇問答TagName一樣的放進相關問答裡

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
        public IActionResult questionEdit()
        {

            var articleID = HttpContext.Request.Query["q"].ToString();
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
                return Redirect("/question/questionwall");
            }
        }
        //接收文章更新資料
        [HttpPost]
        public IActionResult questionEdit(string textNoneMD, string articleTitle, string[] eachTagName)
        {
            string articleID = HttpContext.Request.Query["q"].ToString();
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
            return Redirect($"/question/question?q={articleID}");
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
                return Redirect("/question/questionWall");
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
            return Redirect($"/question/question?q={articleId}");
        }
        //問題刪除
        public IActionResult questionDelete(string id)
        {
            var deleteArticle = _context.Articles.Find(id);
            deleteArticle.ArticleStatus = "D";
            deleteArticle.Version++;

            var articleAccountList = (from a in _context.Articles
                                      where a.ArticleId == id
                                      select a.CreateUser).ToList();
            var articleAccount = articleAccountList[0];
            string articleAuthor = HttpContext.Session.GetString("userName") ?? "noAuthor";
            if (articleAccount == articleAuthor)
            {
                _context.SaveChanges();
                return Redirect("/question/questionwall");
            }
            else
            {
                return Redirect("/question/questionwall");
            }

        }
        //留言刪除
        public IActionResult commentDelete(string id, string aid)
        {
            var deletecomment = _context.Comments.Find(id);
            deletecomment.CommentStatus = "D";
            deletecomment.Version++;

            var commentAccountList = (from a in _context.Comments
                                      where a.CommentId == id
                                      select a.CreateUser).ToList();
            var commentAccount = commentAccountList[0];
            string commentAuthor = HttpContext.Session.GetString("userName") ?? "notAuthor";
            if (commentAccount == commentAuthor)
            {
                _context.SaveChanges();
                return Redirect($"/question/question?q={aid}");
            }
            else
            {
                return Redirect("/question/questionwall");
            }

        }

        //問題回覆刪除
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
                return Redirect($"/question/question?q={aid}");
            }
            else
            {
                return Redirect("/question/questionwall");
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
                return Redirect($"/question/question?q={aid}");
            }
            else
            {
                return Redirect("/question/questionwall");
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

            var typeQuery = from a in _context.Articles
                            select a.ArticleType;

            //搜尋類別為Tag:名稱包含查詢條件的文章/問答。
            if (type == "Tag")
            {

                var articleQuery = from a in _context.Articles
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
                };

                return View(articleTypeVM);
            }

            //搜尋類別為文章OR問答
            else
            {
                var articleQuery = from a in _context.Articles
                                   orderby a.ArticleLike descending
                                   select new indexArticleList
                                   {
                                       Title = a.ArticleTitle,
                                       Views = a.ArticleViews,
                                       Like = a.ArticleLike,
                                       Type = a.ArticleType,
                                       CreateUser = a.CreateUser,
                                       CreateTime = a.CreateDate,
                                       commendCount = (from c in _context.Comments
                                                       where c.ArticleId == a.ArticleId
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
                    ArticleList = articleQuery.ToList()
                };

                return View(articleTypeVM);
            }
        }

        public IActionResult bestAnswer(string id, string aid) {

            //先設其他全部為F
            var otherAnswer = (from c in _context.Comments
                               where c.ArticleId == aid
                               select c.CommentId).ToList();
            foreach (var answer in otherAnswer)
            {
                var changetoF = _context.Comments.Find(answer);
                changetoF.CommentAnswer = "F";
            }
            //設答案為F
            var bestAnswer = _context.Comments.Find(id);
            bestAnswer.CommentAnswer = "Y";
            var soluteQuestion = _context.Articles.Find(aid);
            soluteQuestion.ArticleStatus = "F";
            soluteQuestion.LastupdateUser = HttpContext.Session.GetString("userName");
            soluteQuestion.LastupdateDate = DateTime.Now;
            soluteQuestion.Version++;

            //判斷是不是問題發文者操作

            var questionAuthor = (from a in _context.Articles
                                  where a.ArticleId == aid
                                  select a.CreateUser).ToList()[0];

            if (questionAuthor == HttpContext.Session.GetString("userName"))
            {
                _context.SaveChanges();
                return Redirect($"/question/question?q={aid}");
            }
            else {
                return Redirect("/question/questionwall");
            }
        }
    }
}

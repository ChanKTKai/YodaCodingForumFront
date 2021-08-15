using YodaCodingForumFront.Models;
using YodaCodingForumFront.Models.SearchModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static YodaCodingForumFront.Controllers.ArticleController;

namespace YodaCodingForumFront.Controllers
{
    public class TagController : Controller
    {

        private readonly ArticleDBContext _context;

        public TagController(ArticleDBContext context)
        {
            _context = context;
        }


        public IActionResult TagWall()
        {

            string nowuserName = HttpContext.Session.GetString("userName") is null ? "" : HttpContext.Session.GetString("userName");

            var tagQuery = from tag in _context.Tags
                           select new TagInfo
                           {
                               tagid = tag.TagId,
                               tagname = tag.TagName,

                               usecount = (from a in _context.Articles
                                           join at in _context.ArticleAndTags on a.ArticleId equals at.ArticleId
                                           where a.ArticleStatus == "N" || a.ArticleStatus == "A" || a.ArticleStatus == "F"
                                           where at.TagId == tag.TagId
                                           select a).Count(),

                               articleCount = (from a in _context.Articles
                                               join at in _context.ArticleAndTags on a.ArticleId equals at.ArticleId
                                               where a.ArticleType == "A"
                                               where a.ArticleStatus == "N"
                                               where at.TagId == tag.TagId
                                               select a).Count(),

                               questionCount = (from q in _context.Articles
                                                join at in _context.ArticleAndTags on q.ArticleId equals at.ArticleId
                                                where q.ArticleType == "Q"
                                                where q.ArticleStatus == "A" || q.ArticleStatus == "F"
                                                where at.TagId == tag.TagId
                                                select q).Count(),

                               followCount = (from f in _context.UserFollowTags
                                              where f.TagId == tag.TagId
                                              select f).Count(),

                               userIsFollow = (from f in _context.UserFollowTags
                                               join u in _context.UserInfos on f.UserId equals u.UserId
                                               where u.UserAccount == nowuserName
                                               where f.TagId == tag.TagId
                                               select f).Count()

                           };

            //使用次數降序排列
            tagQuery = tagQuery.OrderByDescending(a => a.usecount);

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


            TagList mytaglist = new TagList()
            {
                tags = tagQuery.ToList(),

                hotArticleList = (from hota in _context.Articles
                                  where hota.ArticleType == "A"
                                  where hota.ArticleStatus == "N"
                                  orderby hota.ArticleViews descending
                                  select new tagPageHotList
                                  {
                                      itemID = hota.ArticleId,
                                      itemTitle = hota.ArticleTitle
                                  }).Take(5).ToList(),
                //HotQuestion
                hotQuestionList = (from hotq in _context.Articles
                                   where hotq.ArticleType == "Q"
                                   where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                   orderby hotq.ArticleViews descending
                                   select new tagPageHotList
                                   {
                                       itemID = hotq.ArticleId,
                                       itemTitle = hotq.ArticleTitle
                                   }).Take(5).ToList(),

                hotTagList = (from a in _context.Articles
                              join atag in _context.ArticleAndTags on a.ArticleId equals atag.ArticleId
                              where a.ArticleStatus != "D"
                              select new tagPageHotList
                              {
                                  itemID = atag.TagId,
                                  itemTitle = (from t in _context.Tags
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

            return View(mytaglist);
        }

        public IActionResult tagSearch(string tagid)
        {

            string nowuserName = HttpContext.Session.GetString("userName");

            var tagQuery = from tag in _context.Tags
                           where tag.TagId == tagid
                           select new TagInfo
                           {
                               tagid = tag.TagId,
                               tagname = tag.TagName,

                               usecount = (from a in _context.Articles
                                           join at in _context.ArticleAndTags on a.ArticleId equals at.ArticleId
                                           where a.ArticleStatus == "N" || a.ArticleStatus == "A" || a.ArticleStatus == "F"
                                           where at.TagId == tag.TagId
                                           select a).Count(),

                               articleCount = (from a in _context.Articles
                                               join at in _context.ArticleAndTags on a.ArticleId equals at.ArticleId
                                               where a.ArticleType == "A"
                                               where a.ArticleStatus == "N"
                                               where at.TagId == tag.TagId
                                               select a).Count(),

                               questionCount = (from q in _context.Articles
                                                join at in _context.ArticleAndTags on q.ArticleId equals at.ArticleId
                                                where q.ArticleType == "Q"
                                                where q.ArticleStatus == "A" || q.ArticleStatus == "F"
                                                where at.TagId == tag.TagId
                                                select q).Count(),

                               followCount = (from f in _context.UserFollowTags
                                              where f.TagId == tag.TagId
                                              select f).Count(),

                               userIsFollow = (from f in _context.UserFollowTags
                                               join u in _context.UserInfos on f.UserId equals u.UserId
                                               where u.UserAccount == nowuserName
                                               where f.TagId == tag.TagId
                                               select f).Count()

                           };


            var articleQuery = from a in _context.Articles
                               join at in _context.ArticleAndTags on a.ArticleId equals at.ArticleId
                               where a.ArticleType == "A" || a.ArticleType == "Q"
                               where a.ArticleStatus == "N" || a.ArticleStatus == "A" || a.ArticleStatus == "F"
                               where at.TagId == tagid
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


            TagList mytaglist = new TagList()
            {
                tags = tagQuery.ToList(),

                articles = articleQuery.ToList(),

                hotArticleList = (from hota in _context.Articles
                                  where hota.ArticleType == "A"
                                  where hota.ArticleStatus == "N"
                                  orderby hota.ArticleViews descending
                                  select new tagPageHotList
                                  {
                                      itemID = hota.ArticleId,
                                      itemTitle = hota.ArticleTitle
                                  }).Take(5).ToList(),
                //HotQuestion
                hotQuestionList = (from hotq in _context.Articles
                                   where hotq.ArticleType == "Q"
                                   where hotq.ArticleStatus == "A" || hotq.ArticleStatus == "F"
                                   orderby hotq.ArticleViews descending
                                   select new tagPageHotList
                                   {
                                       itemID = hotq.ArticleId,
                                       itemTitle = hotq.ArticleTitle
                                   }).Take(5).ToList(),

                hotTagList = (from a in _context.Articles
                              join atag in _context.ArticleAndTags on a.ArticleId equals atag.ArticleId
                              where a.ArticleStatus != "D"
                              select new tagPageHotList
                              {
                                  itemID = atag.TagId,
                                  itemTitle = (from t in _context.Tags
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
            return View(mytaglist);
        }

        public ActionResult userCollection(string type, string userName, string targetID, string targetType, string userDo)
        {

            if (userName == "Guest")
            {
                return Content("pleaseLogin");
            }
            else
            {
                //UserId
                string userID = (from u in _context.UserInfos
                                 where u.UserAccount == userName
                                 select u.UserId).First();

                int havefollow = (from ul in _context.UserFollowTags
                                  where ul.TagId == targetID
                                  where ul.UserId == userID
                                  select ul).Count();

                if (userDo == "up")
                {
                    //沒追蹤過
                    if (havefollow == 0)
                    {
                        UserFollowTag usercollect = new UserFollowTag
                        {
                            UserId = userID,
                            TagId = targetID,
                            CreateUser = userName,
                            CreateDate = DateTime.Now,
                            LastupdateUser = userName,
                            LastupdateDate = DateTime.Now,
                        };

                        _context.UserFollowTags.Add(usercollect);
                        _context.SaveChanges();
                        return Content("Success");
                    }
                    else
                    {
                        var userFollowtagId = (from ul in _context.UserFollowTags
                                               where ul.TagId == targetID
                                               where ul.UserId == userID
                                               select ul.FollowtagId).First();
                        var updateUserFollow = _context.UserFollowTags.Find(userFollowtagId);
                        updateUserFollow.FollowtagStatus = "T";
                        updateUserFollow.LastupdateUser = userName;
                        updateUserFollow.LastupdateDate = DateTime.Now;
                        updateUserFollow.Version++;

                        _context.SaveChanges();
                        return Content("Success");
                    }
                }

                //取消收藏
                else
                {
                    var userFollowtagId = (from ul in _context.UserFollowTags
                                           where ul.TagId == targetID
                                           where ul.UserId == userID
                                           select ul.FollowtagId).First();
                    var updateUserFollow = _context.UserFollowTags.Find(userFollowtagId);
                    updateUserFollow.FollowtagStatus = "F";
                    updateUserFollow.LastupdateUser = userName;
                    updateUserFollow.LastupdateDate = DateTime.Now;
                    updateUserFollow.Version++;
                    _context.SaveChanges();
                    return Content("Success");
                };
            }

        }

    }
}

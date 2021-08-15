using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using static YodaCodingForumFront.Controllers.ArticleController;

namespace YodaCodingForumFront.Models.SearchModels
{
    public class ArticleGenreViewMode
    {
        public List<indexArticleList> ArticleList { set; get; }
        public SelectList Types { set; get; }
        public string Type { set; get; }
        public string SearchString { set; get; }
        public List<hotList> hotArticleList { get; set; }
        public List<hotList> hotQuestionList { get; set; }
        public List<hotTagList> hotTagList { get; set; }
        public List<articleAndTagForSearch> TagList { set; get; }
        public string questiontitel { set; get; }
        public List<questionAndQptions> questionList { set; get; }
    }
}

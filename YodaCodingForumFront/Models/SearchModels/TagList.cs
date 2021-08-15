using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static YodaCodingForumFront.Controllers.ArticleController;

namespace YodaCodingForumFront.Models.SearchModels
{
    public class TagList
    {
        public List<TagInfo> tags { get; set; }
        public List<indexArticleList> articles { get; set; }
        public List<tagPageHotList> hotArticleList { get; set; }
        public List<tagPageHotList> hotQuestionList { get; set; }
        public List<tagPageHotList> hotTagList { get; set; }

        public string questiontitel { set; get; }
        public List<questionAndQptions> questionList { set; get; }
    }
}

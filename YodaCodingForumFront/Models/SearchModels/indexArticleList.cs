using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static YodaCodingForumFront.Controllers.ArticleController;

namespace YodaCodingForumFront.Models.SearchModels
{
    public class indexArticleList
    {
        public string aid { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public int Like { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int commendCount { get; set; }
        public List<Tag> tag { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
    }
}

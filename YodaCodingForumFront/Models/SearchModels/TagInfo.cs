using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YodaCodingForumFront.Models.SearchModels
{
    public class TagInfo
    {
        public string tagid { get; set; }
        public string tagname { get; set; }
        public int usecount { get; set; }
        public int articleCount { get; set; }
        public int questionCount { get; set; }
        public int followCount { get; set; }
        public int userIsFollow { get; set; }
    }
}

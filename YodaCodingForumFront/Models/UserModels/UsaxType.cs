using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YodaCodingForumFront.Models.UserModels
{
    public class UsaxType
    {
        public List<userinfoList> userinfo { set; get; }
        public string code { get; internal set; }
        public string name { get; internal set; }
    }
}

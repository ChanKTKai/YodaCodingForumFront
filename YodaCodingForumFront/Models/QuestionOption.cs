using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class QuestionOption
    {
        public string QoptionId { get; set; }
        public string QuestionId { get; set; }
        public string QoptionName { get; set; }
        public string IscorrectAnswer { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual Question Question { get; set; }
    }
}

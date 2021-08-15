using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Question
    {
        public Question()
        {
            QuestionOptions = new HashSet<QuestionOption>();
        }

        public string QuestionId { get; set; }
        public string PlId { get; set; }
        public string QuestionName { get; set; }
        public string QuestionType { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual ProgramLang Pl { get; set; }
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }
    }
}

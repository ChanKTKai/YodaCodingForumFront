using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class ProgramLang
    {
        public ProgramLang()
        {
            Questions = new HashSet<Question>();
        }

        public string PlId { get; set; }
        public string PlName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Tagalso
    {
        public string TagalsoId { get; set; }
        public string TagId { get; set; }
        public string TagalsoName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual Tag Tag { get; set; }
    }
}

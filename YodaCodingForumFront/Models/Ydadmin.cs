using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Ydadmin
    {
        public string AdminId { get; set; }
        public string AdminAccount { get; set; }
        public string AdminPassword { get; set; }
        public string AdminStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }
    }
}

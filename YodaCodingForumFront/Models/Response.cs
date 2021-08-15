using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Response
    {
        public string ResponseId { get; set; }
        public string ParentId { get; set; }
        public string ParentClass { get; set; }
        public string ResponseContent { get; set; }
        public string ResponseStatus { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }
    }
}

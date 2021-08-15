using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class ReportReason
    {
        public string ReasonId { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class Report
    {
        public string ReportId { get; set; }
        public string UserId { get; set; }
        public string ReportTargetId { get; set; }
        public string ReportTargetType { get; set; }
        public string ReasonCode { get; set; }
        public string ReportRemarks { get; set; }
        public string ReportContents { get; set; }
        public string ReportStatus { get; set; }
        public string ReportVerifyPerson { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastupdateUser { get; set; }
        public DateTime LastupdateDate { get; set; }
        public int Version { get; set; }

        public virtual UserInfo User { get; set; }
    }
}

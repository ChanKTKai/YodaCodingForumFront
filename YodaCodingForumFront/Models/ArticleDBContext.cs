using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace YodaCodingForumFront.Models
{
    public partial class ArticleDBContext : DbContext
    {
        public ArticleDBContext()
        {
        }

        public ArticleDBContext(DbContextOptions<ArticleDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleAndTag> ArticleAndTags { get; set; }
        public virtual DbSet<Chatroom> Chatrooms { get; set; }
        public virtual DbSet<CollectClass> CollectClasses { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<NoticeClass> NoticeClasses { get; set; }
        public virtual DbSet<ProgramLang> ProgramLangs { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
        public virtual DbSet<ReceiveMessage> ReceiveMessages { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportReason> ReportReasons { get; set; }
        public virtual DbSet<Response> Responses { get; set; }
        public virtual DbSet<SendMessage> SendMessages { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Tagalso> Tagalsos { get; set; }
        public virtual DbSet<UserBlock> UserBlocks { get; set; }
        public virtual DbSet<UserCollect> UserCollects { get; set; }
        public virtual DbSet<UserFollow> UserFollows { get; set; }
        public virtual DbSet<UserFollowTag> UserFollowTags { get; set; }
        public virtual DbSet<UserFollower> UserFollowers { get; set; }
        public virtual DbSet<UserImage> UserImages { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<UserLike> UserLikes { get; set; }
        public virtual DbSet<UserLikeComment> UserLikeComments { get; set; }
        public virtual DbSet<UserNotice> UserNotices { get; set; }
        public virtual DbSet<UserPoint> UserPoints { get; set; }
        public virtual DbSet<Ydadmin> Ydadmins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("Article");

                entity.Property(e => e.ArticleId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleContent)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("article_content");

                entity.Property(e => e.ArticleLike).HasColumnName("article_like");

                entity.Property(e => e.ArticleStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("article_status");

                entity.Property(e => e.ArticleTitle)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("article_title");

                entity.Property(e => e.ArticleType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("article_type");

                entity.Property(e => e.ArticleViews).HasColumnName("article_views");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ArticleAndTag>(entity =>
            {
                entity.ToTable("ArticleAndTAG");

                entity.Property(e => e.ArticleandtagId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("articleandtag_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.TagId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("tag_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleAndTags)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArticleAndTAG_Article");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticleAndTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArticleAndTAG_TAG");
            });

            modelBuilder.Entity<Chatroom>(entity =>
            {
                entity.HasKey(e => e.ChatId);

                entity.ToTable("Chatroom");

                entity.Property(e => e.ChatId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("chat_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ChatContent)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("chat_content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Chatrooms)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chatroom_User");
            });

            modelBuilder.Entity<CollectClass>(entity =>
            {
                entity.ToTable("CollectClass");

                entity.Property(e => e.CollectclassId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("collectclass_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CollectclassName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("collectclass_name");

                entity.Property(e => e.CollectclassStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("collectclass_status");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CollectClasses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectClass_User");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("comment_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id");

                entity.Property(e => e.CommentAnswer)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("comment_answer");

                entity.Property(e => e.CommentClass)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("comment_class");

                entity.Property(e => e.CommentContent)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("comment_content");

                entity.Property(e => e.CommentLike).HasColumnName("comment_like");

                entity.Property(e => e.CommentStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("comment_status")
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Article");
            });

            modelBuilder.Entity<NoticeClass>(entity =>
            {
                entity.ToTable("NoticeClass");

                entity.Property(e => e.NoticeclassId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("noticeclass_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.NoticeclassName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("noticeclass_name");

                entity.Property(e => e.NoticeclassRemark)
                    .HasMaxLength(50)
                    .HasColumnName("noticeclass_remark");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ProgramLang>(entity =>
            {
                entity.HasKey(e => e.PlId);

                entity.ToTable("ProgramLang");

                entity.Property(e => e.PlId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("PL_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.PlName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PL_name");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.Property(e => e.QuestionId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("question_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.PlId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("PL_id");

                entity.Property(e => e.QuestionName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("question_name");

                entity.Property(e => e.QuestionType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("question_type");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Pl)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.PlId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Questions_Programming Languages");
            });

            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.HasKey(e => e.QoptionId);

                entity.ToTable("QuestionOption");

                entity.Property(e => e.QoptionId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("qoption_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.IscorrectAnswer)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("iscorrect_answer");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.QoptionName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("qoption_name");

                entity.Property(e => e.QuestionId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("question_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionOptions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question Options_Questions");
            });

            modelBuilder.Entity<ReceiveMessage>(entity =>
            {
                entity.HasKey(e => e.ReceiveId);

                entity.ToTable("ReceiveMessage");

                entity.Property(e => e.ReceiveId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("receive_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.ReceiveContent)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("receive_content");

                entity.Property(e => e.ReceiveStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("receive_status")
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReceiveMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceiveMessage_UserInfo");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("report_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.ReasonCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("reason_code");

                entity.Property(e => e.ReportContents)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("report_contents");

                entity.Property(e => e.ReportRemarks)
                    .HasMaxLength(50)
                    .HasColumnName("report_remarks");

                entity.Property(e => e.ReportStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("report_status")
                    .HasDefaultValueSql("('W')");

                entity.Property(e => e.ReportTargetId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("report_target_id");

                entity.Property(e => e.ReportTargetType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("report_target_type");

                entity.Property(e => e.ReportVerifyPerson)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("report_verify_person");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Report_UserInfo");
            });

            modelBuilder.Entity<ReportReason>(entity =>
            {
                entity.HasKey(e => e.ReasonId)
                    .HasName("PK_Table1");

                entity.ToTable("ReportReason");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("reason_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.ReasonCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("reason_code");

                entity.Property(e => e.ReasonName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("reason_name");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.ToTable("Response");

                entity.Property(e => e.ResponseId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("response_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.ParentClass)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("parent_class");

                entity.Property(e => e.ParentId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("parent_id");

                entity.Property(e => e.ResponseContent)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("response_content");

                entity.Property(e => e.ResponseStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("response_status")
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SendMessage>(entity =>
            {
                entity.HasKey(e => e.SendId);

                entity.ToTable("SendMessage");

                entity.Property(e => e.SendId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("send_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.SendContent)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("send_content");

                entity.Property(e => e.SendStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("send_status")
                    .HasDefaultValueSql("('R')");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SendMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMessage_User");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("TAG");

                entity.Property(e => e.TagId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("tag_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.TagName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tag_name");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Tagalso>(entity =>
            {
                entity.ToTable("Tagalso");

                entity.Property(e => e.TagalsoId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("tagalso_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.TagId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("tag_id");

                entity.Property(e => e.TagalsoName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tagalso_name");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Tagalsos)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tagalso_TAG");
            });

            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.HasKey(e => e.BlockId);

                entity.ToTable("UserBlock");

                entity.Property(e => e.BlockId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("block_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.BlockStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("block_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.BlockUserId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("block_user_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserBlocks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserBlock_User");
            });

            modelBuilder.Entity<UserCollect>(entity =>
            {
                entity.HasKey(e => e.CollectId)
                    .HasName("PK_User_Collect");

                entity.ToTable("UserCollect");

                entity.Property(e => e.CollectId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("collect_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id");

                entity.Property(e => e.CollectStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("collect_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.CollectclassId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("collectclass_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.UserCollects)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserCollect_Article");

                entity.HasOne(d => d.Collectclass)
                    .WithMany(p => p.UserCollects)
                    .HasForeignKey(d => d.CollectclassId)
                    .HasConstraintName("FK_UserCollect_CollectClass");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCollects)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserCollect_User");
            });

            modelBuilder.Entity<UserFollow>(entity =>
            {
                entity.HasKey(e => e.FollowId);

                entity.ToTable("UserFollow");

                entity.Property(e => e.FollowId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("follow_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.FollowStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("follow_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.UserFollows)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFollow_Article");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFollows)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFollow_User");
            });

            modelBuilder.Entity<UserFollowTag>(entity =>
            {
                entity.HasKey(e => e.FollowtagId);

                entity.ToTable("UserFollowTag");

                entity.HasIndex(e => e.TagId, "IXFK_UserFollowTag_TAG");

                entity.HasIndex(e => e.UserId, "IXFK_UserFollowTag_User");

                entity.Property(e => e.FollowtagId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("followtag_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.FollowtagStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("followtag_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.TagId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("tag_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserFollower>(entity =>
            {
                entity.HasKey(e => e.FollowerId);

                entity.ToTable("UserFollower");

                entity.Property(e => e.FollowerId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("follower_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.FollowerStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("follower_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.FollowerUserId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("follower_user_id");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFollowers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFollower_User");
            });

            modelBuilder.Entity<UserImage>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.ToTable("UserImage");

                entity.Property(e => e.ImageId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("image_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.Uimage)
                    .HasColumnType("image")
                    .HasColumnName("uimage");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserImages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserImage_UserInfo");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_User");

                entity.ToTable("UserInfo");

                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .HasColumnName("remark");

                entity.Property(e => e.UserAccount)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_account");

                entity.Property(e => e.UserAddress)
                    .HasMaxLength(200)
                    .HasColumnName("user_address");

                entity.Property(e => e.UserBirthday)
                    .HasColumnType("date")
                    .HasColumnName("user_birthday");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("user_email");

                entity.Property(e => e.UserExperience)
                    .HasColumnType("text")
                    .HasColumnName("user_experience");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserNickname)
                    .HasMaxLength(30)
                    .HasColumnName("user_nickname");

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_password");

                entity.Property(e => e.UserPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_phone");

                entity.Property(e => e.UserProfession)
                    .HasColumnType("text")
                    .HasColumnName("user_profession");

                entity.Property(e => e.UserSax)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("user_sax")
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UserStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("user_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserLike>(entity =>
            {
                entity.HasKey(e => e.LikeId)
                    .HasName("PK_User_Like");

                entity.ToTable("UserLike");

                entity.Property(e => e.LikeId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("like_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("article_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.LikeStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("like_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.UserLikes)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLike_Article");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLikes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLike_User");
            });

            modelBuilder.Entity<UserLikeComment>(entity =>
            {
                entity.HasKey(e => e.LikecommentId);

                entity.ToTable("UserLikeComment");

                entity.Property(e => e.LikecommentId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("likecomment_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CommentId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("comment_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.LikecommentStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("likecomment_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.UserLikeComments)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLikeComment_Comment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLikeComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLikeComment_User");
            });

            modelBuilder.Entity<UserNotice>(entity =>
            {
                entity.HasKey(e => e.NoticeId);

                entity.ToTable("UserNotice");

                entity.Property(e => e.NoticeId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("notice_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.NoticeContent)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("notice_content");

                entity.Property(e => e.NoticeSource)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("notice_source");

                entity.Property(e => e.NoticeStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("notice_status")
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.NoticeclassId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("noticeclass_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Noticeclass)
                    .WithMany(p => p.UserNotices)
                    .HasForeignKey(d => d.NoticeclassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotice_NoticeClass");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotice_User");
            });

            modelBuilder.Entity<UserPoint>(entity =>
            {
                entity.HasKey(e => e.PointId);

                entity.ToTable("UserPoint");

                entity.Property(e => e.PointId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("point_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.PointAmount).HasColumnName("point_amount");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("user_id");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPoints)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPoint_User");
            });

            modelBuilder.Entity<Ydadmin>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .HasName("PK_SystemAdmin");

                entity.ToTable("YDAdmin");

                entity.Property(e => e.AdminId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("admin_id")
                    .HasDefaultValueSql("(replace(newid(),'-',''))");

                entity.Property(e => e.AdminAccount)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("admin_account");

                entity.Property(e => e.AdminPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("admin_password");

                entity.Property(e => e.AdminStatus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("admin_status")
                    .HasDefaultValueSql("('T')");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("create_user");

                entity.Property(e => e.LastupdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdate_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastupdateUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastupdate_user");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasDefaultValueSql("((1))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

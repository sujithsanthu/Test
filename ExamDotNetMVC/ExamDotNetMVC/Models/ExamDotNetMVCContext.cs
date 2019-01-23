using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class ExamDotNetMVCContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ExamDotNetMVCContext() : base("name=ExamDotNetMVCContext")
        {
        }

        public System.Data.Entity.DbSet<ExamDotNetMVC.Models.AddQuestion> AddQuestions { get; set; }

        public System.Data.Entity.DbSet<ExamDotNetMVC.Models.UserDetail> UserDetails { get; set; }

        public System.Data.Entity.DbSet<ExamDotNetMVC.Models.ExamDetail> ExamDetails { get; set; }

        public System.Data.Entity.DbSet<ExamDotNetMVC.Models.MarksDetail> MarksDetails { get; set; }

        public System.Data.Entity.DbSet<ExamDotNetMVC.Models.Status> Status { get; set; }
    }
}

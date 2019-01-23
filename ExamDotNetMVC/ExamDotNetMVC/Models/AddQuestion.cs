using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class AddQuestion
    {
        [Key] public int Questionid { get; set; }
        public string Question { get; set; }
        public int QLevel { get; set; }
        public int Marks { get; set; }
    }
}
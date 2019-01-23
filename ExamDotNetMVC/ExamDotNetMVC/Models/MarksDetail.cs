using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class MarksDetail
    {
        [Key]
        public string Name { get; set; }
        public double TotalMarks { get; set; }
        public double AssignedMarks { get; set; }
        public double Percentage { get; set; }
    }
}
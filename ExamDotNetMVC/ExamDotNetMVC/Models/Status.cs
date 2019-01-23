using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class Status
    {
        [Key] public int Id { get; set; }
        public bool IsEntered { get; set; }
        public string Name { get; set; }
    }
}
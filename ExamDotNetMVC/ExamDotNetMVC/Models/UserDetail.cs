using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class UserDetail
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
}
using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models
{
    public class Teacher
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Course> AssignedCourses { get; set; } = new List<Course>();
    }
}
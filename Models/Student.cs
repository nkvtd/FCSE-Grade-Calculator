using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models
{
    public class Student
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required, StringLength(6)]
        public string Index { get; set; }
        public decimal? AverageGrade { get; set; }
        public virtual ICollection<Enrollment> EnrolledCourses { get; set; } = new List<Enrollment>();

    }
}
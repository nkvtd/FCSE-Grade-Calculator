using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(10)]
        public string Acronym { get; set; }

        public string TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<CourseComponent> Components { get; set; } = new List<CourseComponent>();
        public virtual GradeScale GradeScale { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    }
}
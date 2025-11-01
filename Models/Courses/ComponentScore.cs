using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class ComponentScore
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        public int CourseComponentId { get; set; }
        public virtual CourseComponent CourseComponent { get; set; }
        public decimal Points { get; set; }
    }
}
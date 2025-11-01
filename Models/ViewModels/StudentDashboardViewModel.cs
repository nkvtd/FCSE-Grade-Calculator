using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }
        public IEnumerable<Course> AvailableCourses { get; set; }
    }
}
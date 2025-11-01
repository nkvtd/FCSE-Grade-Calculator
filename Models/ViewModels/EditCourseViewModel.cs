using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.ViewModels
{
    public class EditCourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public List<CourseComponent> Components { get; set; } = new List<CourseComponent>();
        public List<GradeScaleComponent> GradeScaleComponents { get; set; } = new List<GradeScaleComponent>();
    }
}
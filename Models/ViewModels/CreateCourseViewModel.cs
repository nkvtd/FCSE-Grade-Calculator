using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FCSE_Grade_Calculator.Models.ViewModels
{
    public class CreateCourseViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Acronym { get; set; }        
        public List<CourseComponentViewModel> Components { get; set; } = new List<CourseComponentViewModel>();
        public List<GradeScaleItemViewModel> GradeScale { get; set; } = new List<GradeScaleItemViewModel>();
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public IEnumerable<SelectListItem> Teachers { get; set; }
    }

    public class  GradeScaleItemViewModel
    {
        public string GradeLabel { get; set; }
        public int GradeValue { get; set; }
        public decimal MinPoints { get; set; }
        public decimal MaxPoints { get; set; }
    }

    public class CourseComponentViewModel
    {
        public string Name { get; set; }
        public bool Included { get; set; }
        public decimal Weight { get; set; }
    }
}
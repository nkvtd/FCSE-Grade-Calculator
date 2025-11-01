using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class CourseComponent
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(0.0, 100.0)]
        public decimal Weight { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
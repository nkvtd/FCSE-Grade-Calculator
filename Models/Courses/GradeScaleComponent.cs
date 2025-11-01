using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class GradeScaleComponent
    {
        public int Id { get; set; }
        [Required]
        public string GradeLabel { get; set; }
        [Range(5, 10)]
        public int GradeValue { get; set; }
        public decimal MinPoints { get; set; }
        public decimal MaxPoints { get; set; }
        public int GradeScaleId { get; set; }
        public virtual GradeScale GradeScale { get; set; }
    }
}
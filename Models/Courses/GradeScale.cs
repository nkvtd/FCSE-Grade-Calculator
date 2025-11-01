using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class GradeScale
    {
        [Key, ForeignKey("Course")]
        public int Id { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<GradeScaleComponent> Components { get; set; } = new List<GradeScaleComponent>();
    }
}
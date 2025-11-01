using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Models.Courses
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public decimal? FinalPoints { get; set; }
        public string FinalGradeLabel { get; set; }
        public int FinalGradeValue { get; set; }
        public bool IsGradeConfirmed { get; set; }
        public virtual ICollection<ComponentScore> ComponentScores { get; set; } = new List<ComponentScore>();
    }
}
using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCSE_Grade_Calculator.Services
{
    public class GradeCalculationService
    {
        public decimal CalculateTotalPoints(Enrollment enrollment)
        {
            if (enrollment == null || enrollment.Course == null) return 0;

            var scores = enrollment.ComponentScores.ToList();

            decimal total = 0m;
            foreach (var score in scores)
            {
                if (score.CourseComponent != null)
                {
                    total += (score.Points * (score.CourseComponent.Weight / 100m));
                }
            }

            return decimal.Round(total, 2);
        }

        public string CalculateGradeLabel(decimal totalPoints, GradeScale scale)
        {
            if (scale == null || !scale.Components.Any()) return "N/A";

            var matched = scale.Components.FirstOrDefault(g => totalPoints >= g.MinPoints && totalPoints <= g.MaxPoints);
            return matched?.GradeLabel ?? "F";
        }

        public int? CalculateGradeValue(decimal totalPoints, GradeScale scale)
        {
            var matched = scale.Components.FirstOrDefault(g => totalPoints >= g.MinPoints && totalPoints <= g.MaxPoints);
            return matched?.GradeValue ?? 5;
        }

    }
}
using FCSE_Grade_Calculator.Models;
using FCSE_Grade_Calculator.Models.Courses;
using FCSE_Grade_Calculator.Models.ViewModels;
using FCSE_Grade_Calculator.Services;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FCSE_Grade_Calculator.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly GradeCalculationService calc = new GradeCalculationService();

        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();

            var student = db.Students
                            .Include(s => s.EnrolledCourses.Select(e => e.Course.Components))
                            .Include(s => s.EnrolledCourses.Select(e => e.Course.GradeScale.Components))
                            .Include(s => s.EnrolledCourses.Select(e => e.ComponentScores.Select(cs => cs.CourseComponent)))
                            .FirstOrDefault(s => s.UserId == userId);

            if (student == null) return HttpNotFound();

            var enrolledIds = student.EnrolledCourses.Select(e => e.CourseId).ToList();
            var availableCourses = db.Courses.Where(c => !enrolledIds.Contains(c.Id)).ToList();

            var vm = new StudentDashboardViewModel
            {
                Student = student,
                AvailableCourses = availableCourses
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId)
        {
            var userId = User.Identity.GetUserId();
            var student = db.Students.Find(userId);
            if (student == null) return HttpNotFound();

            if (!db.Enrollments.Any(e => e.CourseId == courseId && e.StudentId == userId))
            {
                var enrollment = new Enrollment { CourseId = courseId, StudentId = userId };
                db.Enrollments.Add(enrollment);
                db.SaveChanges();

                var components = db.CourseComponents
                                   .Where(c => c.CourseId == courseId)
                                   .ToList();

                foreach (var comp in components)
                {
                    db.ComponentScores.Add(new ComponentScore
                    {
                        EnrollmentId = enrollment.Id,
                        CourseComponentId = comp.Id,
                        Points = 0m
                    });
                }
                db.SaveChanges();

                var course = db.Courses.Find(courseId);

                return Json(new
                {
                    success = true,
                    courseId = course.Id,
                    acronym = course.Acronym,
                    name = course.Name
                });
            }

            return Json(new { success = false });
        }


        public ActionResult CourseDetails(int id)
        {
            var userId = User.Identity.GetUserId();

            var enrollment = db.Enrollments
                .Include(e => e.Course.Components)
                .Include(e => e.ComponentScores.Select(cs => cs.CourseComponent))
                .Include(e => e.Course.GradeScale.Components)
                .FirstOrDefault(e => e.CourseId == id && e.StudentId == userId);

            return View(enrollment);
        }

        [HttpPost]
        public ActionResult Calculate(int enrollmentId, FormCollection form)
        {
            var enrollment = db.Enrollments
                .Include(e => e.Course.Components)
                .Include(e => e.ComponentScores.Select(cs => cs.CourseComponent))
                .Include(e => e.Course.GradeScale.Components)
                .FirstOrDefault(e => e.Id == enrollmentId);

            if (enrollment == null)
                return Json(new { success = false, message = "Enrollment not found" });

            foreach (var cs in enrollment.ComponentScores)
            {
                var key = "score_" + cs.Id;
                if (form[key] != null && decimal.TryParse(form[key], out decimal pts))
                {
                    cs.Points = pts;
                }
            }

            var total = calc.CalculateTotalPoints(enrollment);
            var label = calc.CalculateGradeLabel(total, enrollment.Course.GradeScale);
            var value = calc.CalculateGradeValue(total, enrollment.Course.GradeScale);

            enrollment.FinalPoints = total;
            enrollment.FinalGradeLabel = label ?? "F";
            enrollment.FinalGradeValue = value ?? 5;

            db.SaveChanges();

            return Json(new { success = true, points = total, label = label, value = value });
        }
    }
}

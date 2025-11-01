using FCSE_Grade_Calculator.Models;
using FCSE_Grade_Calculator.Services;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FCSE_Grade_Calculator.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var teacher = db.Teachers
                            .Include(t => t.AssignedCourses.Select(c => c.Enrollments))
                            .FirstOrDefault(t => t.UserId == userId);

            return View(teacher);
        }

        public ActionResult CourseDetails(int id)
        {
            var course = db.Courses
                           .Include(c => c.Enrollments.Select(e => e.Student))
                           .Include(c => c.Enrollments.Select(e => e.ComponentScores))
                           .Include(c => c.GradeScale.Components)
                           .FirstOrDefault(c => c.Id == id);

            if (course == null) return HttpNotFound();

            return View(course);
        }

        [HttpPost]
        public ActionResult UpdateGrade(int enrollmentId, decimal points, string gradeLabel)
        {
            var enrollment = db.Enrollments
                .Include(e => e.Course.GradeScale.Components)
                .FirstOrDefault(e => e.Id == enrollmentId);

            if (enrollment == null)
                return HttpNotFound();

            if (enrollment.IsGradeConfirmed)
                return new HttpStatusCodeResult(403, "Grade already confirmed");

            var gradeService = new GradeCalculationService();
            enrollment.FinalPoints = points;
            enrollment.FinalGradeLabel = gradeLabel;

            var gradeValue = gradeService.CalculateGradeValue(points, enrollment.Course.GradeScale);
            enrollment.FinalGradeValue = gradeValue ?? 5;

            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult ConfirmGrade(int enrollmentId)
        {
            var enrollment = db.Enrollments.Find(enrollmentId);
            if (enrollment == null) return HttpNotFound();

            enrollment.IsGradeConfirmed = true;
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}

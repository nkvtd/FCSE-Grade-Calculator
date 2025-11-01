using FCSE_Grade_Calculator.Models;
using FCSE_Grade_Calculator.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using FCSE_Grade_Calculator.Models.ViewModels;

namespace FCSE_Grade_Calculator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            var courses = db.Courses
                .Include(c => c.Components)
                .Include(c => c.GradeScale.Components)
                .Include(c => c.Teacher.User)
                .ToList();

            return View(courses);
        }

        [HttpGet]
        public ActionResult CreateCourse()
        {
            var model = new CreateCourseViewModel
            {
                Components = new List<CourseComponentViewModel>
                {
                    new CourseComponentViewModel { Name = "Exam" },
                    new CourseComponentViewModel { Name = "Theoretical Exam" },
                    new CourseComponentViewModel { Name = "Practical Exam" },
                    new CourseComponentViewModel { Name = "Lab Exercises" },
                    new CourseComponentViewModel { Name = "Project" },
                    new CourseComponentViewModel { Name = "Homework" },
                    new CourseComponentViewModel { Name = "Tests" },
                    new CourseComponentViewModel { Name = "Attendance" },
                },
                GradeScale = new List<GradeScaleItemViewModel>
                {
                    new GradeScaleItemViewModel { GradeLabel = "A", GradeValue = 10 },
                    new GradeScaleItemViewModel { GradeLabel = "B", GradeValue = 9 },
                    new GradeScaleItemViewModel { GradeLabel = "C", GradeValue = 8 },
                    new GradeScaleItemViewModel { GradeLabel = "D", GradeValue = 7 },
                    new GradeScaleItemViewModel { GradeLabel = "E", GradeValue = 6 }
                },
                Teachers = db.Teachers
                    .Select(t => new SelectListItem
                    {
                        Value = t.UserId,
                        Text = t.User.FirstName + " " + t.User.LastName
                    })
                    .AsNoTracking()
                    .ToList()
            };

            return View("CreateCourse", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourse(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Teachers = db.Teachers.Include(t => t.User)
                    .Select(t => new SelectListItem
                    {
                        Value = t.UserId,
                        Text = t.User.FirstName + " " + t.User.LastName
                    }).ToList();
                return View("CreateCourse", model);
            }

            var course = new Course
            {
                Name = model.Name,
                Acronym = model.Acronym,
                GradeScale = new GradeScale()
            };

            db.Courses.Add(course);
            db.SaveChanges();

            AssignTeacher(course, model.TeacherId);

            foreach (var comp in model.Components.Where(c => c.Included))
            {
                db.CourseComponents.Add(new CourseComponent
                {
                    CourseId = course.Id,
                    Name = comp.Name,
                    Weight = comp.Weight
                });
            }

            db.SaveChanges();

            foreach (var g in model.GradeScale)
            {
                db.GradeScaleComponents.Add(new GradeScaleComponent
                {
                    GradeScaleId = course.GradeScale.Id,
                    GradeLabel = g.GradeLabel,
                    GradeValue = g.GradeValue,
                    MinPoints = g.MinPoints,
                    MaxPoints = g.MaxPoints
                });
            }

            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public ActionResult EditCourse(int id)
        {
            var course = db.Courses
                .Include(c => c.Components)
                .Include(c => c.GradeScale.Components)
                .Include(c => c.Teacher.User)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return HttpNotFound();

            var model = new CreateCourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Acronym = course.Acronym,
                TeacherId = course.TeacherId,
                TeacherName = course.Teacher?.User?.FullName,
                Components = new List<CourseComponentViewModel>
                {
                    new CourseComponentViewModel { Name = "Exam" },
                    new CourseComponentViewModel { Name = "Theoretical Exam" },
                    new CourseComponentViewModel { Name = "Practical Exam" },
                    new CourseComponentViewModel { Name = "Lab Exercises" },
                    new CourseComponentViewModel { Name = "Project" },
                    new CourseComponentViewModel { Name = "Homework" },
                    new CourseComponentViewModel { Name = "Tests" },
                    new CourseComponentViewModel { Name = "Attendance" },
                    
                },
                GradeScale = course.GradeScale?.Components
                    .Select(gs => new GradeScaleItemViewModel
                    {
                        GradeLabel = gs.GradeLabel,
                        GradeValue = gs.GradeValue,
                        MinPoints = gs.MinPoints,
                        MaxPoints = gs.MaxPoints
                    }).ToList() ?? new List<GradeScaleItemViewModel>(),
                Teachers = db.Teachers
                    .Include(t => t.User)
                    .Select(t => new SelectListItem
                    {
                        Value = t.UserId,
                        Text = t.User.FirstName + " " + t.User.LastName
                    }).ToList()
            };

            foreach (var comp in model.Components)
            {
                var existing = course.Components.FirstOrDefault(c => c.Name == comp.Name);
                if (existing != null)
                {
                    comp.Included = true;
                    comp.Weight = existing.Weight;
                }
            }

            return View("CreateCourse", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(int id, CreateCourseViewModel model)
        {
            var course = db.Courses
                .Include(c => c.Components)
                .Include(c => c.GradeScale.Components)
                //.Include(c => c.Enrollments.Select(e => e.ComponentScores))
                .Include(c => c.Teacher.User)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.Teachers = db.Teachers.Include(t => t.User)
                    .Select(t => new SelectListItem
                    {
                        Value = t.UserId,
                        Text = t.User.FirstName + " " + t.User.LastName
                    }).ToList();

                return View("CreateCourse", model);
            }

            course.Name = model.Name;
            course.Acronym = model.Acronym;

            AssignTeacher(course, model.TeacherId);

            var existingByName = course.Components
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            var includedFromForm = model.Components.Where(cm => cm.Included)
                                                   .Select(cm => cm.Name)
                                                   .ToList();

            foreach (var compVm in model.Components.Where(cm => cm.Included))
            {
                if (existingByName.TryGetValue(compVm.Name, out var existingComp))
                {
                    if (Math.Abs(existingComp.Weight - compVm.Weight) > 1e-6m)
                    {
                        existingComp.Weight = compVm.Weight;
                        db.Entry(existingComp).State = EntityState.Modified;
                    }
                }
                else
                {
                    var newComp = new CourseComponent
                    {
                        CourseId = course.Id,
                        Name = compVm.Name,
                        Weight = compVm.Weight
                    };
                    db.CourseComponents.Add(newComp);
                    course.Components.Add(newComp);
                }
            }

            var toRemove = course.Components
                .Where(c => !includedFromForm.Contains(c.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (toRemove.Any())
            {
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var comp in toRemove)
                        {
                            var scores = db.ComponentScores.Where(s => s.Id == comp.Id).ToList();
                            if (scores.Any())
                                db.ComponentScores.RemoveRange(scores);

                            db.CourseComponents.Remove(comp);
                        }

                        db.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            if (course.GradeScale == null)
            {
                course.GradeScale = new GradeScale { Id = course.Id };
                db.GradeScales.Add(course.GradeScale);
                db.SaveChanges();
            }

            db.GradeScaleComponents.RemoveRange(course.GradeScale.Components);
            db.SaveChanges();

            foreach (var g in model.GradeScale)
            {
                db.GradeScaleComponents.Add(new GradeScaleComponent
                {
                    GradeScaleId = course.GradeScale.Id,
                    GradeLabel = g.GradeLabel,
                    GradeValue = g.GradeValue,
                    MinPoints = g.MinPoints,
                    MaxPoints = g.MaxPoints
                });
            }

            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourse(int id)
        {
            var course = db.Courses
                .Include(c => c.Components)
                .Include(c => c.GradeScale.Components)
                .Include(c => c.Enrollments.Select(e => e.ComponentScores))
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return HttpNotFound();

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var e in course.Enrollments.ToList())
                    {
                        db.ComponentScores.RemoveRange(e.ComponentScores);
                    }

                    db.Enrollments.RemoveRange(course.Enrollments);
                    db.GradeScaleComponents.RemoveRange(course.GradeScale.Components);
                    db.GradeScales.Remove(course.GradeScale);
                    db.CourseComponents.RemoveRange(course.Components);
                    db.Courses.Remove(course);

                    db.SaveChanges();
                    tx.Commit();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return new HttpStatusCodeResult(500, ex.Message);
                }
            }
        }

        private void AssignTeacher(Course course, string teacherId)
        {
            if (!string.IsNullOrEmpty(course.TeacherId) && course.TeacherId != teacherId)
            {
                var oldTeacher = db.Teachers
                    .Include(t => t.AssignedCourses)
                    .FirstOrDefault(t => t.UserId == course.TeacherId);

                if (oldTeacher != null)
                {
                    oldTeacher.AssignedCourses.Remove(course);
                }
            }

            if (!string.IsNullOrEmpty(teacherId))
            {
                var newTeacher = db.Teachers
                    .Include(t => t.AssignedCourses)
                    .FirstOrDefault(t => t.UserId == teacherId);

                if (newTeacher != null && !newTeacher.AssignedCourses.Any(c => c.Id == course.Id))
                {
                    newTeacher.AssignedCourses.Add(course);
                }

                course.TeacherId = teacherId;
            }
            else
            {
                course.TeacherId = null;
            }
        }
    }
}

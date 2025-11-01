# FCSE Grade Calculator

A web-based grade management system that allows students, teachers, and admins to manage courses, enrollments, and grades.

## Features

### Student

- View enrolled courses and points from individual activities
- Calculate final grades
- Enroll in available courses
- Dashboard with course overview

### Teacher

- Manage assigned courses and enrolled students
- Update and confirm grades
- Confirmed grades are read-only for students

### Admin
    
- Create, edit, and delete courses
- Assign or remove teachers
- Manage individual course components and grade weights
	
#### Sample Admin Login

**Email:** `admin@finki.ukim.mk`    
**Password:** `Admin@123`

**IMPORTANT** Change the sample login at `Startup.cs` before the first launch of the app

## Setup

1. Clone the repository:   
`git clone https://github.com/yourusername/FCSE_Grade_Calculator.git`   
2. Open in Visual Studio  
3. Restore NuGet packages 
4. Configure the database connection in `Web.config`    
5. Run the application   

Users are redirected to their role-specific dashboard upon login:   
**Admin:** `/Admin/Dashboard`   
**Teacher:** `/Teacher/Dashboard`   
**Student:** `/Student/Dashboard`   
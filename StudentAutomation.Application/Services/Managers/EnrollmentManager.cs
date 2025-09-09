using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Services.Managers
{
    public class EnrollmentManager : IEnrollmentService
    {
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly IStudentDal _studentDal;
        private readonly ICourseDal _courseDal;

        public EnrollmentManager(IEnrollmentDal enrollmentDal, IStudentDal studentDal, ICourseDal courseDal)
        {
            _enrollmentDal = enrollmentDal;
            _studentDal = studentDal;
            _courseDal = courseDal;
        }

        public async Task<IResult> EnrollAsync(int studentId, int courseId, DateTime? enrolledAt = null)
        {
            var student = await _studentDal.GetByIdAsync(studentId);
            if (student == null) return new ErrorResult("Öğrenci bulunamadı.");

            if (!await _courseDal.IsActiveAsync(courseId))
                return new ErrorResult("Aktif ders bulunamadı.");

            var exist = await _enrollmentDal.GetAsync(studentId, courseId);
            if (exist != null) return new ErrorResult("Öğrenci zaten bu derse kayıtlı.");

            await _enrollmentDal.AddAsync(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = enrolledAt ?? DateTime.UtcNow
            });

            return new SuccessResult("Öğrenci derse kaydedildi.");
        }

        public async Task<IResult> UnenrollAsync(int studentId, int courseId)
        {
            var exist = await _enrollmentDal.GetAsync(studentId, courseId);
            if (exist == null) return new ErrorResult("Kayıt bulunamadı.");

            await _enrollmentDal.DeleteAsync(exist);
            return new SuccessResult("Öğrenci dersten çıkarıldı.");
        }

        public async Task<IDataResult<List<CourseMiniDto>>> GetCoursesOfStudentAsync(int studentId)
        {
            var courses = await _enrollmentDal.GetCoursesOfStudentAsync(studentId);
            var minis = courses.Select(c => new CourseMiniDto { Id = c.Id, Code = c.Code, Name = c.Name }).ToList();
            return new SuccessDataResult<List<CourseMiniDto>>(minis);
        }

        public async Task<IDataResult<List<StudentMiniDto>>> GetStudentsOfCourseAsync(int courseId)
        {
            var students = await _enrollmentDal.GetStudentsOfCourseAsync(courseId);
            var minis = students.Select(s => new StudentMiniDto
            {
                Id = s.Id,
                StudentNumber = s.StudentNumber,
                FullName = s.User.FirstName + " " + s.User.LastName
            }).ToList();

            return new SuccessDataResult<List<StudentMiniDto>>(minis);
        }
    }
}

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

        public async Task<IResult> EnrollAsync(int studentId, int courseId, DateOnly? enrolledAt = null)
        {
            var student = await _studentDal.GetByIdAsync(studentId);
            if (student == null) return new ErrorResult("Öğrenci bulunamadı.");

            var course = await _courseDal.GetByIdAsync(courseId);
            if (course is null) return new ErrorResult("Ders bulunamadı.");
            if (!course.IsActive) return new ErrorResult("Ders aktif değil.");

            var exist = await _enrollmentDal.GetAsync(studentId, courseId);
            if (exist != null) return new ErrorResult("Öğrenci zaten bu derse kayıtlı.");

            var date = enrolledAt ?? DateOnly.FromDateTime(DateTime.Today); // ← varsayılan bugün

            await _enrollmentDal.AddAsync(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = date
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

        // PROJECTION DAL'da: NRE ve gereksiz Include maliyeti yok.
        public async Task<IDataResult<List<StudentMiniDto>>> GetStudentsOfCourseAsync(int courseId)
        {
            var minis = await _enrollmentDal.GetStudentMinisOfCourseAsync(courseId);
            return new SuccessDataResult<List<StudentMiniDto>>(minis);
        }
    }
}
    


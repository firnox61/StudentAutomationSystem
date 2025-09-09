using AutoMapper;
using StudentAutomation.Application.DTOs.Attendances;
using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Application.DTOs.Grades;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.DTOs.Teachers;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudentAutomation.Application.MappingProfiles
{
    public class GeneralMapping : Profile
    {


        public GeneralMapping()
        {

            CreateMap<UserCreateDto, User>()
    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
    .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());

            CreateMap<User, UserForLoginDto>().ReverseMap();
            CreateMap<User, UserForRegisterDto>().ReverseMap();
            CreateMap<User, UserListDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();


            CreateMap<OperationClaim, OperationClaimCreateDto>().ReverseMap();
            CreateMap<OperationClaim, OperationClaimListDto>().ReverseMap();
            CreateMap<OperationClaim, OperationClaimUpdateDto>().ReverseMap();



            CreateMap<UserOperationClaim, UserOperationClaimCreateDto>().ReverseMap();
            CreateMap<UserOperationClaim, UserOperationClaimListDto>().ReverseMap();
            // Student
            CreateMap<StudentCreateDto, Student>();
            CreateMap<StudentUpdateDto, Student>();
            CreateMap<Student, StudentListDto>()
                .ForMember(d => d.FirstName, m => m.MapFrom(s => s.User.FirstName))
                .ForMember(d => d.LastName, m => m.MapFrom(s => s.User.LastName))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.User.Email));
            CreateMap<Student, StudentDetailDto>()
                .IncludeBase<Student, StudentListDto>();
            CreateMap<Student, StudentMiniDto>()
                .ForMember(d => d.FullName, m => m.MapFrom(s => s.User.FirstName + " " + s.User.LastName));

            // Teacher
            CreateMap<TeacherCreateDto, Teacher>();
            CreateMap<TeacherUpdateDto, Teacher>();
            CreateMap<Teacher, TeacherListDto>()
                .ForMember(d => d.FirstName, m => m.MapFrom(s => s.User.FirstName))
                .ForMember(d => d.LastName, m => m.MapFrom(s => s.User.LastName))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.User.Email));
            CreateMap<Teacher, TeacherDetailDto>()
                .IncludeBase<Teacher, TeacherListDto>();
            CreateMap<StudentFeedback, StudentFeedbackListDto>()
               .ForMember(d => d.TeacherFullName,
                   m => m.MapFrom(s => s.Teacher.User.FirstName + " " + s.Teacher.User.LastName));

            CreateMap<StudentFeedbackCreateDto, StudentFeedback>();
            // Course
            CreateMap<CourseCreateDto, Course>();
            CreateMap<CourseUpdateDto, Course>();
            CreateMap<Course, CourseListDto>()
                 .ForMember(d => d.TeacherFullName,
                     m => m.MapFrom(s => s.Teacher != null
                         ? (s.Teacher.User.FirstName + " " + s.Teacher.User.LastName)
                         : null))
                 .ForMember(d => d.EnrollmentCount,
                     m => m.MapFrom(s => s.Enrollments.Count));
            CreateMap<Course, CourseDetailDto>()
                .IncludeBase<Course, CourseListDto>()
                .ForMember(d => d.Students, m => m.MapFrom(c => c.Enrollments.Select(e => e.Student)));

            CreateMap<Course, CourseMiniDto>();

            // Grade
            CreateMap<GradeUpsertDto, Grade>();
            CreateMap<Grade, GradeListDto>()
                .ForMember(d => d.StudentFullName, m => m.MapFrom(g => g.Student.User.FirstName + " " + g.Student.User.LastName))
                .ForMember(d => d.CourseCode, m => m.MapFrom(g => g.Course.Code))
                .ForMember(d => d.CourseName, m => m.MapFrom(g => g.Course.Name));
            CreateMap<Grade, GradeDetailDto>().IncludeBase<Grade, GradeListDto>();

            // Attendance
            CreateMap<AttendanceUpsertDto, Attendance>();
            CreateMap<Attendance, AttendanceListDto>()
                .ForMember(d => d.StudentFullName, m => m.MapFrom(a => a.Student.User.FirstName + " " + a.Student.User.LastName))
                .ForMember(d => d.CourseCode, m => m.MapFrom(a => a.Course.Code))
                .ForMember(d => d.CourseName, m => m.MapFrom(a => a.Course.Name));
            CreateMap<Attendance, AttendanceDetailDto>().IncludeBase<Attendance, AttendanceListDto>();
        }
    }
}

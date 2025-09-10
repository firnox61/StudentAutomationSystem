using StudentAutomation.Blazor.Models;

namespace StudentAutomation.Blazor.Services
{
    public interface IApiClient
    {
        Task<IReadOnlyList<StudentListDto>> GetStudentsAsync();
        Task<StudentDetailDto?> GetStudentAsync(int id);
        // Teachers
        Task<IReadOnlyList<TeacherListDto>> GetTeachersAsync();
        Task<TeacherDetailDto?> GetTeacherAsync(int id);

        // Courses
        Task<IReadOnlyList<CourseListDto>> GetCoursesAsync();
        Task<CourseDetailDto?> GetCourseAsync(int id);



        // --- Dashboard counts ---
        Task<int> GetStudentCountAsync();
        Task<int> GetTeacherCountAsync();
        Task<int> GetCourseCountAsync();
        // ileride:
        
    }
}

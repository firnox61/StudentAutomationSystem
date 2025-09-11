using AutoMapper;
using StudentAutomation.Application.Abstraction;
using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Core.Aspects.Security;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Application.Services.Managers
{
    public class StudentFeedbackManager : IStudentFeedbackService
    {
        private readonly ICurrentUser _currentUser;

        private readonly IStudentFeedbackDal _feedbackDal;
        private readonly ITeacherDal _teacherDal;
        private readonly ICourseDal _courseDal;
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly IMapper _mapper;


        public StudentFeedbackManager(
            ICurrentUser currentUser,
            ITeacherDal teacherDal,
            ICourseDal courseDal,
            IEnrollmentDal enrollmentDal,
            IStudentFeedbackDal feedbackDal,
            IMapper mapper)
        {
            _currentUser = currentUser;
            _teacherDal = teacherDal;
            _courseDal = courseDal;
            _enrollmentDal = enrollmentDal;
            _feedbackDal = feedbackDal;
            _mapper = mapper;
        }
        [SecuredOperation("Admin,Teacher")]
        public async Task<IDataResult<List<StudentFeedbackListDto>>> GetByCourseAsync(int courseId)
        {
            var list = await _feedbackDal.GetByCourseAsync(courseId); // entity list
            var dto = _mapper.Map<List<StudentFeedbackListDto>>(list);
            return new SuccessDataResult<List<StudentFeedbackListDto>>(dto);
        }
        [SecuredOperation("Admin,Teacher")]
        public async Task<IDataResult<List<StudentFeedbackListDto>>> GetByStudentAsync(int studentId, int? courseId = null)
        {
            var list = await _feedbackDal.GetByStudentAsync(studentId, courseId); // entity list
            var dto = _mapper.Map<List<StudentFeedbackListDto>>(list);
            return new SuccessDataResult<List<StudentFeedbackListDto>>(dto);
        }
        [SecuredOperation("Admin,Teacher")]
        public async Task<IResult> CreateAsync(StudentFeedbackCreateDto dto)
        {
            if (dto is null) return new ErrorResult("Geçersiz istek.");
            dto.Comment = dto.Comment?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return new ErrorResult("Yorum boş olamaz.");

            // ↓↓↓ Kritikti
            var userId = _currentUser.UserId;
            if (userId is null)
                return new ErrorResult("Kullanıcı kimliği bulunamadı.");

            // Öğretmen kimliği
            var teacher = await _teacherDal.GetAsync(t => t.UserId == userId.Value);
            if (teacher is null)
                return new ErrorResult("Öğretmen bulunamadı.");

            // Ders doğrulama ve sahiplik
            var course = await _courseDal.GetByIdAsync(dto.CourseId);
            if (course is null)
                return new ErrorResult("Ders bulunamadı.");
            if (course.TeacherId != teacher.Id)
                return new ErrorResult("Sadece atandığınız derste yorum yapabilirsiniz.");

            // Öğrenci derse kayıtlı mı?
            var enrollment = await _enrollmentDal.GetAsync(dto.StudentId, dto.CourseId);
            if (enrollment is null)
                return new ErrorResult("Öğrenci bu derse kayıtlı değil.");

            // Kayıt
            var entity = _mapper.Map<StudentFeedback>(dto);
            entity.TeacherId = teacher.Id;
            entity.CreatedAt = DateTime.UtcNow;

            await _feedbackDal.AddAsync(entity);
            return new SuccessResult("Yorum eklendi.");
        }

        [SecuredOperation("Admin,Teacher")]
        public async Task<IResult> UpdateAsync(int userId, StudentFeedbackUpdateDto dto)
        {
            if (dto is null) return new ErrorResult("Geçersiz istek.");
            dto.Comment = dto.Comment?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return new ErrorResult("Yorum boş olamaz.");

            var fb = await _feedbackDal.GetAsync(x => x.Id == dto.Id);
            if (fb is null)
                return new ErrorResult("Yorum bulunamadı.");

            var teacher = await _teacherDal.GetAsync(t => t.UserId == userId);
            if (teacher is null || fb.TeacherId != teacher.Id)
                return new ErrorResult("Sadece kendi yorumunuzu güncelleyebilirsiniz.");

            fb.Comment = dto.Comment;
            fb.UpdatedAt = DateTime.UtcNow;

            await _feedbackDal.UpdateAsync(fb);
            return new SuccessResult("Yorum güncellendi.");
        }
        [SecuredOperation("Admin,Teacher")]
        public async Task<IResult> DeleteAsync(int userId, int id)
        {
            var fb = await _feedbackDal.GetAsync(x => x.Id == id);
            if (fb is null)
                return new ErrorResult("Yorum bulunamadı.");

            var teacher = await _teacherDal.GetAsync(t => t.UserId == userId);
            if (teacher is null || fb.TeacherId != teacher.Id)
                return new ErrorResult("Sadece kendi yorumunuzu silebilirsiniz.");

            await _feedbackDal.DeleteAsync(fb);
            return new SuccessResult("Yorum silindi.");
        }
    }
}

using AutoMapper;
using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Application.Services.Managers
{
    public class StudentFeedbackManager : IStudentFeedbackService
    {
        private readonly IStudentFeedbackDal _feedbackDal;
        private readonly ITeacherDal _teacherDal;
        private readonly ICourseDal _courseDal;
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly IMapper _mapper;

        public StudentFeedbackManager(
            IStudentFeedbackDal feedbackDal,
            ITeacherDal teacherDal,
            ICourseDal courseDal,
            IEnrollmentDal enrollmentDal,
            IMapper mapper)
        {
            _feedbackDal = feedbackDal;
            _teacherDal = teacherDal;
            _courseDal = courseDal;
            _enrollmentDal = enrollmentDal;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<StudentFeedbackListDto>>> GetByCourseAsync(int courseId)
        {
            var list = await _feedbackDal.GetByCourseAsync(courseId); // entity list
            var dto = _mapper.Map<List<StudentFeedbackListDto>>(list);
            return new SuccessDataResult<List<StudentFeedbackListDto>>(dto);
        }

        public async Task<IDataResult<List<StudentFeedbackListDto>>> GetByStudentAsync(int studentId, int? courseId = null)
        {
            var list = await _feedbackDal.GetByStudentAsync(studentId, courseId); // entity list
            var dto = _mapper.Map<List<StudentFeedbackListDto>>(list);
            return new SuccessDataResult<List<StudentFeedbackListDto>>(dto);
        }

        public async Task<IResult> CreateAsync(int userId, StudentFeedbackCreateDto dto)
        {
            // 1) Alan doğrulama
            if (dto is null) return new ErrorResult("Geçersiz istek.");
            dto.Comment = dto.Comment?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return new ErrorResult("Yorum boş olamaz.");

            // 2) Öğretmen kimliği (userId → Teacher)
            var teacher = await _teacherDal.GetAsync(t => t.UserId == userId);
            if (teacher is null)
                return new ErrorResult("Öğretmen bulunamadı.");

            // 3) Ders doğrulama ve sahiplik
            var course = await _courseDal.GetByIdAsync(dto.CourseId);
            if (course is null)
                return new ErrorResult("Ders bulunamadı.");
            if (course.TeacherId != teacher.Id)
                return new ErrorResult("Sadece atandığınız derste yorum yapabilirsiniz.");

            // 4) Öğrenci derse kayıtlı mı?
            var enrollment = await _enrollmentDal.GetAsync(dto.StudentId, dto.CourseId);
            if (enrollment is null)
                return new ErrorResult("Öğrenci bu derse kayıtlı değil.");

            // 5) Kayıt
            var entity = _mapper.Map<StudentFeedback>(dto);
            entity.TeacherId = teacher.Id;
            entity.CreatedAt = DateTime.UtcNow;

            await _feedbackDal.AddAsync(entity);
            return new SuccessResult("Yorum eklendi.");
        }

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

using AutoMapper;
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
    public class CourseManager : ICourseService
    {
        private readonly ICourseDal _courseDal;
        private readonly ITeacherDal _teacherDal;
        private readonly IMapper _mapper;

        public CourseManager(ICourseDal courseDal, ITeacherDal teacherDal, IMapper mapper)
        {
            _courseDal = courseDal;
            _teacherDal = teacherDal;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<CourseListDto>>> GetAllAsync(bool onlyActive = true)
        {
            var list = await _courseDal.GetAllWithTeacherAsync(onlyActive);
            return new SuccessDataResult<List<CourseListDto>>(_mapper.Map<List<CourseListDto>>(list));
        }

        public async Task<IDataResult<CourseDetailDto>> GetByIdAsync(int id)
        {
            var entity = await _courseDal.GetByIdFullAsync(id);
            if (entity == null) return new ErrorDataResult<CourseDetailDto>("Ders bulunamadı.");
            return new SuccessDataResult<CourseDetailDto>(_mapper.Map<CourseDetailDto>(entity));
        }

        public async Task<IResult> AddAsync(CourseCreateDto dto)
        {
            if (await _courseDal.ExistsByCodeAsync(dto.Code))
                return new ErrorResult("Bu ders kodu zaten kayıtlı.");

            var entity = _mapper.Map<Course>(dto);
            await _courseDal.AddAsync(entity);
            return new SuccessResult("Ders eklendi.");
        }

        public async Task<IResult> UpdateAsync(CourseUpdateDto dto)
        {
            var entity = await _courseDal.GetByIdAsync(dto.Id);
            if (entity == null) return new ErrorResult("Ders bulunamadı.");

            if (await _courseDal.ExistsByCodeAsync(dto.Code, excludeId: dto.Id))
                return new ErrorResult("Bu ders kodu başka bir kayıtta var.");

            _mapper.Map(dto, entity);
            await _courseDal.UpdateAsync(entity);
            return new SuccessResult("Ders güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _courseDal.GetByIdAsync(id);
            if (entity == null) return new ErrorResult("Ders bulunamadı.");
            await _courseDal.DeleteAsync(entity);
            return new SuccessResult("Ders silindi.");
        }

        public async Task<IResult> AssignTeacherAsync(int courseId, int? teacherId)
        {
            var entity = await _courseDal.GetByIdAsync(courseId);
            if (entity == null) return new ErrorResult("Ders bulunamadı.");
            entity.TeacherId = teacherId;
            await _courseDal.UpdateAsync(entity);
            return new SuccessResult("Dersin öğretmeni güncellendi.");
        }

        public async Task<IResult> SetActiveAsync(int courseId, bool isActive)
        {
            var entity = await _courseDal.GetByIdAsync(courseId);
            if (entity == null) return new ErrorResult("Ders bulunamadı.");
            entity.IsActive = isActive;
            await _courseDal.UpdateAsync(entity);
            return new SuccessResult(isActive ? "Ders aktif edildi." : "Ders pasif edildi.");
        }

        public async Task<IDataResult<List<StudentMiniDto>>> GetStudentsAsync(int courseId)
        {
            var students = await _courseDal.GetStudentsOfCourseAsync(courseId);
            return new SuccessDataResult<List<StudentMiniDto>>(_mapper.Map<List<StudentMiniDto>>(students));
        }
        public async Task<IDataResult<List<CourseListDto>>> GetByTeacherAsync(int teacherId)
        {
            var list = await _courseDal.GetByTeacherAsync(teacherId);
            var dto = _mapper.Map<List<CourseListDto>>(list);
            return new SuccessDataResult<List<CourseListDto>>(dto);
        }

        public async Task<IDataResult<List<CourseListDto>>> GetMineAsync(int userId)
        {
            var teacher = await _teacherDal.GetAsync(t => t.UserId == userId);
            if (teacher is null)
                return new ErrorDataResult<List<CourseListDto>>("Teacher not found for current user.");

            var list = await _courseDal.GetByTeacherAsync(teacher.Id);
            var dto = _mapper.Map<List<CourseListDto>>(list);
            return new SuccessDataResult<List<CourseListDto>>(dto);
        }
    }
}

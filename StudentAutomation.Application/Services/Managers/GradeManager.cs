using AutoMapper;
using StudentAutomation.Application.DTOs.Grades;
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
    public class GradeManager : IGradeService
    {
        private readonly IGradeDal _gradeDal;
        private readonly IMapper _mapper;

        public GradeManager(IGradeDal gradeDal, IMapper mapper)
        {
            _gradeDal = gradeDal;
            _mapper = mapper;
        }

        public async Task<IResult> UpsertAsync(GradeUpsertDto dto)
        {
            var exist = await _gradeDal.GetByKeysAsync(dto.StudentId, dto.CourseId, dto.Type);

            if (exist == null)
            {
                var entity = _mapper.Map<Grade>(dto);
                entity.CreatedAt = DateTime.UtcNow;
                await _gradeDal.AddAsync(entity);
                return new SuccessResult("Not eklendi.");
            }

            exist.Value = dto.Value;
            exist.Term = dto.Term;
            await _gradeDal.UpdateAsync(exist);
            return new SuccessResult("Not güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _gradeDal.GetAsync(g => g.Id == id); // base method; predicate basit
            if (entity == null) return new ErrorResult("Not kaydı bulunamadı.");
            await _gradeDal.DeleteAsync(entity);
            return new SuccessResult("Not silindi.");
        }

        public async Task<IDataResult<List<GradeListDto>>> GetByStudentAsync(int studentId, int? courseId = null)
        {
            var list = await _gradeDal.GetByStudentAsync(studentId, courseId);
            return new SuccessDataResult<List<GradeListDto>>(_mapper.Map<List<GradeListDto>>(list));
        }

        public async Task<IDataResult<List<GradeListDto>>> GetByCourseAsync(int courseId)
        {
            var list = await _gradeDal.GetByCourseAsync(courseId);
            return new SuccessDataResult<List<GradeListDto>>(_mapper.Map<List<GradeListDto>>(list));
        }

        public async Task<IDataResult<GradeDetailDto>> GetByIdAsync(int id)
        {
            var entity = await _gradeDal.GetByIdDetailAsync(id);
            if (entity == null) return new ErrorDataResult<GradeDetailDto>("Not kaydı bulunamadı.");
            return new SuccessDataResult<GradeDetailDto>(_mapper.Map<GradeDetailDto>(entity));
        }
    }
}

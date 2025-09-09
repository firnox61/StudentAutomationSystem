using AutoMapper;
using StudentAutomation.Application.DTOs.Teachers;
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
    public class TeacherManager : ITeacherService
    {
        private readonly ITeacherDal _teacherDal;
        private readonly IMapper _mapper;

        public TeacherManager(ITeacherDal teacherDal, IMapper mapper)
        {
            _teacherDal = teacherDal;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<TeacherListDto>>> GetAllAsync()
        {
            var list = await _teacherDal.GetAllWithUserAsync();
            return new SuccessDataResult<List<TeacherListDto>>(_mapper.Map<List<TeacherListDto>>(list));
        }

        public async Task<IDataResult<TeacherDetailDto>> GetByIdAsync(int id)
        {
            var entity = await _teacherDal.GetByIdWithUserAsync(id);
            if (entity == null) return new ErrorDataResult<TeacherDetailDto>("Öğretmen bulunamadı.");
            return new SuccessDataResult<TeacherDetailDto>(_mapper.Map<TeacherDetailDto>(entity));
        }

        public async Task<IDataResult<TeacherDetailDto>> GetByUserIdAsync(int userId)
        {
            var entity = await _teacherDal.GetByUserIdWithUserAsync(userId);
            if (entity == null) return new ErrorDataResult<TeacherDetailDto>("Öğretmen bulunamadı.");
            return new SuccessDataResult<TeacherDetailDto>(_mapper.Map<TeacherDetailDto>(entity));
        }

        public async Task<IResult> AddAsync(TeacherCreateDto dto)
        {
            var entity = _mapper.Map<Teacher>(dto);
            await _teacherDal.AddAsync(entity);
            return new SuccessResult("Öğretmen eklendi.");
        }

        public async Task<IResult> UpdateAsync(TeacherUpdateDto dto)
        {
            var entity = await _teacherDal.GetByIdAsync(dto.Id);
            if (entity == null) return new ErrorResult("Öğretmen bulunamadı.");

            _mapper.Map(dto, entity);
            await _teacherDal.UpdateAsync(entity);
            return new SuccessResult("Öğretmen güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _teacherDal.GetByIdAsync(id);
            if (entity == null) return new ErrorResult("Öğretmen bulunamadı.");
            await _teacherDal.DeleteAsync(entity);
            return new SuccessResult("Öğretmen silindi.");
        }
    }
}

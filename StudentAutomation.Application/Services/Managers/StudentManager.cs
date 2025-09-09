using AutoMapper;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Core.Aspects.Security;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Services.Managers
{
    public class StudentManager : IStudentService
    {
        private readonly IStudentDal _studentDal;
        private readonly IMapper _mapper;

        public StudentManager(IStudentDal studentDal, IMapper mapper)
        {
            _studentDal = studentDal;
            _mapper = mapper;
        }
        [SecuredOperation("Admin,Teacher")]
        public async Task<IDataResult<List<StudentListDto>>> GetAllAsync()
        {
            var list = await _studentDal.GetAllWithUserAsync();
            return new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(list));
        }

        public async Task<IDataResult<StudentDetailDto>> GetByIdAsync(int id)
        {
            var entity = await _studentDal.GetByIdWithUserAsync(id);
            if (entity == null) return new ErrorDataResult<StudentDetailDto>("Öğrenci bulunamadı.");
            return new SuccessDataResult<StudentDetailDto>(_mapper.Map<StudentDetailDto>(entity));
        }

        public async Task<IDataResult<StudentDetailDto>> GetByUserIdAsync(int userId)
        {
            var entity = await _studentDal.GetByUserIdWithUserAsync(userId);
            if (entity == null) return new ErrorDataResult<StudentDetailDto>("Öğrenci bulunamadı.");
            return new SuccessDataResult<StudentDetailDto>(_mapper.Map<StudentDetailDto>(entity));
        }

        public async Task<IResult> AddAsync(StudentCreateDto dto)
        {
            if (await _studentDal.ExistsByStudentNumberAsync(dto.StudentNumber))
                return new ErrorResult("Bu öğrenci numarası zaten kayıtlı.");

            var entity = _mapper.Map<Student>(dto);
            await _studentDal.AddAsync(entity);
            return new SuccessResult("Öğrenci eklendi.");
        }

        public async Task<IResult> UpdateAsync(StudentUpdateDto dto)
        {
            var entity = await _studentDal.GetByIdAsync(dto.Id);
            if (entity == null) return new ErrorResult("Öğrenci bulunamadı.");

            if (await _studentDal.ExistsByStudentNumberAsync(dto.StudentNumber, excludeId: dto.Id))
                return new ErrorResult("Bu öğrenci numarası başka bir kayıtta var.");

            _mapper.Map(dto, entity);
            await _studentDal.UpdateAsync(entity);
            return new SuccessResult("Öğrenci güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _studentDal.GetByIdAsync(id);
            if (entity == null) return new ErrorResult("Öğrenci bulunamadı.");
            await _studentDal.DeleteAsync(entity);
            return new SuccessResult("Öğrenci silindi.");
        }
    }
}

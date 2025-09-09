using AutoMapper;
using StudentAutomation.Application.DTOs.Attendances;
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
    public class AttendanceManager : IAttendanceService
    {
        private readonly IAttendanceDal _attendanceDal;
        private readonly IMapper _mapper;

        public AttendanceManager(IAttendanceDal attendanceDal, IMapper mapper)
        {
            _attendanceDal = attendanceDal;
            _mapper = mapper;
        }

        public async Task<IResult> UpsertAsync(AttendanceUpsertDto dto)
        {
            var exist = await _attendanceDal.GetByKeysAsync(dto.StudentId, dto.CourseId, dto.Date);

            if (exist == null)
            {
                var entity = _mapper.Map<Attendance>(dto);
                await _attendanceDal.AddAsync(entity);
                return new SuccessResult("Devamsızlık kaydı eklendi.");
            }

            exist.Status = dto.Status;
            exist.Week = dto.Week;
            exist.Note = dto.Note;
            await _attendanceDal.UpdateAsync(exist);
            return new SuccessResult("Devamsızlık kaydı güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _attendanceDal.GetAsync(a => a.Id == id); // basit predicate
            if (entity == null) return new ErrorResult("Devamsızlık kaydı bulunamadı.");
            await _attendanceDal.DeleteAsync(entity);
            return new SuccessResult("Devamsızlık kaydı silindi.");
        }

        public async Task<IDataResult<List<AttendanceListDto>>> GetByStudentAsync(int studentId, int? courseId = null)
        {
            var list = await _attendanceDal.GetByStudentAsync(studentId, courseId);
            return new SuccessDataResult<List<AttendanceListDto>>(_mapper.Map<List<AttendanceListDto>>(list));
        }

        public async Task<IDataResult<List<AttendanceListDto>>> GetByCourseAsync(int courseId, DateOnly? date = null)
        {
            var list = await _attendanceDal.GetByCourseAsync(courseId, date);
            return new SuccessDataResult<List<AttendanceListDto>>(_mapper.Map<List<AttendanceListDto>>(list));
        }

        public async Task<IDataResult<AttendanceDetailDto>> GetByIdAsync(int id)
        {
            var entity = await _attendanceDal.GetByIdDetailAsync(id);
            if (entity == null) return new ErrorDataResult<AttendanceDetailDto>("Devamsızlık kaydı bulunamadı.");
            return new SuccessDataResult<AttendanceDetailDto>(_mapper.Map<AttendanceDetailDto>(entity));
        }
    }
}

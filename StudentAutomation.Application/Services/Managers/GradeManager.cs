using AutoMapper;
using StudentAutomation.Application.DTOs.Courses;
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
        private readonly IStudentDal _studentDal;

        public GradeManager(IGradeDal gradeDal, IMapper mapper, IStudentDal studentDal)
        {
            _gradeDal = gradeDal;
            _mapper = mapper;
            _studentDal = studentDal;
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
        public async Task<IDataResult<StudentGradeAverageDto>> AverageByStudentAsync(int studentId, string? term = null)
        {
            // Öğrenci basic info
            var student = await _studentDal.GetByIdAsync(studentId);
            if (student is null) return new ErrorDataResult<StudentGradeAverageDto>("Öğrenci bulunamadı.");

            var grades = await _gradeDal.GetByStudentAsync(studentId, term);

            // Ders bazında grupla
            var perCourse = grades
                .GroupBy(g => new { g.CourseId, g.Course.Code, g.Course.Name })
                .Select(g =>
                {
                    decimal? mid = g.FirstOrDefault(x => x.Type == GradeType.Midterm)?.Value;
                    decimal? fin = g.FirstOrDefault(x => x.Type == GradeType.Final)?.Value;
                    decimal? mk = g.FirstOrDefault(x => x.Type == GradeType.Makeup)?.Value;

                    // Makeup varsa final yerine geçer
                    var finalEff = mk ?? fin;

                    // Eksik not varsa 0 kabul (istersen parametreleştiririz)
                    var midEff = mid ?? 0m;
                    var finEff = finalEff ?? 0m;

                    var weighted = midEff * 0.40m + finEff * 0.60m;

                    return new CourseBreakdownDto
                    {
                        CourseId = g.Key.CourseId,
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        Midterm = mid,
                        Final = fin,
                        Makeup = mk,
                        Weighted = Math.Round(weighted, 2)
                    };
                })
                .ToList();

            var avg = perCourse.Any() ? Math.Round(perCourse.Average(x => x.Weighted), 2) : 0m;

            var dto = new StudentGradeAverageDto
            {
                StudentId = studentId,
                StudentFullName = student.User.FirstName + " " + student.User.LastName,
                Term = term,
                Average = avg,
                Courses = perCourse
            };

            return new SuccessDataResult<StudentGradeAverageDto>(dto);
        }

        public async Task<IDataResult<List<GradeListDto>>> GetAllAsync()
        {
            var list = await _gradeDal.GetAllDetailAsync(); // ⬅⬅⬅ önemli
            var dto = _mapper.Map<List<GradeListDto>>(list);
            return new SuccessDataResult<List<GradeListDto>>(dto);
        }
    }//return new SuccessDataResult<List<TeacherListDto>>(_mapper.Map<List<TeacherListDto>>(list));
}

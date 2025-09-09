// /tests/StudentAutomation.Tests.Unit/Courses/CourseManager_Tests.cs
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.Services.Managers;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using Xunit;

namespace StudentAutomation.Tests.Unit.Courses
{
    public class CourseManager_Tests
    {
        private readonly Mock<ICourseDal> _courseDal = new();
        private readonly Mock<ITeacherDal> _teacherDal = new();
        private readonly Mock<IMapper> _mapper = new();

        private CourseManager CreateSut() => new(_courseDal.Object, _teacherDal.Object, _mapper.Object);

        [Fact]
        public async Task AddAsync_Should_Return_Error_When_Code_Exists()
        {
            // Arrange
            var dto = new CourseCreateDto { Code = "SE101", Name = "Intro SE" };

            // Opsiyonel parametre (excludeId) için null’ı açıkça veriyoruz
            _courseDal
                .Setup(d => d.ExistsByCodeAsync("SE101", null))
                .ReturnsAsync(true);

            var sut = CreateSut();

            // Act
            var res = await sut.AddAsync(dto);

            // Assert
            res.Success.Should().BeFalse();
            res.Message.Should().Be("Bu ders kodu zaten kayıtlı.");
            _courseDal.Verify(d => d.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task GetMineAsync_Should_Return_Error_When_Teacher_Not_Found_For_User()
        {
            // Arrange
            _teacherDal
                .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Teacher, bool>>>()))
                .ReturnsAsync((Teacher)null!);

            var sut = CreateSut();

            // Act
            var res = await sut.GetMineAsync(userId: 123);

            // Assert
            res.Success.Should().BeFalse();
            res.Message.Should().Be("Teacher not found for current user.");
            _courseDal.Verify(d => d.GetByTeacherAsync(It.IsAny<int>()), Times.Never);
        }
    }
}

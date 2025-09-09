using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using StudentAutomation.Application.DTOs.Teachers;
using StudentAutomation.Application.Services.Managers;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using Xunit;

public class TeacherManager_Add_Tests
{
    private readonly Mock<ITeacherDal> _teacherDal = new();
    private readonly Mock<IUserDal> _userDal = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task AddAsync_Should_Return_Error_When_User_Not_Found()
    {
        var dto = new TeacherCreateDto { UserId = 77, Title = "Dr." };

        _userDal.Setup(d => d.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null!);

        var sut = new TeacherManager(_teacherDal.Object, _mapper.Object, _userDal.Object);

        var res = await sut.AddAsync(dto);

        res.Success.Should().BeFalse();
        res.Message.Should().Be("Kullanıcı bulunamadı.");
        _teacherDal.Verify(d => d.AddAsync(It.IsAny<Teacher>()), Times.Never);
    }
    [Fact]
    public async Task DeleteAsync_Should_Delete_When_Found()
    {
        var teacher = new Teacher { Id = 9 };
        _teacherDal.Setup(d => d.GetByIdAsync(9)).ReturnsAsync(teacher);

        var sut = new TeacherManager(_teacherDal.Object, _mapper.Object, _userDal.Object);

        var res = await sut.DeleteAsync(9);

        res.Success.Should().BeTrue();
        res.Message.Should().Be("Öğretmen silindi.");
        _teacherDal.Verify(d => d.DeleteAsync(teacher), Times.Once);
    }
}

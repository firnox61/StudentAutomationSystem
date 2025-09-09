using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.Services.Managers;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using Xunit;

public class StudentManager_Add_Tests
{
    private readonly Mock<IStudentDal> _studentDal = new();
    private readonly Mock<IUserDal> _userDal = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task AddAsync_Should_Return_Error_When_StudentNumber_Exists()
    {
        var dto = new StudentCreateDto { UserId = 10, StudentNumber = "2025001" };

        _studentDal
      .Setup(d => d.ExistsByStudentNumberAsync("2025001", null))
      .ReturnsAsync(true);


        var sut = new StudentManager(_studentDal.Object, _mapper.Object, _userDal.Object);

        var res = await sut.AddAsync(dto);

        res.Success.Should().BeFalse();
        res.Message.Should().Be("Bu öğrenci numarası zaten kayıtlı.");
        _studentDal.Verify(d => d.AddAsync(It.IsAny<Student>()), Times.Never);
    }
}

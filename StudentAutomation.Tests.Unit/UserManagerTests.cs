using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Application.Services.Managers;
using StudentAutomation.Core.Security;
using StudentAutomation.Domain.Entities;
using Xunit;

namespace StudentAutomation.Tests.Unit
{
    public class UserManagerTests
    {
        private readonly Mock<IUserDal> _userDal = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IHashingService> _hashing = new();

        private UserManager CreateSut() => new(_userDal.Object, _mapper.Object, _hashing.Object);

        [Fact]
        public async Task DeleteAsync_Should_Return_Error_When_User_Not_Found()
        {
            // Arrange
            _userDal
                .Setup(d => d.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null!);

            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(42);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Kullanıcı bulunamadı.");
            _userDal.Verify(d => d.DeleteAsync(It.IsAny<User>()), Times.Never,
                "kullanıcı bulunamayınca silme çağrılmamalı");
        }

        [Fact]
        public async Task GetByEmailAsync_Should_Return_Success_With_User_When_Found()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Id = 7, Email = email, FirstName = "Ada", LastName = "Lovelace" };

            _userDal
                .Setup(d => d.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);

            var sut = CreateSut();

            // Act
            var result = await sut.GetByEmailAsync(email);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(7);
            result.Data.Email.Should().Be(email);
        }
    }
}

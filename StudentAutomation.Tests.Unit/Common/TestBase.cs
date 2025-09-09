using AutoMapper;
using Moq;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Tests.Unit.Common
{
    public abstract class TestBase
    {
        protected readonly Mock<IUserDal> UserDal = new();
        protected readonly Mock<IMapper> Mapper = new();
        protected readonly Mock<IHashingService> Hashing = new();

        // başka DAL ve servis mock'ları da ekleyebiliriz (ICourseDal vs.)
    }
}

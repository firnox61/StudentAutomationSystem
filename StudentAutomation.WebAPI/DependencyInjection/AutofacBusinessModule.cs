using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using StudentAutomation.Application.Abstraction;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Application.Services.Managers;
using StudentAutomation.Core.Aspects.Interceptors;
using StudentAutomation.Core.Aspects.Validation;
using StudentAutomation.Core.Security;
using StudentAutomation.Domain.Security;
using StudentAutomation.Infrastructure.Persistence.Repositories.EntityFramework;
using StudentAutomation.Infrastructure.Security.Hashing;
using StudentAutomation.Infrastructure.Security.Jwt;
using StudentAutomation.WebAPI.getClaim;


namespace StudentAutomation.WebAPI.DependencyInjection
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
           
            builder.RegisterType<AuthManager>().As<IAuthService>();
          //  builder.RegisterType<EfUserDal>().As<IUserDal>();
            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

         //    builder.RegisterType<UserOperationClaimManager>().As<IUserOperationClaimService>();
            builder.RegisterType<EfUserOperationClaimDal>().As<IUserOperationClaimDal>();

         //     builder.RegisterType<OperationClaimManager>().As<IOperationClaimService>();
            builder.RegisterType<EfOperationClaimDal>().As<IOperationClaimDal>();



            // Student
           // builder.RegisterType<StudentManager>().As<IStudentService>();
            builder.RegisterType<EfStudentDal>().As<IStudentDal>();

            // Teacher
         //   builder.RegisterType<TeacherManager>().As<ITeacherService>();
            builder.RegisterType<EfTeacherDal>().As<ITeacherDal>();

            // Course
        //    builder.RegisterType<CourseManager>().As<ICourseService>();
            builder.RegisterType<EfCourseDal>().As<ICourseDal>();

            // Enrollment
        //    builder.RegisterType<EnrollmentManager>().As<IEnrollmentService>();
            builder.RegisterType<EfEnrollmentDal>().As<IEnrollmentDal>();

            // Grade
          //  builder.RegisterType<GradeManager>().As<IGradeService>();
            builder.RegisterType<EfGradeDal>().As<IGradeDal>();

            // Attendance
          //  builder.RegisterType<AttendanceManager>().As<IAttendanceService>();
            builder.RegisterType<EfAttendanceDal>().As<IAttendanceDal>();

          // builder.RegisterType<StudentFeedbackManager>().As<IStudentFeedbackService>();
            builder.RegisterType<EfStudentFeedbackDal>().As<IStudentFeedbackDal>();


            builder.RegisterType<JwtHelper>().As<ITokenHelper>();
            builder.RegisterType<HashingService>().As<IHashingService>();
            builder.RegisterType<CurrentUser>().As<ICurrentUser>();
           // builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            // builder.RegisterType<AuditLogService>().As<IAuditLogService>().InstancePerLifetimeScope();
            // serviceCollection.AddScoped<IAuditLogService, AuditLogService>();

            // builder.RegisterType<AuditLogCleanupJob>().AsSelf();

            var managerAssembly = typeof(UserManager).Assembly; // En net yol
              var coreAssembly = typeof(ValidationAspect).Assembly;
              var infrastructureAssembly = typeof(AspectInterceptorSelector).Assembly;

              builder.RegisterAssemblyTypes(managerAssembly, coreAssembly, infrastructureAssembly)
                  .AsImplementedInterfaces()
                  .EnableInterfaceInterceptors(new ProxyGenerationOptions
                  {
                      Selector = new AspectInterceptorSelector()
                  })
                  .InstancePerLifetimeScope();
            /*builder.RegisterType<IngredientManager>()
      .As<IIngredientService>()
      .EnableInterfaceInterceptors(new ProxyGenerationOptions
      {
          Selector = new AspectInterceptorSelector()
      })
      .InstancePerLifetimeScope();*/






        }
    }
}

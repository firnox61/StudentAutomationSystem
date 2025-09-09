using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StudentAutomation.Core.Aspects.Interceptors;
using StudentAutomation.Core.Services;
using StudentAutomation.Core.Utilities.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace StudentAutomation.Core.Aspects.AuditLogs
{
    public class AuditLogAspect : MethodInterception
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;

        public AuditLogAspect()
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>()!;
            _auditLogService = ServiceTool.ServiceProvider.GetService<IAuditLogService>()!;
        }

        protected override void OnAfter(IInvocation invocation)
        {
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "Anonymous";
            if (_auditLogService != null)
            {
                _auditLogService.Log(
                    userEmail,
                    invocation.TargetType.Name,
                    invocation.Method.Name
                );
            }
            else
            {
                Console.WriteLine("AuditLogService is null!");
            }
        }
    }

}


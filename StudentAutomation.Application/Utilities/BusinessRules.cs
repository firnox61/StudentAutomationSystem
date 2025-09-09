using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StudentAutomation.Application.Utilities
{
    public static class BusinessRules
    {
        public static async Task<IResult?> RunAsync(params Func<Task<IResult>>[] logics)
        {
            foreach (var logic in logics)
            {
                IResult result = await logic();
                if (!result.Success)
                {
                    return result;
                }
            }
            return null;
        }
    }
}

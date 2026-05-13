using Microsoft.AspNetCore.Mvc;
using ESC.CONCOST.ModuleEmployeeCore;
using Microsoft.Extensions.Logging;
using ESC.CONCOST.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;

namespace ESC.CONCOST.ModuleEmployee.Controllers;

[Authorize]
[Route("api/Employee/[action]")]
[ApiController]
public class EmployeeController : EmployeeService, IEmployeeService
{
    [Obsolete]
    public EmployeeController(IMyContext ctx, ILogger<EmployeeService> log, IWebHostEnvironment env) : base(ctx, log, env)
    {
    }
}
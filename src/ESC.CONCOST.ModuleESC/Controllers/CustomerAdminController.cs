using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESC.Services;
using ESC.CONCOST.ModuleESCCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleESC.Controllers;

[Authorize]
[ApiController]
[Route("api/CustomerAdmin/[action]")]
public class CustomerAdminController : CustomerAdminService, ICustomerAdminService
{
    [Obsolete]
    public CustomerAdminController(IMyContext ctx, ILogger<CustomerAdminService> log, IWebHostEnvironment env) : base(ctx, log, env)
    {
    }

    [HttpGet]
    public new Task<List<Customer>> GetCustomersAsync()
    {
        return base.GetCustomersAsync();
    }

    [HttpPost("{guid}")]
    public new Task<Result> ApproveAsync(Guid guid)
    {
        return base.ApproveAsync(guid);
    }

    [HttpPost("{guid}")]
    public new Task<Result> RejectAsync(Guid guid, [FromBody] string reason)
    {
        return base.RejectAsync(guid, reason);
    }

    [HttpPost("{guid}")]
    public new Task<Result> SetPaidAsync(Guid guid, [FromBody] bool isPaid)
    {
        return base.SetPaidAsync(guid, isPaid);
    }

    [HttpPost("{guid}")]
    public new Task<Result> SetMembershipAsync(Guid guid, [FromBody] string membershipType)
    {
        return base.SetMembershipAsync(guid, membershipType);
    }
    [HttpGet("{guid}")]
    public new Task<ResultOf<Customer>> GetCustomerAsync(Guid guid)
    {
        return base.GetCustomerAsync(guid);
    }
}
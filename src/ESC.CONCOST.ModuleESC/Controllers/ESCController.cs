using Microsoft.AspNetCore.Mvc;
using ESC.CONCOST.ModuleESCCore;
using Microsoft.Extensions.Logging;
using ESC.CONCOST.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore.Models;

namespace ESC.CONCOST.ModuleESC.Controllers;

[Authorize]
[Route("api/ESC/[action]")]
[ApiController]
public class ESCController : ESCService, IESCService
{
    [Obsolete]
    public ESCController(IMyContext ctx, ILogger<ESCService> log, IWebHostEnvironment env) : base(ctx, log, env)
    {
    }

}
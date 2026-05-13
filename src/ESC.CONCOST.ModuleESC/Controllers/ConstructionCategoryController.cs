using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore.Interfaces;

namespace ESC.CONCOST.ModuleESC.Controllers;

[Authorize]
[Route("api/ConstructionCategory/[action]")]
[ApiController]
public class ConstructionCategoryController : ControllerBase
{
    private readonly IConstructionCategoryService _service;

    public ConstructionCategoryController(IConstructionCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ResultsOf<ConstructionCategory>> GetCategories() => await _service.GetCategories();

    [HttpGet]
    public async Task<ConstructionCategory?> GetCategoryByGuid(Guid guid) => await _service.GetCategoryByGuid(guid);

    [HttpPost]
    public async Task<Result> SaveCategory(ConstructionCategory model) => await _service.SaveCategory(model);

    [HttpDelete]
    public async Task<Result> DeleteCategory(Guid guid) => await _service.DeleteCategory(guid);

    [HttpGet]
    public async Task<List<ConstructionCategory>> GetActiveCategories() => await _service.GetActiveCategories();
}

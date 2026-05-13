using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESC.CONCOST.ModuleESC.Controllers;

[ApiController]
[Route("api/contract-category")]
public class ContractCategoryController : ControllerBase
{
    private readonly IContractCategoryService _service;

    public ContractCategoryController(IContractCategoryService service)
    {
        _service = service;
    }

    [HttpGet("all")]
    public async Task<ResultsOf<ContractCategory>> GetCategories() => await _service.GetCategories();

    [HttpGet("active")]
    public async Task<List<ContractCategory>> GetActiveCategories() => await _service.GetActiveCategories();

    [HttpGet("{guid}")]
    public async Task<ContractCategory> GetCategory(Guid guid) => await _service.GetCategory(guid);

    [HttpPost]
    public async Task<Result> SaveCategory(ContractCategory category) => await _service.SaveCategory(category);

    [HttpDelete("{guid}")]
    public async Task<Result> DeleteCategory(Guid guid) => await _service.DeleteCategory(guid);
}

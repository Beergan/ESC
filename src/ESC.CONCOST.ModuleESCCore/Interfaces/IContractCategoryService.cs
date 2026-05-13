using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Abstract;
using RestEase;

namespace ESC.CONCOST.ModuleESCCore.Interfaces;

public interface IContractCategoryService
{
    [Get("api/contract-category/all")]
    Task<ResultsOf<ContractCategory>> GetCategories();

    [Get("api/contract-category/active")]
    Task<List<ContractCategory>> GetActiveCategories();

    [Get("api/contract-category/{guid}")]
    Task<ContractCategory> GetCategory(Guid guid);

    [Post("api/contract-category")]
    Task<Result> SaveCategory([Body] ContractCategory category);

    [Delete("api/contract-category/{guid}")]
    Task<Result> DeleteCategory(Guid guid);
}

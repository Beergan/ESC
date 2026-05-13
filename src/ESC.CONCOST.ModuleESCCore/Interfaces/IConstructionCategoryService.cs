using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Abstract.Entities;

namespace ESC.CONCOST.ModuleESCCore.Interfaces;

[BasePath("api/ConstructionCategory")]
public interface IConstructionCategoryService
{
    [Get("GetCategories")]
    Task<ResultsOf<ConstructionCategory>> GetCategories();

    [Get("GetCategoryByGuid")]
    Task<ConstructionCategory?> GetCategoryByGuid(Guid guid);

    [Post("SaveCategory")]
    Task<Result> SaveCategory(ConstructionCategory model);

    [Delete("DeleteCategory")]
    Task<Result> DeleteCategory(Guid guid);

    [Get("GetActiveCategories")]
    Task<List<ConstructionCategory>> GetActiveCategories();
}

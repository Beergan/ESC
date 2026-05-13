using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore.Interfaces;
using ESC.CONCOST.Base;
using Microsoft.EntityFrameworkCore;

namespace ESC.CONCOST.ModuleESC.Services;

public class ContractCategoryService : IContractCategoryService
{
    private readonly IRepository<ContractCategory> _repository;
    private readonly IRepository<Contract> _contractRepo;

    public ContractCategoryService(IRepository<ContractCategory> repository, IRepository<Contract> contractRepo)
    {
        _repository = repository;
        _contractRepo = contractRepo;
    }

    public async Task<ResultsOf<ContractCategory>> GetCategories()
    {
        var items = await _repository.Query()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
        return new ResultsOf<ContractCategory> { Items = items, Success = true };
    }

    public async Task<List<ContractCategory>> GetActiveCategories()
    {
        return await _repository.Query()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
    }

    public async Task<ContractCategory> GetCategory(Guid guid)
    {
        return await _repository.GetOne(x => x.Guid == guid);
    }

    public async Task<Result> SaveCategory(ContractCategory category)
    {
        try
        {
            if (category.Id > 0)
            {
                await _repository.Update(category);
            }
            else
            {
                await _repository.Insert(category);
            }
            return new Result { Success = true };
        }
        catch (Exception ex)
        {
            return new Result { Success = false, Message = ex.Message };
        }
    }

    public async Task<Result> DeleteCategory(Guid guid)
    {
        try
        {
            var item = await _repository.GetOne(x => x.Guid == guid);
            if (item == null) return new Result { Success = false, Message = "Category not found" };

            // Check if used in contracts
            var isUsed = await _contractRepo.Query().AnyAsync(x => x.ContractMethod == item.Name);
            if (isUsed)
            {
                return new Result { Success = false, Message = "Cannot delete category that is currently in use by contracts." };
            }

            await _repository.Remove(item);
            return new Result { Success = true };
        }
        catch (Exception ex)
        {
            return new Result { Success = false, Message = ex.Message };
        }
    }
}

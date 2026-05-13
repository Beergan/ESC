using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESCCore.Interfaces;

namespace ESC.CONCOST.ModuleESC.Services;

public class ConstructionCategoryService : MyServiceBase, IConstructionCategoryService
{
    private readonly ILogger<ConstructionCategoryService> _log;

    public ConstructionCategoryService(IMyContext ctx, ILogger<ConstructionCategoryService> logger) : base(ctx)
    {
        _log = logger;
    }

    public async Task<ResultsOf<ConstructionCategory>> GetCategories()
    {
        using (var db = _ctx.ConnectDb())
        {
            var data = await db.Repo<ConstructionCategory>().Query()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            return ResultsOf<ConstructionCategory>.Ok(data);
        }
    }

    public async Task<ConstructionCategory?> GetCategoryByGuid(Guid guid)
    {
        using (var db = _ctx.ConnectDb())
        {
            return await db.Repo<ConstructionCategory>().Query()
                .FirstOrDefaultAsync(x => x.Guid == guid);
        }
    }

    public async Task<Result> SaveCategory(ConstructionCategory model)
    {
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                if (model.Id > 0)
                {
                    var exist = await db.Repo<ConstructionCategory>().GetOne(model.Id);
                    if (exist == null) return Result.Error(_ctx.Text["데이터를 찾을 수 없습니다|Data not found"]);

                    exist.Name = model.Name;
                    exist.Description = model.Description;
                    exist.SortOrder = model.SortOrder;
                    exist.IsActive = model.IsActive;
                    exist.DateModified = DateTime.UtcNow;
                    exist.UserModified = _ctx.GetCurrentUser()?.UserName ?? "System";

                    await db.Repo<ConstructionCategory>().Update(exist);
                }
                else
                {
                    model.DateCreated = DateTime.UtcNow;
                    model.UserCreated = _ctx.GetCurrentUser()?.UserName ?? "System";
                    await db.Repo<ConstructionCategory>().Insert(model);
                }
                return Result.Ok(model.Guid.ToString());
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error saving category");
                return Result.Error(ex.Message);
            }
        }
    }

    public async Task<Result> DeleteCategory(Guid guid)
    {
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var exist = await db.Repo<ConstructionCategory>().Query()
                    .FirstOrDefaultAsync(x => x.Guid == guid);
                if (exist == null) return Result.Error(_ctx.Text["데이터를 찾을 수 없습니다|Data not found"]);

                // Kiểm tra xem có đang được sử dụng trong Contract không
                // Lưu ý: ProjectName hoặc WorkType trong Contract có thể lưu Name của Category này
                // Ở đây tạm thời kiểm tra theo Name (KO|EN)
                var isUsed = await db.Repo<Contract>().Query()
                    .AnyAsync(x => x.WorkType == exist.Name);
                
                if (isUsed)
                {
                    return Result.Error(_ctx.Text["이 카테고리는 이미 사용 중이므로 삭제할 수 없습니다.|This category is already in use and cannot be deleted."]);
                }

                await db.Repo<ConstructionCategory>().Remove(exist);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting category");
                return Result.Error(ex.Message);
            }
        }
    }

    public async Task<List<ConstructionCategory>> GetActiveCategories()
    {
        using (var db = _ctx.ConnectDb())
        {
            return await db.Repo<ConstructionCategory>().Query()
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }
    }
}

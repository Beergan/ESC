using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSetting;

public class EscIndexService : MyServiceBase, IEscIndexService
{
    private readonly ILogger<EscIndexService> _log;

    public EscIndexService(IMyContext ctx, ILogger<EscIndexService> logger)
        : base(ctx)
    {
        _log = logger;
    }

    public async Task<List<EscIndexGroup>> GetIndexGroupsAsync(bool includeInactive = false)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<EscIndexGroup>();
            var query = repo.Query();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.GroupName)
                .ToListAsync();
        }
    }

    public async Task<EscIndexGroup?> GetIndexGroupByIdAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            return await db.Repo<EscIndexGroup>().GetOne(x => x.Id == id);
        }
    }

    public async Task<ResultOf<EscIndexGroup>> SaveIndexGroupAsync(EscIndexGroup model)
    {
        if (model == null)
        {
            return ResultOf<EscIndexGroup>.Error("잘못된 그룹 데이터입니다.|Invalid group data.");
        }

        model.GroupCode = NormalizeCode(model.GroupCode);
        model.GroupName = model.GroupName?.Trim() ?? string.Empty;
        model.GroupNameEn = model.GroupNameEn?.Trim() ?? string.Empty;
        model.Description = model.Description?.Trim() ?? string.Empty;
        model.IconClass = string.IsNullOrWhiteSpace(model.IconClass)
            ? "fa-solid fa-layer-group"
            : model.IconClass.Trim();

        if (string.IsNullOrWhiteSpace(model.GroupCode))
        {
            return ResultOf<EscIndexGroup>.Error("그룹 코드는 필수입니다.|Group code is required.");
        }

        if (string.IsNullOrWhiteSpace(model.GroupName))
        {
            return ResultOf<EscIndexGroup>.Error("그룹명은 필수입니다.|Group name is required.");
        }

        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<EscIndexGroup>();

            var duplicated = await repo.Exists(x => x.GroupCode == model.GroupCode && x.Id != model.Id);

            if (duplicated)
            {
                return ResultOf<EscIndexGroup>.Error($"그룹 코드 '{model.GroupCode}'가 이미 존재합니다.|Group code '{model.GroupCode}' already exists.");
            }

            if (model.Id <= 0)
            {
                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.DateCreated = DateTime.Now;
                model.DateModified = DateTime.Now;

                await repo.Insert(model);
            }
            else
            {
                var entity = await repo.GetOneEdit(x => x.Id == model.Id);

                if (entity == null)
                {
                    return ResultOf<EscIndexGroup>.Error("지수 그룹을 찾을 수 없습니다.|Index group not found.");
                }

                entity.GroupCode = model.GroupCode;
                entity.GroupName = model.GroupName;
                entity.GroupNameEn = model.GroupNameEn;
                entity.Description = model.Description;
                entity.IconClass = model.IconClass;
                entity.SortOrder = model.SortOrder;
                entity.IsActive = model.IsActive;
                entity.DateModified = DateTime.Now;

                await repo.Update(entity);
            }

            return ResultOf<EscIndexGroup>.Ok(model, "지수 그룹이 저장되었습니다.|Index group saved successfully.");
        }
    }

    public async Task<ResultOf<bool>> ToggleGroupActiveAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<EscIndexGroup>();
            var entity = await repo.GetOneEdit(x => x.Id == id);

            if (entity == null)
            {
                return ResultOf<bool>.Error("지수 그룹을 찾을 수 없습니다.|Index group not found.");
            }

            entity.IsActive = !entity.IsActive;
            entity.DateModified = DateTime.Now;

            await repo.Update(entity);

            return ResultOf<bool>.Ok(true, "그룹 상태가 업데이트되었습니다.|Group status updated successfully.");
        }
    }

    public async Task<ResultOf<bool>> DeleteGroupAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<EscIndexGroup>();
            var entity = await repo.Query(x => x.Id == id)
                .Include(x => x.IndexTypes)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultOf<bool>.Error("지수 그룹을 찾을 수 없습니다.|Index group not found.");
            }

            if (entity.IndexTypes.Any())
            {
                entity.IsActive = false;
                entity.DateModified = DateTime.Now;

                await repo.Update(entity);

                return ResultOf<bool>.Ok(true, "지수 항목이 포함된 그룹이므로 비활성화되었습니다.|Group has index items, so it was deactivated.");
            }

            await repo.Remove(entity);

            return ResultOf<bool>.Ok(true, "그룹이 삭제되었습니다.|Group deleted successfully.");
        }
    }

    public async Task<List<IndexType>> GetIndexTypesAsync(bool includeInactive = false, string groupCode = null)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexType>();
            var query = repo.Query().Include(x => x.IndexGroup).AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(groupCode))
            {
                query = query.Where(x => x.GroupCode == groupCode);
            }

            return await query
                .OrderBy(x => x.IndexGroup == null ? 9999 : x.IndexGroup.SortOrder)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.IndexName)
                .ToListAsync();
        }
    }

    public async Task<IndexType?> GetIndexTypeByIdAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            return await db.Repo<IndexType>().Query(x => x.Id == id).Include(x => x.IndexGroup).FirstOrDefaultAsync();
        }
    }

    public async Task<ResultOf<IndexType>> SaveIndexTypeAsync(IndexType model)
    {
        if (model == null)
        {
            return ResultOf<IndexType>.Error("잘못된 지수 유형 데이터입니다.|Invalid index type data.");
        }

        model.IndexCode = NormalizeCode(model.IndexCode);
        model.IndexName = model.IndexName?.Trim() ?? string.Empty;
        model.IndexNameEn = model.IndexNameEn?.Trim() ?? string.Empty;
        model.Type = model.Type?.Trim() ?? BasicCodes.EscIndexValueType.Value;
        model.DataSource = model.DataSource?.Trim() ?? string.Empty;
        model.Unit = model.Unit?.Trim() ?? string.Empty;
        model.UpdateFreq = string.IsNullOrWhiteSpace(model.UpdateFreq)
            ? "MONTHLY"
            : model.UpdateFreq.Trim();

        if (string.IsNullOrWhiteSpace(model.IndexCode))
        {
            return ResultOf<IndexType>.Error("지수 코드는 필수입니다.|Index code is required.");
        }

        if (string.IsNullOrWhiteSpace(model.IndexName))
        {
            return ResultOf<IndexType>.Error("지수명은 필수입니다.|Index name is required.");
        }

        if (!model.IndexGroupId.HasValue || model.IndexGroupId.Value <= 0)
        {
            return ResultOf<IndexType>.Error("지수 그룹은 필수입니다.|Index group is required.");
        }

        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexType>();

            var group = await db.Repo<EscIndexGroup>().GetOne(x => x.Id == model.IndexGroupId.Value);

            if (group == null)
            {
                return ResultOf<IndexType>.Error("선택한 그룹이 존재하지 않습니다.|Selected group does not exist.");
            }

            model.GroupCode = group.GroupCode;

            var duplicated = await repo.Exists(x => x.IndexCode == model.IndexCode && x.Id != model.Id);

            if (duplicated)
            {
                return ResultOf<IndexType>.Error($"지수 코드 '{model.IndexCode}'가 이미 존재합니다.|Index code '{model.IndexCode}' already exists.");
            }

            if (model.Id <= 0)
            {
                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.DateCreated = DateTime.Now;
                model.DateModified = DateTime.Now;

                await repo.Insert(model);
            }
            else
            {
                var entity = await repo.GetOneEdit(x => x.Id == model.Id);

                if (entity == null)
                {
                    return ResultOf<IndexType>.Error("지수 유형을 찾을 수 없습니다.|Index type not found.");
                }

                entity.IndexGroupId = model.IndexGroupId;
                entity.GroupCode = model.GroupCode;
                entity.IndexCode = model.IndexCode;
                entity.IndexName = model.IndexName;
                entity.IndexNameEn = model.IndexNameEn;
                entity.Type = model.Type;
                entity.DataSource = model.DataSource;
                entity.Unit = model.Unit;
                entity.UpdateFreq = model.UpdateFreq;
                entity.IsPpiType = model.IsPpiType;
                entity.SortOrder = model.SortOrder;
                entity.IsActive = model.IsActive;
                entity.DateModified = DateTime.Now;

                await repo.Update(entity);
            }

            return ResultOf<IndexType>.Ok(model, "지수 유형이 저장되었습니다.|Index type saved successfully.");
        }
    }

    public async Task<ResultOf<bool>> ToggleActiveAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexType>();
            var entity = await repo.GetOneEdit(x => x.Id == id);

            if (entity == null)
            {
                return ResultOf<bool>.Error("지수 유형을 찾을 수 없습니다.|Index type not found.");
            }

            entity.IsActive = !entity.IsActive;
            entity.DateModified = DateTime.Now;

            await repo.Update(entity);

            return ResultOf<bool>.Ok(true, "지수 유형 상태가 업데이트되었습니다.|Index type status updated successfully.");
        }
    }

    public async Task<ResultOf<bool>> DeleteIndexTypeAsync(int id)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexType>();
            var entity = await repo.Query(x => x.Id == id)
                .Include(x => x.TimeSeries)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultOf<bool>.Error("지수 유형을 찾을 수 없습니다.|Index type not found.");
            }

            if (entity.TimeSeries.Any())
            {
                entity.IsActive = false;
                entity.DateModified = DateTime.Now;

                await repo.Update(entity);

                return ResultOf<bool>.Ok(true, "시계열 데이터가 있는 지수 유형이므로 비활성화되었습니다.|Index type has time series data, so it was deactivated.");
            }

            await repo.Remove(entity);

            return ResultOf<bool>.Ok(true, "지수 유형이 삭제되었습니다.|Index type deleted successfully.");
        }
    }

    public async Task<List<IndexTimeSeries>> GetTimeSeriesAsync(int? year = null, string groupCode = null)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexTimeSeries>();
            var query = repo.Query()
                .Include(x => x.IndexType)
                .ThenInclude(x => x.IndexGroup)
                .AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(x => x.Year == year.Value);
            }

            if (!string.IsNullOrWhiteSpace(groupCode))
            {
                query = query.Where(x => x.IndexType.GroupCode == groupCode);
            }

            return await query
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.IndexType.IndexGroup == null ? 9999 : x.IndexType.IndexGroup.SortOrder)
                .ThenBy(x => x.IndexType.SortOrder)
                .ToListAsync();
        }
    }

    public async Task<List<string>> GetAvailablePeriodsAsync()
    {
        using (var db = _ctx.ConnectDb())
        {
            return await db.Repo<IndexTimeSeries>().Query()
                .Select(x => x.PeriodKey)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();
        }
    }

    public async Task<ResultOf<bool>> SaveTimeSeriesBatchAsync(List<IndexTimeSeries> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            return ResultOf<bool>.Ok(true, "저장할 데이터가 없습니다.|No data to save.");
        }

        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexTimeSeries>();

            foreach (var item in entries)
            {
                if (item.IndexTypeId <= 0)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.PeriodKey))
                {
                    item.PeriodKey = $"{item.Year}{item.Month:D2}";
                }

                if (item.Year <= 0 || item.Month <= 0)
                {
                    ParsePeriod(item.PeriodKey, out var y, out var m);
                    item.Year = y;
                    item.Month = m;
                }

                var entity = await repo.GetOneEdit(x =>
                    x.IndexTypeId == item.IndexTypeId &&
                    x.PeriodKey == item.PeriodKey);

                if (entity == null)
                {
                    item.Guid = item.Guid == Guid.Empty ? Guid.NewGuid() : item.Guid;
                    item.DateCreated = DateTime.Now;
                    item.DateModified = DateTime.Now;

                    await repo.Insert(item, commit: false);
                }
                else
                {
                    entity.IndexValue = item.IndexValue;
                    entity.DataVerified = item.DataVerified;
                    entity.DateModified = DateTime.Now;

                    await repo.Update(entity, commit: false);
                }
            }

            await db.SaveChangesAsync();

            return ResultOf<bool>.Ok(true, "월별 지수 데이터가 저장되었습니다.|Monthly index data saved successfully.");
        }
    }

    public async Task<ResultOf<bool>> AddMonthsAsync(string fromPeriodKey, string toPeriodKey)
    {
        if (!ParsePeriod(fromPeriodKey, out var fromYear, out var fromMonth))
        {
            return ResultOf<bool>.Error("시작 기간이 잘못되었습니다.|Invalid start period.");
        }

        if (!ParsePeriod(toPeriodKey, out var toYear, out var toMonth))
        {
            return ResultOf<bool>.Error("종료 기간이 잘못되었습니다.|Invalid end period.");
        }

        var fromDate = new DateTime(fromYear, fromMonth, 1);
        var toDate = new DateTime(toYear, toMonth, 1);
        var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        if (fromDate > toDate)
        {
            return ResultOf<bool>.Error("시작 월은 종료 월보다 작거나 같아야 합니다.|Start month must be less than or equal to end month.");
        }

        if (toDate > currentMonth)
        {
            return ResultOf<bool>.Error("미래 월의 데이터는 생성할 수 없습니다.|Cannot create data for future months.");
        }

        var totalMonths =
            ((toDate.Year - fromDate.Year) * 12) +
            toDate.Month -
            fromDate.Month +
            1;

        if (totalMonths > 24)
        {
            return ResultOf<bool>.Error("한 번에 최대 24개월까지 생성할 수 있습니다.|You can create up to 24 months at once.");
        }

        using (var db = _ctx.ConnectDb())
        {

            var activeIndexes = await db.Repo<IndexType>().Query()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.GroupCode)
                .ThenBy(x => x.SortOrder)
                .ToListAsync();

            if (!activeIndexes.Any())
            {
                return ResultOf<bool>.Error("활성 지수 유형이 없습니다. 먼저 지수 항목을 생성하세요.|No active index types. Please create index items first.");
            }

            var periodKeys = new List<string>();

            for (var date = fromDate; date <= toDate; date = date.AddMonths(1))
            {
                periodKeys.Add($"{date.Year}{date.Month:D2}");
            }

            var existingRows = await db.Repo<IndexTimeSeries>().Query()
                .AsNoTracking()
                .Where(x => periodKeys.Contains(x.PeriodKey))
                .Select(x => new
                {
                    x.PeriodKey,
                    x.IndexTypeId
                })
                .ToListAsync();

            var existingSet = existingRows
                .Select(x => $"{x.PeriodKey}_{x.IndexTypeId}")
                .ToHashSet();

            var newRows = new List<IndexTimeSeries>();

            foreach (var periodKey in periodKeys)
            {
                ParsePeriod(periodKey, out var year, out var month);

                foreach (var index in activeIndexes)
                {
                    var key = $"{periodKey}_{index.Id}";

                    if (existingSet.Contains(key))
                    {
                        continue;
                    }

                    newRows.Add(new IndexTimeSeries
                    {
                        Guid = Guid.NewGuid(),
                        IndexTypeId = index.Id,
                        PeriodKey = periodKey,
                        Year = year,
                        Month = month,
                        IndexValue = 0,
                        DataVerified = false,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });
                }
            }

            if (!newRows.Any())
            {
                return ResultOf<bool>.Error("선택한 모든 월이 이미 존재합니다.|All selected months already exist.");
            }

            await db.Repo<IndexTimeSeries>().InsertRange(newRows.ToArray());
            return ResultOf<bool>.Ok(true, $"{newRows.Count}개의 월별 지수 행이 생성되었습니다.|Created {newRows.Count} monthly index rows.");

        }


    }

    public async Task<ResultOf<bool>> CopyFromPreviousMonthAsync(string periodKey)
    {
        if (!ParsePeriod(periodKey, out var year, out var month))
        {
            return ResultOf<bool>.Error("잘못된 기간 키입니다.|Invalid period key.");
        }

        var currentDate = new DateTime(year, month, 1);
        var previousDate = currentDate.AddMonths(-1);
        var previousKey = $"{previousDate.Year}{previousDate.Month:D2}";

        using (var db = _ctx.ConnectDb())
        {
            var previousRows = await db.Repo<IndexTimeSeries>().Query(x => x.PeriodKey == previousKey).ToListAsync();

            if (!previousRows.Any())
            {
                return ResultOf<bool>.Error("이전 달의 데이터가 없습니다.|Previous month has no data.");
            }

            var currentIds = await db.Repo<IndexTimeSeries>().Query(x => x.PeriodKey == periodKey)
                .Select(x => x.IndexTypeId)
                .ToListAsync();

            var newRows = previousRows
                .Where(x => !currentIds.Contains(x.IndexTypeId))
                .Select(x => new IndexTimeSeries
                {
                    Guid = Guid.NewGuid(),
                    IndexTypeId = x.IndexTypeId,
                    PeriodKey = periodKey,
                    Year = year,
                    Month = month,
                    IndexValue = x.IndexValue,
                    DataVerified = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                })
                .ToList();

            if (!newRows.Any())
            {
                return ResultOf<bool>.Ok(true, "복사할 행이 없습니다.|No rows to copy.");
            }

            foreach (var row in newRows)
            {
                await db.Repo<IndexTimeSeries>().Insert(row, commit: false);
            }

            await db.SaveChangesAsync();

            return ResultOf<bool>.Ok(true, "이전 달 데이터가 성공적으로 복사되었습니다.|Previous month data copied successfully.");
        }
    }

    public async Task<ResultOf<bool>> DeleteMonthAsync(string periodKey)
    {
        using (var db = _ctx.ConnectDb())
        {
            var repo = db.Repo<IndexTimeSeries>();
            var rows = await repo.Query(x => x.PeriodKey == periodKey).ToListAsync();

            if (!rows.Any())
            {
                return ResultOf<bool>.Ok(true, "삭제할 행이 없습니다.|No rows to delete.");
            }

            await repo.RemoveRange(rows);

            return ResultOf<bool>.Ok(true, "월 데이터가 삭제되었습니다.|Month data deleted successfully.");
        }
    }

    private static string NormalizeCode(string value)
    {
        return (value ?? string.Empty).Trim().Replace(" ", "_").ToUpperInvariant();
    }

    private static bool ParsePeriod(string periodKey, out int year, out int month)
    {
        year = 0;
        month = 0;

        if (string.IsNullOrWhiteSpace(periodKey))
        {
            return false;
        }

        var normalized = periodKey.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

        if (normalized.Length != 6)
        {
            return false;
        }

        if (!int.TryParse(normalized.Substring(0, 4), out year))
        {
            return false;
        }

        if (!int.TryParse(normalized.Substring(4, 2), out month))
        {
            return false;
        }

        return year >= 2000 && month >= 1 && month <= 12;
    }

}
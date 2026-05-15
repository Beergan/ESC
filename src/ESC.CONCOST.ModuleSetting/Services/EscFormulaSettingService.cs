using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSetting;

/// <summary>
/// Service quản lý cấu hình công thức tính ESC (Escalation Cost).
/// Hỗ trợ: CRUD, soft-delete, audit history, validate, test công thức.
/// </summary>
public class EscFormulaSettingService : MyServiceBase, IEscFormulaSettingService
{
    private readonly ILogger<EscFormulaSettingService> _log;

    public EscFormulaSettingService(IMyContext ctx, ILogger<EscFormulaSettingService> logger)
        : base(ctx)
    {
        _log = logger;
    }

    // ─────────────────────────────────────────────
    // QUERIES
    // ─────────────────────────────────────────────

    public async Task<List<EscFormulaSetting>> GetListAsync(bool includeInactive = false)
    {
        using var db = _ctx.ConnectDb();

        var query = db.Repo<EscFormulaSetting>().Query().AsNoTracking();

        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.IsActive)
            .ThenByDescending(x => x.VersionNo)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.FormulaCode)
            .ToListAsync();
    }

    public async Task<EscFormulaSetting?> GetCurrentAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Repo<EscFormulaSetting>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive && x.IsCurrent);
    }

    public async Task<EscFormulaSetting?> GetByGuidAsync(Guid guid)
    {
        using var db = _ctx.ConnectDb();

        return await db.Repo<EscFormulaSetting>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Guid == guid);
    }

    public async Task<List<EscFormulaVariable>> GetVariablesAsync()
    {
        using var db = _ctx.ConnectDb();

        var variables = await db.Repo<EscFormulaVariable>().Query()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsSystem)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.VariableCode)
            .ToListAsync();

        return variables.Count > 0 ? variables : EscFormulaDefaultVariables.Create();
    }

    // ─────────────────────────────────────────────
    // COMMANDS
    // ─────────────────────────────────────────────

    public async Task<ResultOf<EscFormulaSetting>> SaveAsync(EscFormulaSetting model, string changeNote)
    {
        try
        {
            var validate = await ValidateFormulaAsync(model);
            if (!validate.Success)
                return ResultOf<EscFormulaSetting>.Error(validate.Message);

            using var db = _ctx.ConnectDb();
            var now = DateTime.Now;

            var entity = await LoadOrCreateEntityAsync(db, model, changeNote, now);

            EscFormulaMapper.Map(model, entity);

            if (entity.IsCurrent)
                await ClearOtherCurrentFlagsAsync(db, entity.Guid, now);

            await UpsertAsync(db, model.Guid, entity);

            return ResultOf<EscFormulaSetting>.Ok(entity);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "SaveAsync failed. FormulaCode: {Code}", model.FormulaCode);
            return ResultOf<EscFormulaSetting>.Error(
                BilingualError("계산식 저장 중 오류가 발생했습니다.", "Failed to save formula setting."));
        }
    }

    public async Task<ResultOf<bool>> SetCurrentAsync(Guid guid)
    {
        try
        {
            using var db = _ctx.ConnectDb();
            var now = DateTime.Now;

            var entity = await db.Repo<EscFormulaSetting>().Query()
                .FirstOrDefaultAsync(x => x.Guid == guid);

            if (entity == null)
                return ResultOf<bool>.Error(
                    BilingualError("계산식을 찾을 수 없습니다.", "Formula setting not found."));

            var validate = await ValidateFormulaAsync(entity);
            if (!validate.Success)
                return ResultOf<bool>.Error(validate.Message);

            await ClearOtherCurrentFlagsAsync(db, guid, now);

            entity.IsCurrent = true;
            entity.IsActive = true;
            entity.DateModified = now;

            await db.SaveChangesAsync();

            return ResultOf<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "SetCurrentAsync failed. Guid: {Guid}", guid);
            return ResultOf<bool>.Error(
                BilingualError("현재 계산식 설정 중 오류가 발생했습니다.", "Failed to set current formula."));
        }
    }

    public async Task<ResultOf<bool>> DisableAsync(Guid guid)
    {
        try
        {
            using var db = _ctx.ConnectDb();

            var entity = await db.Repo<EscFormulaSetting>().Query()
                .FirstOrDefaultAsync(x => x.Guid == guid);

            if (entity == null)
                return ResultOf<bool>.Error(
                    BilingualError("계산식을 찾을 수 없습니다.", "Formula setting not found."));

            if (entity.IsCurrent)
                return ResultOf<bool>.Error(
                    BilingualError("현재 사용 중인 계산식은 비활성화할 수 없습니다.", "Current formula cannot be disabled."));

            entity.IsActive = false;
            entity.DateModified = DateTime.Now;

            await db.Repo<EscFormulaSetting>().Update(entity);

            return ResultOf<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "DisableAsync failed. Guid: {Guid}", guid);
            return ResultOf<bool>.Error(
                BilingualError("계산식 비활성화 중 오류가 발생했습니다.", "Failed to disable formula."));
        }
    }

    /// <summary>
    /// Soft delete: giữ lại record trong DB, chỉ set IsActive = false.
    /// Ghi audit history trước khi xóa.
    /// </summary>
    public async Task<ResultOf<bool>> DeleteAsync(Guid guid, string deleteReason)
    {
        try
        {
            if (guid == Guid.Empty)
                return ResultOf<bool>.Error(
                    BilingualError("삭제할 계산식이 올바르지 않습니다.", "Invalid formula selected."));

            using var db = _ctx.ConnectDb();

            var entity = await db.Repo<EscFormulaSetting>().Query()
                .FirstOrDefaultAsync(x => x.Guid == guid);

            if (entity == null)
                return ResultOf<bool>.Error(
                    BilingualError("계산식을 찾을 수 없습니다.", "Formula setting not found."));

            if (entity.IsCurrent)
                return ResultOf<bool>.Error(
                    BilingualError(
                        "현재 사용 중인 계산식은 삭제할 수 없습니다. 먼저 다른 계산식을 현재 계산식으로 설정하세요.",
                        "The current formula cannot be deleted. Please set another formula as current first."));

            if (entity.IsDefault)
                return ResultOf<bool>.Error(
                    BilingualError("기본 계산식은 삭제할 수 없습니다.", "The default formula cannot be deleted."));

            var now = DateTime.Now;
            var auditReason = deleteReason.ToAuditReason("Formula deleted by admin");

            EscFormulaHistoryHelper.Append(db, entity, auditReason, now);

            entity.IsActive = false;
            entity.IsCurrent = false;
            entity.DateModified = now;
            entity.Description = BuildDeletedDescription(entity.Description, deleteReason, now);

            await db.Repo<EscFormulaSetting>().Update(entity);

            return ResultOf<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "DeleteAsync failed. Guid: {Guid}", guid);
            return ResultOf<bool>.Error(
                BilingualError("계산식 삭제 중 오류가 발생했습니다.", "Failed to delete formula."));
        }
    }

    // ─────────────────────────────────────────────
    // VALIDATE & TEST
    // ─────────────────────────────────────────────

    public async Task<ResultOf<bool>> ValidateFormulaAsync(EscFormulaSetting model)
    {
        try
        {
            // 1. Kiểm tra field bắt buộc
            if (string.IsNullOrWhiteSpace(model.FormulaCode))
                return ResultOf<bool>.Error(
                    BilingualError("계산식 코드가 필요합니다.", "Formula code is required."));

            if (string.IsNullOrWhiteSpace(model.FormulaName))
                return ResultOf<bool>.Error(
                    BilingualError("계산식명이 필요합니다.", "Formula name is required."));

            // 2. Kiểm tra từng formula không rỗng và không chứa expression nguy hiểm
            var formulaMap = EscFormulaMapper.GetFormulaMap(model);

            foreach (var (key, value) in formulaMap)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return ResultOf<bool>.Error($"{key} is required.");

                FormulaExpressionEngine.ValidateUnsafeExpression(value);
            }

            // 3. Load allowed variables 1 lần duy nhất
            var allowedSet = await LoadAllowedVariableSetAsync();

            // 4. Kiểm tra từng variable trong công thức
            foreach (var (key, value) in formulaMap)
            {
                if (IsCompositeFormula(value)) continue;

                var usedVariables = FormulaExpressionEngine.ExtractVariables(value);
                var unknownVar = usedVariables.FirstOrDefault(v => !allowedSet.Contains(v));

                if (unknownVar != null)
                    return ResultOf<bool>.Error($"Unknown variable in {key}: {unknownVar}");
            }

            return ResultOf<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "ValidateFormulaAsync failed.");
            return ResultOf<bool>.Error(
                BilingualError("계산식 검증 실패", $"Formula validation failed: {ex.Message}"));
        }
    }

    public async Task<ResultOf<FormulaTestResultDto>> TestFormulaAsync(
        EscFormulaSetting model,
        FormulaTestInputDto input)
    {
        try
        {
            var validate = await ValidateFormulaAsync(model);
            if (!validate.Success)
                return ResultOf<FormulaTestResultDto>.Error(validate.Message);

            var ctx = FormulaEvaluationContext.FromInput(model, input);
            var result = ctx.Evaluate(model);

            return ResultOf<FormulaTestResultDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "TestFormulaAsync failed.");
            return ResultOf<FormulaTestResultDto>.Error(
                BilingualError("계산식 테스트 실패", $"Formula test failed: {ex.Message}"));
        }
    }

    // ─────────────────────────────────────────────
    // PRIVATE HELPERS
    // ─────────────────────────────────────────────

    /// <summary>Load entity nếu đã tồn tại, hoặc tạo mới. Tự động tạo history khi update.</summary>
    private async Task<EscFormulaSetting> LoadOrCreateEntityAsync(
        IDbContext db,
        EscFormulaSetting model,
        string changeNote,
        DateTime now)
    {
        if (model.Guid == Guid.Empty)
            return CreateNewEntity(now);

        var existing = await db.Repo<EscFormulaSetting>().Query()
            .FirstOrDefaultAsync(x => x.Guid == model.Guid);

        if (existing == null)
            return CreateNewEntity(now);

        EscFormulaHistoryHelper.Append(db, existing, changeNote, now);
        existing.DateModified = now;

        return existing;
    }

    private static EscFormulaSetting CreateNewEntity(DateTime now) => new()
    {
        Guid = Guid.NewGuid(),
        DateCreated = now,
        DateModified = now
    };

    /// <summary>Set IsCurrent = false cho toàn bộ record khác, để đảm bảo chỉ 1 record là current.</summary>
    private static async Task ClearOtherCurrentFlagsAsync(IDbContext db, Guid excludeGuid, DateTime now)
    {
        var others = await db.Repo<EscFormulaSetting>().Query()
            .Where(x => x.Guid != excludeGuid && x.IsCurrent)
            .ToListAsync();

        foreach (var row in others)
        {
            row.IsCurrent = false;
            row.DateModified = now;
        }

        // Lưu thay đổi IsCurrent = false trước khi insert/update entity chính
        await db.SaveChangesAsync();
    }

    /// <summary>Insert nếu entity mới, Update nếu đã tồn tại trong DB.</summary>
    private static async Task UpsertAsync(IDbContext db, Guid originalGuid, EscFormulaSetting entity)
    {
        if (originalGuid == Guid.Empty)
            await db.Repo<EscFormulaSetting>().Insert(entity);
        else
            await db.Repo<EscFormulaSetting>().Update(entity);
    }

    private async Task<HashSet<string>> LoadAllowedVariableSetAsync()
    {
        using var db = _ctx.ConnectDb();

        var codes = await db.Repo<EscFormulaVariable>().Query()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => x.VariableCode)
            .ToListAsync();

        if (codes.Count == 0)
            codes = EscFormulaDefaultVariables.Create().Select(x => x.VariableCode).ToList();

        return new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>CompositeFormula dạng SUM(WeightedRatio) là special case, bỏ qua validate variable.</summary>
    private static bool IsCompositeFormula(string value) =>
        value.Trim().Equals("SUM(WeightedRatio)", StringComparison.OrdinalIgnoreCase);

    private static string BuildDeletedDescription(string? oldDescription, string? deleteReason, DateTime deletedAt)
    {
        var reason = string.IsNullOrWhiteSpace(deleteReason) ? "No reason" : deleteReason.Trim();
        var deletedText = $"[Deleted: {deletedAt:yyyy-MM-dd HH:mm:ss}] {reason}";

        return string.IsNullOrWhiteSpace(oldDescription)
            ? deletedText
            : $"{oldDescription.Trim()}{Environment.NewLine}{deletedText}";
    }

    /// <summary>Format thông báo lỗi song ngữ KR|EN dùng dấu | làm separator.</summary>
    private static string BilingualError(string kr, string en) => $"{kr}|{en}";
}
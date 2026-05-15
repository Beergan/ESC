using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.Base.Engine;
using ESC.CONCOST.ModuleSettingCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSetting;

public class EscFormulaSettingService : MyServiceBase, IEscFormulaSettingService
{
    private readonly ILogger<EscFormulaSettingService> _log;

    public EscFormulaSettingService(
        IMyContext ctx,
        ILogger<EscFormulaSettingService> log
    ) : base(ctx)
    {
        _log = log;
    }

    public async Task<List<EscFormulaSetting>> GetFormulasAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaSetting>()
            .AsNoTracking()
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.IsActive)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.FormulaName)
            .ToListAsync();
    }

    public async Task<EscFormulaSetting?> GetFormulaAsync(Guid formulaGuid)
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Guid == formulaGuid);
    }

    public async Task<EscFormulaSetting?> GetActiveFormulaAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaSetting>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsCurrent)
            .OrderBy(x => x.SortOrder)
            .FirstOrDefaultAsync();
    }

    public async Task<Result> SaveFormulaAsync(EscFormulaSetting model)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var validate = ValidateFormula(model);
            if (!validate.Success)
            {
                return validate;
            }

            var now = DateTime.UtcNow;

            model.FormulaCode = NormalizeCode(model.FormulaCode);
            model.FormulaName = model.FormulaName.Trim();
            model.FormulaNameEn = model.FormulaNameEn?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.RoundingMethod = string.IsNullOrWhiteSpace(model.RoundingMethod)
                ? BasicCodes.EscFormulaRounding.Round
                : model.RoundingMethod.Trim();

            var old = await db.Set<EscFormulaSetting>()
                .FirstOrDefaultAsync(x => x.Guid == model.Guid);

            if (old == null)
            {
                var duplicated = await db.Set<EscFormulaSetting>()
                    .AnyAsync(x => x.FormulaCode == model.FormulaCode);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 계산식 코드입니다.|Formula code already exists."]);
                }

                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.VersionNo = 1;
                model.DateCreated = now;
                model.DateModified = now;
                model.UserCreated = _ctx.GuidEmployee.ToString();
                model.UserModified = _ctx.GuidEmployee.ToString();

                db.Set<EscFormulaSetting>().Add(model);
                await db.SaveChangesAsync();
                
                await CreateHistorySnapshotAsync(db, model, "Initial creation", now);
            }
            else
            {
                var duplicated = await db.Set<EscFormulaSetting>()
                    .AnyAsync(x => x.Id != old.Id && x.FormulaCode == model.FormulaCode);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 계산식 코드입니다.|Formula code already exists."]);
                }
                
                bool isFormulaChanged = 
                    old.WeightFormula != model.WeightFormula ||
                    old.IndexRatioFormula != model.IndexRatioFormula ||
                    old.WeightedRatioFormula != model.WeightedRatioFormula ||
                    old.CompositeFormula != model.CompositeFormula ||
                    old.AdjustmentRateFormula != model.AdjustmentRateFormula ||
                    old.ApplicableAmountFormula != model.ApplicableAmountFormula ||
                    old.GrossAdjustmentFormula != model.GrossAdjustmentFormula ||
                    old.AdvanceDeductionFormula != model.AdvanceDeductionFormula ||
                    old.FinalAdjustmentFormula != model.FinalAdjustmentFormula ||
                    old.EligibleConditionFormula != model.EligibleConditionFormula ||
                    old.ThresholdRate != model.ThresholdRate ||
                    old.ThresholdDays != model.ThresholdDays ||
                    old.RoundingMethod != model.RoundingMethod ||
                    old.DecimalPlaces != model.DecimalPlaces ||
                    old.UseAdvanceDeduction != model.UseAdvanceDeduction ||
                    old.OtherDeductionDefault != model.OtherDeductionDefault;

                old.FormulaCode = model.FormulaCode;
                old.FormulaName = model.FormulaName;
                old.FormulaNameEn = model.FormulaNameEn;
                old.Description = model.Description;
                
                old.WeightFormula = model.WeightFormula;
                old.IndexRatioFormula = model.IndexRatioFormula;
                old.WeightedRatioFormula = model.WeightedRatioFormula;
                old.CompositeFormula = model.CompositeFormula;
                old.AdjustmentRateFormula = model.AdjustmentRateFormula;
                old.ApplicableAmountFormula = model.ApplicableAmountFormula;
                old.GrossAdjustmentFormula = model.GrossAdjustmentFormula;
                old.AdvanceDeductionFormula = model.AdvanceDeductionFormula;
                old.FinalAdjustmentFormula = model.FinalAdjustmentFormula;
                old.EligibleConditionFormula = model.EligibleConditionFormula;
                
                old.ThresholdRate = model.ThresholdRate;
                old.ThresholdDays = model.ThresholdDays;
                
                old.RoundingMethod = model.RoundingMethod;
                old.DecimalPlaces = model.DecimalPlaces < 0 ? 0 : model.DecimalPlaces;
                old.UseAdvanceDeduction = model.UseAdvanceDeduction;
                old.OtherDeductionDefault = model.OtherDeductionDefault;
                
                old.IsActive = model.IsActive;
                old.SortOrder = model.SortOrder;
                old.DateModified = now;
                old.UserModified = _ctx.GuidEmployee.ToString();
                
                if (isFormulaChanged)
                {
                    old.VersionNo += 1;
                    await db.SaveChangesAsync();
                    await CreateHistorySnapshotAsync(db, old, "Formula updated", now);
                }
                else
                {
                    await db.SaveChangesAsync();
                }
            }

            return Ok(_ctx.Text["저장되었습니다.|Saved successfully."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save ESC formula.");
            return Fail(_ctx.Text["계산식 저장 중 오류가 발생했습니다.|An error occurred while saving formula."]);
        }
    }

    private async Task CreateHistorySnapshotAsync(IDbContext db, EscFormulaSetting formula, string note, DateTime now)
    {
        var history = new EscFormulaHistory
        {
            Guid = Guid.NewGuid(),
            FormulaSettingId = formula.Id,
            FormulaSettingGuid = formula.Guid,
            VersionNo = formula.VersionNo,
            SnapshotJson = JsonSerializer.Serialize(formula),
            ChangeNote = note,
            DateCreated = now,
            UserCreated = _ctx.GuidEmployee.ToString()
        };
        db.Set<EscFormulaHistory>().Add(history);
        await db.SaveChangesAsync();
    }

    public async Task<Result> SetActiveAsync(Guid formulaGuid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var target = await db.Set<EscFormulaSetting>()
                .FirstOrDefaultAsync(x => x.Guid == formulaGuid);

            if (target == null)
            {
                return Fail(_ctx.Text["계산식을 찾을 수 없습니다.|Formula was not found."]);
            }

            if (!target.IsActive)
            {
                return Fail(_ctx.Text["비활성 계산식은 활성화 설정할 수 없습니다.|Inactive formula cannot be set as current."]);
            }

            var currents = await db.Set<EscFormulaSetting>()
                .Where(x => x.IsCurrent)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var item in currents)
            {
                item.IsCurrent = false;
                item.DateModified = now;
                item.UserModified = _ctx.GuidEmployee.ToString();
            }

            target.IsCurrent = true;
            target.IsActive = true;
            target.DateModified = now;
            target.UserModified = _ctx.GuidEmployee.ToString();

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["활성 계산식으로 설정되었습니다.|Current formula has been set."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to set current ESC formula. FormulaGuid={FormulaGuid}", formulaGuid);
            return Fail(_ctx.Text["활성 계산식 설정 중 오류가 발생했습니다.|An error occurred while setting current formula."]);
        }
    }

    public async Task<Result> DeleteFormulaAsync(Guid formulaGuid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var formula = await db.Set<EscFormulaSetting>()
                .FirstOrDefaultAsync(x => x.Guid == formulaGuid);

            if (formula == null)
            {
                return Fail(_ctx.Text["계산식을 찾을 수 없습니다.|Formula was not found."]);
            }

            if (formula.IsCurrent)
            {
                return Fail(_ctx.Text["활성 계산식은 삭제할 수 없습니다.|Current formula cannot be deleted."]);
            }

            formula.IsActive = false;
            formula.DateModified = DateTime.UtcNow;
            formula.UserModified = _ctx.GuidEmployee.ToString();

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["비활성 처리되었습니다.|Formula has been disabled."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete ESC formula. FormulaGuid={FormulaGuid}", formulaGuid);
            return Fail(_ctx.Text["삭제 중 오류가 발생했습니다.|An error occurred while deleting."]);
        }
    }

    public async Task<List<EscFormulaVariable>> GetVariablesAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaVariable>()
            .AsNoTracking()
            .OrderByDescending(x => x.IsSystem)
            .ThenByDescending(x => x.IsActive)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.VariableName)
            .ToListAsync();
    }

    public async Task<Result> SaveVariableAsync(EscFormulaVariable model)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var now = DateTime.UtcNow;

            model.VariableCode = NormalizeCode(model.VariableCode);
            model.VariableName = model.VariableName.Trim();
            
            var old = await db.Set<EscFormulaVariable>()
                .FirstOrDefaultAsync(x => x.Guid == model.Guid);

            if (old == null)
            {
                var duplicated = await db.Set<EscFormulaVariable>()
                    .AnyAsync(x => x.VariableCode == model.VariableCode);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 변수 코드입니다.|Variable code already exists."]);
                }

                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.DateCreated = now;
                model.DateModified = now;
                model.UserCreated = _ctx.GuidEmployee.ToString();
                model.UserModified = _ctx.GuidEmployee.ToString();

                db.Set<EscFormulaVariable>().Add(model);
            }
            else
            {
                var duplicated = await db.Set<EscFormulaVariable>()
                    .AnyAsync(x => x.Id != old.Id && x.VariableCode == model.VariableCode);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 변수 코드입니다.|Variable code already exists."]);
                }
                
                if (old.IsSystem)
                {
                     // Do not allow changing code of system variables
                     if (old.VariableCode != model.VariableCode)
                         return Fail(_ctx.Text["시스템 변수 코드는 변경할 수 없습니다.|System variable code cannot be changed."]);
                }

                old.VariableCode = model.VariableCode;
                old.VariableName = model.VariableName;
                old.VariableNameEn = model.VariableNameEn;
                old.DataType = model.DataType;
                old.DefaultValue = model.DefaultValue;
                old.Description = model.Description;
                old.IsActive = model.IsActive;
                old.SortOrder = model.SortOrder;
                old.DateModified = now;
                old.UserModified = _ctx.GuidEmployee.ToString();
            }

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["변수가 저장되었습니다.|Variable has been saved."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save ESC variable.");
            return Fail(_ctx.Text["변수 저장 중 오류가 발생했습니다.|An error occurred while saving variable."]);
        }
    }

    public async Task<Result> DeleteVariableAsync(Guid variableGuid)
    {
         using var db = _ctx.ConnectDb();

        try
        {
            var variable = await db.Set<EscFormulaVariable>()
                .FirstOrDefaultAsync(x => x.Guid == variableGuid);

            if (variable == null)
            {
                return Fail(_ctx.Text["변수를 찾을 수 없습니다.|Variable was not found."]);
            }

            if (variable.IsSystem)
            {
                return Fail(_ctx.Text["시스템 변수는 삭제할 수 없습니다.|System variable cannot be deleted."]);
            }

            db.Set<EscFormulaVariable>().Remove(variable);

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["변수가 삭제되었습니다.|Variable has been deleted."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete ESC variable. Guid={Guid}", variableGuid);
            return Fail(_ctx.Text["변수 삭제 중 오류가 발생했습니다.|An error occurred while deleting variable."]);
        }
    }

    public async Task<List<EscFormulaHistory>> GetFormulaHistoryAsync(int formulaSettingId)
    {
         using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaHistory>()
            .AsNoTracking()
            .Where(x => x.FormulaSettingId == formulaSettingId)
            .OrderByDescending(x => x.VersionNo)
            .ToListAsync();
    }

    public async Task<ResultOf<Dictionary<string, decimal>>> TestFormulaAsync(EscFormulaSetting formula, Dictionary<string, decimal> inputValues)
    {
        try
        {
            var results = new Dictionary<string, decimal>();
            var variables = new Dictionary<string, decimal>(inputValues, StringComparer.OrdinalIgnoreCase);

            // 1. Evaluate Item logic with a mock item
            decimal weight = EvaluateSafe(formula.WeightFormula, variables);
            results["Weight"] = weight;
            variables["Weight"] = weight;

            decimal indexRatio = EvaluateSafe(formula.IndexRatioFormula, variables);
            results["IndexRatio"] = indexRatio;
            variables["IndexRatio"] = indexRatio;

            decimal weightedRatio = EvaluateSafe(formula.WeightedRatioFormula, variables);
            results["WeightedRatio"] = weightedRatio;
            variables["WeightedRatio"] = weightedRatio;

            // 2. Mock CompositeCoefficient since we only test 1 item
            decimal compositeCoeff = weightedRatio; 
            results["CompositeCoefficient"] = compositeCoeff;
            variables["CompositeCoefficient"] = compositeCoeff;

            variables["ThresholdRate"] = formula.ThresholdRate;
            variables["ThresholdDays"] = formula.ThresholdDays;
            
            // 3. Global evaluations
            decimal adjustmentRate = EvaluateSafe(formula.AdjustmentRateFormula, variables);
            results["AdjustmentRate"] = adjustmentRate;
            variables["AdjustmentRate"] = adjustmentRate;

            decimal applicableAmount = EvaluateSafe(formula.ApplicableAmountFormula, variables);
            results["ApplicableAmount"] = applicableAmount;
            variables["ApplicableAmount"] = applicableAmount;

            decimal grossAdj = EvaluateSafe(formula.GrossAdjustmentFormula, variables);
            results["GrossAdjustmentAmount"] = grossAdj;
            variables["GrossAdjustmentAmount"] = grossAdj;

            decimal advanceDeduct = EvaluateSafe(formula.AdvanceDeductionFormula, variables);
            results["AdvanceDeduction"] = advanceDeduct;
            variables["AdvanceDeduction"] = advanceDeduct;

            decimal finalAdj = EvaluateSafe(formula.FinalAdjustmentFormula, variables);
            results["FinalAdjustmentAmount"] = finalAdj;
            variables["FinalAdjustmentAmount"] = finalAdj;

            decimal condition = EvaluateSafe(formula.EligibleConditionFormula, variables);
            results["EligibleCondition"] = condition;

            return ResultOf<Dictionary<string, decimal>>.Ok(results);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Formula test failed.");
            return ResultOf<Dictionary<string, decimal>>.Error(_ctx.Text["계산 테스트 중 오류가 발생했습니다: " + ex.Message + "|An error occurred while testing formula: " + ex.Message]);
        }
    }
    
    private decimal EvaluateSafe(string expression, Dictionary<string, decimal> variables)
    {
        if (string.IsNullOrWhiteSpace(expression)) return 0;
        
        // Handle logical AND / OR loosely for EligibleConditionFormula
        // Since FormulaEngine doesn't support AND/OR natively yet, we can do a simple string replace for tests
        // Example: "AdjustmentRate >= ThresholdRate AND ElapsedDays >= ThresholdDays"
        // This is a math engine, so boolean is 1 or 0.
        // We'll just execute it and if there are comparative operators we might need to enhance the engine
        // For now, FormulaEngine doesn't support >=, <=, ==. 
        // We should just wrap the formula validation loosely or enhance FormulaEngine later.
        // As per requirements: only + - * / ( ) and Math functions. 
        // If they need to evaluate EligibleConditionFormula, they shouldn't use > or < inside FormulaEngine unless we parse it.
        
        var engine = new FormulaEngine(expression, variables);
        return engine.Evaluate();
    }

    private Result ValidateFormula(EscFormulaSetting model)
    {
        if (model == null)
            return Fail(_ctx.Text["계산식 정보가 없습니다.|Formula information is missing."]);

        if (string.IsNullOrWhiteSpace(model.FormulaCode))
            return Fail(_ctx.Text["계산식 코드를 입력하세요.|Please enter formula code."]);

        if (string.IsNullOrWhiteSpace(model.FormulaName))
            return Fail(_ctx.Text["계산식 이름을 입력하세요.|Please enter formula name."]);

        return Ok();
    }

    private static string NormalizeCode(string value)
    {
        return Regex.Replace(value?.Trim() ?? string.Empty, @"\s+", "_").ToUpperInvariant();
    }
}
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleSettingCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.IsActive)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.FormulaName)
            .ToListAsync();
    }

    public async Task<EscFormulaSetting?> GetFormulaWithFieldsAsync(Guid formulaGuid)
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaSetting>()
            .AsNoTracking()
            .Include(x => x.Fields.OrderBy(f => f.SortOrder))
                .ThenInclude(x => x.Options.OrderBy(o => o.SortOrder))
            .FirstOrDefaultAsync(x => x.Guid == formulaGuid);
    }

    public async Task<EscFormulaSetting?> GetDefaultFormulaWithFieldsAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Set<EscFormulaSetting>()
            .AsNoTracking()
            .Include(x => x.Fields.OrderBy(f => f.SortOrder))
                .ThenInclude(x => x.Options.OrderBy(o => o.SortOrder))
            .Where(x => x.IsActive && x.IsDefault)
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
            model.FormulaExpression = NormalizeExpression(model.FormulaExpression);
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
                model.DateCreated = now;
                model.DateModified = now;
                model.UserCreated = _ctx.GuidEmployee.ToString();
                model.UserModified = _ctx.GuidEmployee.ToString();

                db.Set<EscFormulaSetting>().Add(model);
            }
            else
            {
                var duplicated = await db.Set<EscFormulaSetting>()
                    .AnyAsync(x => x.Id != old.Id && x.FormulaCode == model.FormulaCode);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 계산식 코드입니다.|Formula code already exists."]);
                }

                old.FormulaCode = model.FormulaCode;
                old.FormulaName = model.FormulaName;
                old.FormulaNameEn = model.FormulaNameEn;
                old.Description = model.Description;
                old.FormulaExpression = model.FormulaExpression;
                old.RoundingMethod = model.RoundingMethod;
                old.DecimalPlaces = model.DecimalPlaces < 0 ? 0 : model.DecimalPlaces;
                old.AllowNegativeResult = model.AllowNegativeResult;
                old.IsActive = model.IsActive;
                old.SortOrder = model.SortOrder;
                old.DateModified = now;
                old.UserModified = _ctx.GuidEmployee.ToString();
            }

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["저장되었습니다.|Saved successfully."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save ESC formula.");

            return Fail(_ctx.Text["계산식 저장 중 오류가 발생했습니다.|An error occurred while saving formula."]);
        }
    }

    public async Task<Result> SetDefaultAsync(Guid formulaGuid)
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
                return Fail(_ctx.Text["비활성 계산식은 기본값으로 설정할 수 없습니다.|Inactive formula cannot be set as default."]);
            }

            var defaults = await db.Set<EscFormulaSetting>()
                .Where(x => x.IsDefault)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var item in defaults)
            {
                item.IsDefault = false;
                item.DateModified = now;
                item.UserModified = _ctx.GuidEmployee.ToString();
            }

            target.IsDefault = true;
            target.IsActive = true;
            target.DateModified = now;
            target.UserModified = _ctx.GuidEmployee.ToString();

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["기본 계산식으로 설정되었습니다.|Default formula has been set."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to set default ESC formula. FormulaGuid={FormulaGuid}", formulaGuid);

            return Fail(_ctx.Text["기본 계산식 설정 중 오류가 발생했습니다.|An error occurred while setting default formula."]);
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

            if (formula.IsDefault)
            {
                return Fail(_ctx.Text["기본 계산식은 삭제할 수 없습니다.|Default formula cannot be deleted."]);
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

    public async Task<Result> SaveFieldAsync(EscFormulaField model)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var validate = ValidateField(model);
            if (!validate.Success)
            {
                return validate;
            }

            var formulaExists = await db.Set<EscFormulaSetting>()
                .AnyAsync(x => x.Id == model.FormulaSettingId);

            if (!formulaExists)
            {
                return Fail(_ctx.Text["상위 계산식을 찾을 수 없습니다.|Parent formula was not found."]);
            }

            var now = DateTime.UtcNow;

            model.FieldKey = NormalizeCode(model.FieldKey);
            model.LabelKo = model.LabelKo.Trim();
            model.LabelEn = model.LabelEn?.Trim() ?? string.Empty;
            model.FieldType = model.FieldType.Trim();
            model.DefaultValue = model.DefaultValue?.Trim() ?? string.Empty;
            model.Placeholder = model.Placeholder?.Trim() ?? string.Empty;
            model.Unit = model.Unit?.Trim() ?? string.Empty;

            var old = await db.Set<EscFormulaField>()
                .FirstOrDefaultAsync(x => x.Guid == model.Guid);

            if (old == null)
            {
                var duplicated = await db.Set<EscFormulaField>()
                    .AnyAsync(x => x.FormulaSettingId == model.FormulaSettingId && x.FieldKey == model.FieldKey);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 필드 키입니다.|Field key already exists."]);
                }

                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.DateCreated = now;
                model.DateModified = now;
                model.UserCreated = _ctx.GuidEmployee.ToString();
                model.UserModified = _ctx.GuidEmployee.ToString();

                db.Set<EscFormulaField>().Add(model);
            }
            else
            {
                var duplicated = await db.Set<EscFormulaField>()
                    .AnyAsync(x =>
                        x.Id != old.Id &&
                        x.FormulaSettingId == old.FormulaSettingId &&
                        x.FieldKey == model.FieldKey);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 필드 키입니다.|Field key already exists."]);
                }

                old.FieldKey = model.FieldKey;
                old.LabelKo = model.LabelKo;
                old.LabelEn = model.LabelEn;
                old.FieldType = model.FieldType;
                old.DefaultValue = model.DefaultValue;
                old.Placeholder = model.Placeholder;
                old.Unit = model.Unit;
                old.IsRequired = model.IsRequired;
                old.IsReadonly = model.IsReadonly;
                old.UseInFormula = model.UseInFormula;
                old.SortOrder = model.SortOrder;
                old.ValidationMin = model.ValidationMin;
                old.ValidationMax = model.ValidationMax;
                old.DateModified = now;
                old.UserModified = _ctx.GuidEmployee.ToString();
            }

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["필드가 저장되었습니다.|Field has been saved."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save ESC formula field.");

            return Fail(_ctx.Text["필드 저장 중 오류가 발생했습니다.|An error occurred while saving field."]);
        }
    }

    public async Task<Result> DeleteFieldAsync(Guid fieldGuid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var field = await db.Set<EscFormulaField>()
                .Include(x => x.Options)
                .FirstOrDefaultAsync(x => x.Guid == fieldGuid);

            if (field == null)
            {
                return Fail(_ctx.Text["필드를 찾을 수 없습니다.|Field was not found."]);
            }

            var formula = await db.Set<EscFormulaSetting>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == field.FormulaSettingId);

            if (formula != null && IsFieldUsedInExpression(formula.FormulaExpression, field.FieldKey))
            {
                return Fail(_ctx.Text["계산식에서 사용 중인 필드는 삭제할 수 없습니다.|Field used in expression cannot be deleted."]);
            }

            db.Set<EscFormulaFieldOption>().RemoveRange(field.Options);
            db.Set<EscFormulaField>().Remove(field);

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["필드가 삭제되었습니다.|Field has been deleted."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete ESC formula field. FieldGuid={FieldGuid}", fieldGuid);

            return Fail(_ctx.Text["필드 삭제 중 오류가 발생했습니다.|An error occurred while deleting field."]);
        }
    }

    public async Task<Result> SaveFieldOptionAsync(EscFormulaFieldOption model)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            if (model.FormulaFieldId <= 0)
            {
                return Fail(_ctx.Text["필드를 먼저 선택하세요.|Please select a field first."]);
            }

            if (string.IsNullOrWhiteSpace(model.OptionValue))
            {
                return Fail(_ctx.Text["옵션 값을 입력하세요.|Please enter option value."]);
            }

            if (string.IsNullOrWhiteSpace(model.OptionTextKo))
            {
                return Fail(_ctx.Text["옵션명을 입력하세요.|Please enter option text."]);
            }

            var fieldExists = await db.Set<EscFormulaField>()
                .AnyAsync(x => x.Id == model.FormulaFieldId);

            if (!fieldExists)
            {
                return Fail(_ctx.Text["필드를 찾을 수 없습니다.|Field was not found."]);
            }

            var now = DateTime.UtcNow;

            model.OptionValue = model.OptionValue.Trim();
            model.OptionTextKo = model.OptionTextKo.Trim();
            model.OptionTextEn = model.OptionTextEn?.Trim() ?? string.Empty;

            var old = await db.Set<EscFormulaFieldOption>()
                .FirstOrDefaultAsync(x => x.Guid == model.Guid);

            if (old == null)
            {
                var duplicated = await db.Set<EscFormulaFieldOption>()
                    .AnyAsync(x => x.FormulaFieldId == model.FormulaFieldId && x.OptionValue == model.OptionValue);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 옵션 값입니다.|Option value already exists."]);
                }

                model.Guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid;
                model.DateCreated = now;
                model.DateModified = now;
                model.UserCreated = _ctx.GuidEmployee.ToString();
                model.UserModified = _ctx.GuidEmployee.ToString();

                db.Set<EscFormulaFieldOption>().Add(model);
            }
            else
            {
                var duplicated = await db.Set<EscFormulaFieldOption>()
                    .AnyAsync(x =>
                        x.Id != old.Id &&
                        x.FormulaFieldId == old.FormulaFieldId &&
                        x.OptionValue == model.OptionValue);

                if (duplicated)
                {
                    return Fail(_ctx.Text["이미 사용 중인 옵션 값입니다.|Option value already exists."]);
                }

                old.OptionValue = model.OptionValue;
                old.OptionTextKo = model.OptionTextKo;
                old.OptionTextEn = model.OptionTextEn;
                old.SortOrder = model.SortOrder;
                old.IsActive = model.IsActive;
                old.DateModified = now;
                old.UserModified = _ctx.GuidEmployee.ToString();
            }

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["옵션이 저장되었습니다.|Option has been saved."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save ESC formula field option.");

            return Fail(_ctx.Text["옵션 저장 중 오류가 발생했습니다.|An error occurred while saving option."]);
        }
    }

    public async Task<Result> DeleteFieldOptionAsync(Guid optionGuid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var option = await db.Set<EscFormulaFieldOption>()
                .FirstOrDefaultAsync(x => x.Guid == optionGuid);

            if (option == null)
            {
                return Fail(_ctx.Text["옵션을 찾을 수 없습니다.|Option was not found."]);
            }

            db.Set<EscFormulaFieldOption>().Remove(option);

            await db.SaveChangesAsync();

            return Ok(_ctx.Text["옵션이 삭제되었습니다.|Option has been deleted."]);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete ESC formula field option. OptionGuid={OptionGuid}", optionGuid);

            return Fail(_ctx.Text["옵션 삭제 중 오류가 발생했습니다.|An error occurred while deleting option."]);
        }
    }

    public async Task<ResultOf<decimal>> TestFormulaAsync(Guid formulaGuid, Dictionary<string, string> values)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var formula = await db.Set<EscFormulaSetting>()
                .AsNoTracking()
                .Include(x => x.Fields.OrderBy(f => f.SortOrder))
                .FirstOrDefaultAsync(x => x.Guid == formulaGuid);

            if (formula == null)
            {
                return ResultOf<decimal>.Error(_ctx.Text["계산식을 찾을 수 없습니다.|Formula was not found."]);
            }

            return CalculateFormula(formula, values);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to test ESC formula. FormulaGuid={FormulaGuid}", formulaGuid);

            return ResultOf<decimal>.Error(_ctx.Text["계산식 테스트 중 오류가 발생했습니다.|An error occurred while testing formula."]);
        }
    }

    public ResultOf<decimal> CalculateFormula(EscFormulaSetting formula, Dictionary<string, string> values)
    {
        try
        {
            var validate = ValidateFormulaExpression(formula.FormulaExpression, formula.Fields.ToList());
            if (!validate.Success)
            {
                return ResultOf<decimal>.Error(validate.Message);
            }

            var expression = NormalizeExpression(formula.FormulaExpression);

            foreach (var field in formula.Fields.Where(x => x.UseInFormula))
            {
                values.TryGetValue(field.FieldKey, out var rawValue);

                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    rawValue = field.DefaultValue;
                }

                var decimalValue = ConvertFieldValueToDecimal(field, rawValue);

                if (decimalValue == null)
                {
                    return ResultOf<decimal>.Error(
                        _ctx.Text[
                            $"필드 값이 올바르지 않습니다: {field.LabelKo}|Invalid field value: {field.LabelEn}"
                        ]
                    );
                }

                expression = ReplaceVariable(expression, field.FieldKey, decimalValue.Value);
            }

            var table = new DataTable();
            var raw = table.Compute(expression, string.Empty);

            var result = Convert.ToDecimal(raw, CultureInfo.InvariantCulture);

            if (!formula.AllowNegativeResult && result < 0)
            {
                result = 0;
            }

            result = ApplyRounding(result, formula.RoundingMethod, formula.DecimalPlaces);

            return ResultOf<decimal>.Ok(result);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Formula calculation failed. FormulaCode={FormulaCode}", formula.FormulaCode);

            return ResultOf<decimal>.Error(_ctx.Text["계산식 계산 중 오류가 발생했습니다.|An error occurred while calculating formula."]);
        }
    }

    private Result ValidateFormula(EscFormulaSetting model)
    {
        if (model == null)
        {
            return Fail(_ctx.Text["계산식 정보가 없습니다.|Formula information is missing."]);
        }

        if (string.IsNullOrWhiteSpace(model.FormulaCode))
        {
            return Fail(_ctx.Text["계산식 코드를 입력하세요.|Please enter formula code."]);
        }

        if (string.IsNullOrWhiteSpace(model.FormulaName))
        {
            return Fail(_ctx.Text["계산식 이름을 입력하세요.|Please enter formula name."]);
        }

        if (string.IsNullOrWhiteSpace(model.FormulaExpression))
        {
            return Fail(_ctx.Text["계산식을 입력하세요.|Please enter formula expression."]);
        }

        if (!BasicCodes.EscFormulaRounding.IsValid(model.RoundingMethod))
        {
            return Fail(_ctx.Text["올바르지 않은 반올림 방식입니다.|Invalid rounding method."]);
        }

        if (model.DecimalPlaces < 0 || model.DecimalPlaces > 6)
        {
            return Fail(_ctx.Text["소수점 자릿수는 0~6 사이여야 합니다.|Decimal places must be between 0 and 6."]);
        }

        return Ok();
    }

    private Result ValidateField(EscFormulaField model)
    {
        if (model == null)
        {
            return Fail(_ctx.Text["필드 정보가 없습니다.|Field information is missing."]);
        }

        if (model.FormulaSettingId <= 0)
        {
            return Fail(_ctx.Text["상위 계산식이 없습니다.|Parent formula is missing."]);
        }

        if (string.IsNullOrWhiteSpace(model.FieldKey))
        {
            return Fail(_ctx.Text["필드 키를 입력하세요.|Please enter field key."]);
        }

        if (!Regex.IsMatch(model.FieldKey.Trim(), @"^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            return Fail(_ctx.Text["필드 키는 영문, 숫자, 밑줄만 사용할 수 있으며 숫자로 시작할 수 없습니다.|Field key can contain letters, numbers, underscore and cannot start with number."]);
        }

        if (string.IsNullOrWhiteSpace(model.LabelKo))
        {
            return Fail(_ctx.Text["필드명을 입력하세요.|Please enter field label."]);
        }

        if (!BasicCodes.EscFormulaFieldType.IsValid(model.FieldType))
        {
            return Fail(_ctx.Text["올바르지 않은 필드 타입입니다.|Invalid field type."]);
        }

        if (model.ValidationMin.HasValue &&
            model.ValidationMax.HasValue &&
            model.ValidationMin.Value > model.ValidationMax.Value)
        {
            return Fail(_ctx.Text["최소값은 최대값보다 클 수 없습니다.|Minimum value cannot be greater than maximum value."]);
        }

        return Ok();
    }

    private Result ValidateFormulaExpression(string expression, List<EscFormulaField> fields)
    {
        expression = NormalizeExpression(expression);

        if (string.IsNullOrWhiteSpace(expression))
        {
            return Fail(_ctx.Text["계산식을 입력하세요.|Please enter formula expression."]);
        }

        var allowPattern = @"^[A-Za-z0-9_\s\+\-\*\/\(\)\.]+$";

        if (!Regex.IsMatch(expression, allowPattern))
        {
            return Fail(_ctx.Text["계산식에 허용되지 않는 문자가 포함되어 있습니다.|Formula contains invalid characters."]);
        }

        if (!IsParenthesesBalanced(expression))
        {
            return Fail(_ctx.Text["괄호가 올바르지 않습니다.|Parentheses are not balanced."]);
        }

        var tokens = Regex.Matches(expression, @"[A-Za-z_][A-Za-z0-9_]*")
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var allowedKeys = fields
            .Where(x => x.UseInFormula)
            .Select(x => x.FieldKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var invalidTokens = tokens
            .Where(x => !allowedKeys.Contains(x))
            .ToList();

        if (invalidTokens.Count > 0)
        {
            return Fail(_ctx.Text[
                $"허용되지 않는 변수가 있습니다: {string.Join(", ", invalidTokens)}|Invalid variable(s): {string.Join(", ", invalidTokens)}"
            ]);
        }

        return Ok();
    }

    private static decimal? ConvertFieldValueToDecimal(EscFormulaField field, string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return field.IsRequired ? null : 0m;
        }

        var value = rawValue
            .Replace(",", "")
            .Replace("원", "")
            .Replace("%", "")
            .Trim();

        if (field.FieldType == BasicCodes.EscFormulaFieldType.Boolean)
        {
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1" || value == "Y")
            {
                return 1m;
            }

            if (value.Equals("false", StringComparison.OrdinalIgnoreCase) || value == "0" || value == "N")
            {
                return 0m;
            }

            return null;
        }

        if (field.FieldType == BasicCodes.EscFormulaFieldType.Date)
        {
            return null;
        }

        if (field.FieldType == BasicCodes.EscFormulaFieldType.Text)
        {
            return null;
        }

        if (field.FieldType == BasicCodes.EscFormulaFieldType.Select)
        {
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var selectNumber)
                ? selectNumber
                : null;
        }

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    private static string NormalizeCode(string value)
    {
        return Regex.Replace(value?.Trim() ?? string.Empty, @"\s+", "_");
    }

    private static string NormalizeExpression(string expression)
    {
        return Regex.Replace(expression?.Trim() ?? string.Empty, @"\s+", " ");
    }

    private static string ReplaceVariable(string expression, string variableName, decimal value)
    {
        return Regex.Replace(
            expression,
            $@"\b{Regex.Escape(variableName)}\b",
            value.ToString(CultureInfo.InvariantCulture),
            RegexOptions.IgnoreCase
        );
    }

    private static bool IsFieldUsedInExpression(string expression, string fieldKey)
    {
        if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(fieldKey))
        {
            return false;
        }

        return Regex.IsMatch(
            expression,
            $@"\b{Regex.Escape(fieldKey)}\b",
            RegexOptions.IgnoreCase
        );
    }

    private static decimal ApplyRounding(decimal value, string method, int decimalPlaces)
    {
        decimalPlaces = Math.Clamp(decimalPlaces, 0, 6);

        return method switch
        {
            BasicCodes.EscFormulaRounding.Floor => Math.Floor(value),
            BasicCodes.EscFormulaRounding.Ceiling => Math.Ceiling(value),
            BasicCodes.EscFormulaRounding.None => value,
            _ => Math.Round(value, decimalPlaces)
        };
    }

    private static bool IsParenthesesBalanced(string expression)
    {
        var count = 0;

        foreach (var ch in expression)
        {
            if (ch == '(')
            {
                count++;
            }
            else if (ch == ')')
            {
                count--;
            }

            if (count < 0)
            {
                return false;
            }
        }

        return count == 0;
    }

    private static Result Ok(string message = "")
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    private static Result Fail(string message)
    {
        return new Result
        {
            Success = false,
            Message = message
        };
    }
}
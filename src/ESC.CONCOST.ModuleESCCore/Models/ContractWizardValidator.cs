using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace ESC.CONCOST.ModuleESCCore.Models;

public static class ContractWizardValidator
{
    public static ContractWizardValidationResult Validate(ContractWizardDto? model, int? step = null)
    {
        if (model == null)
        {
            return new ContractWizardValidationResult(new[]
            {
                "계약 입력 데이터가 없습니다.|Contract input data is required."
            });
        }

        var errors = new List<string>();

        if (step is null or 1)
        {
            ValidateStep1(model, errors);
        }

        if (step is null or 2)
        {
            ValidateStep2(model, errors);
        }

        if (step is null or 3)
        {
            ValidateStep3(model, errors);
        }

        return new ContractWizardValidationResult(errors);
    }

    public static bool IsValidYearMonth(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return DateTime.TryParseExact(
            value.Trim(),
            "yyyy-MM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    public static string NormalizeText(string? value, int maxLength)
    {
        var normalized = (value ?? string.Empty).Trim();

        if (normalized.Length > maxLength)
        {
            normalized = normalized[..maxLength];
        }

        return normalized;
    }

    private static void ValidateStep1(ContractWizardDto model, ICollection<string> errors)
    {
        ValidateRequired(model.ProjectName, 255, "공사명을 입력하세요|Please enter Project Name", errors);
        ValidateRequired(model.Contractor, 255, "시공사명을 입력하세요|Please enter Contractor", errors);
        ValidateRequired(model.Client, 255, "발주처명을 입력하세요|Please enter Client", errors);
        ValidateRequired(model.WorkType, 50, "공사 종류를 선택하세요|Please select Work Type", errors);
        ValidateRequired(model.ContractMethod, 50, "계약방법을 선택하세요|Please select Contract Method", errors);

        if (model.BidRate <= 0)
        {
            errors.Add("낙찰율은 0보다 커야 합니다|Bid Rate must be greater than 0");
        }
        else if (model.BidRate > 100)
        {
            errors.Add("낙찰율은 100%를 초과할 수 없습니다|Bid Rate cannot exceed 100%");
        }

        ValidateMaxLength(model.PreparedBy, 255, "작성기관은 255자를 초과할 수 없습니다|Prepared By cannot exceed 255 characters", errors);
    }

    private static void ValidateStep2(ContractWizardDto model, ICollection<string> errors)
    {
        if (model.ContractAmount <= 0)
        {
            errors.Add("계약금액을 입력하세요|Please enter Contract Amount");
        }

        if (model.AdvanceAmt < 0)
        {
            errors.Add("선금액은 0보다 작을 수 없습니다|Advance Payment cannot be less than 0");
        }
        else if (model.ContractAmount > 0 && model.AdvanceAmt > model.ContractAmount)
        {
            errors.Add("선금액은 계약금액보다 클 수 없습니다|Advance Payment cannot exceed Contract Amount");
        }

        ValidateRequiredDate(model.ContractDate, "계약체결일을 선택하세요|Please select Contract Date", errors);
        ValidateRequiredDate(model.BidDate, "입찰일을 선택하세요|Please select Bid Date", errors);
        ValidateRequiredDate(model.StartDate, "착공일을 선택하세요|Please select Start Date", errors);
        ValidateRequiredDate(model.CompletionDate, "준공예정일을 선택하세요|Please select Completion Date", errors);

        if (model.StartDate.HasValue && model.CompletionDate.HasValue &&
            model.CompletionDate.Value.Date <= model.StartDate.Value.Date)
        {
            errors.Add("준공일은 착공일보다 늦어야 합니다|Completion Date must be after Start Date");
        }

        if (model.BidDate.HasValue && model.ContractDate.HasValue &&
            model.ContractDate.Value.Date < model.BidDate.Value.Date)
        {
            errors.Add("계약체결일은 입찰일보다 빠를 수 없습니다|Contract Date cannot be earlier than Bid Date");
        }
    }

    private static void ValidateStep3(ContractWizardDto model, ICollection<string> errors)
    {
        ValidateRequiredDate(model.CompareDate, "조정기준일을 선택하세요|Please select Adjustment Base Date", errors);

        if (model.ThresholdRate <= 0)
        {
            errors.Add("등락율 기준은 0보다 커야 합니다|Fluctuation Rate must be greater than 0");
        }

        if (model.ThresholdDays <= 0)
        {
            errors.Add("경과기간 기준은 0보다 커야 합니다|Time Threshold must be greater than 0");
        }

        if (model.ExcludedAmt < 0)
        {
            errors.Add("적용제외금액은 0보다 작을 수 없습니다|Exclusion Amount cannot be less than 0");
        }

        if (model.CompareDate.HasValue && model.BidDate.HasValue && model.ThresholdDays > 0)
        {
            var diffDays = (model.CompareDate.Value.Date - model.BidDate.Value.Date).Days;

            if (diffDays < model.ThresholdDays)
            {
                errors.Add($"경과기간이 {diffDays}일입니다. 조정기준일은 입찰일로부터 {model.ThresholdDays}일 이상이어야 합니다.|The elapsed period is {diffDays} days. The adjustment base date must be at least {model.ThresholdDays} days after the bid date.");
            }
        }

        if (!IsValidYearMonth(model.PreviousMonth))
        {
            errors.Add("직전연월 형식은 YYYY-MM 이어야 합니다|Previous Month format must be YYYY-MM");
        }
    }

    private static void ValidateRequired(string? value, int maxLength, string message, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(message);
            return;
        }

        ValidateMaxLength(value, maxLength, message.Split('|')[0] + $"은 {maxLength}자를 초과할 수 없습니다|" + message.Split('|').Last() + $" cannot exceed {maxLength} characters", errors);
    }

    private static void ValidateMaxLength(string? value, int maxLength, string message, ICollection<string> errors)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength)
        {
            errors.Add(message);
        }
    }

    private static void ValidateRequiredDate(DateTime? value, string message, ICollection<string> errors)
    {
        if (!value.HasValue)
        {
            errors.Add(message);
        }
    }
}

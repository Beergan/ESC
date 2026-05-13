using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Abstract.Entities;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESCCore;
using ESC.CONCOST.ModuleESCCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;
using RestEase;
using System.IO;
using ClosedXML.Excel;
using UglyToad.PdfPig;

namespace ESC.CONCOST.ModuleESC;

public class ESCService : MyServiceBase, IESCService
{
    private readonly IWebHostEnvironment _hostingEnv;
    private readonly ILogger<ESCService> _log;
    private readonly string _enterpriseCode;

    public ESCService(IMyContext ctx, ILogger<ESCService> logger, IWebHostEnvironment env) : base(ctx)
    {
        _hostingEnv = env;
        _log = logger;
        _enterpriseCode = _ctx.EnterpriseCode;
    }

    public async Task<List<Contract>> GetContractsAsync()
    {
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var customerId = _ctx.GetCurrentUser()?.CustomerId;

                return await db.Repo<Contract>().Query()
                    .AsNoTracking()
                    .Where(x => !customerId.HasValue || x.CustomerId == customerId)
                    .OrderByDescending(x => x.ContractDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error getting contracts");
                throw;
            }
        }
    }

    public async Task<ResultOf<Contract>> CreateContractAsync(ContractWizardDto model)
    {
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var validation = ContractWizardValidator.Validate(model);
                if (!validation.IsValid)
                {
                    return ResultOf<Contract>.Error(_ctx.Text[validation.Message]);
                }

                var currentUser = _ctx.GetCurrentUser();
                var customerId = currentUser?.CustomerId;
                if (!customerId.HasValue)
                {
                    return ResultOf<Contract>.Error(_ctx.Text["고객 정보가 없는 계정은 ESC 프로젝트를 생성할 수 없습니다.|An account without customer information cannot create an ESC project."]);
                }

                Normalize(model);

                var normalizedProjectName = model.ProjectName.ToUpper();
                var isExist = await db.Repo<Contract>().Query()
                    .AnyAsync(x => x.CustomerId == customerId &&
                                   x.ProjectName != null &&
                                   x.ProjectName.ToUpper() == normalizedProjectName);
                if (isExist)
                {
                    return ResultOf<Contract>.Error(_ctx.Text[$"'{model.ProjectName}' 프로젝트가 존재합니다.|Project '{model.ProjectName}' already exists."]);
                }

                var workTypeExists = await db.Repo<ConstructionCategory>().Query()
                    .AnyAsync(x => x.IsActive && x.Name == model.WorkType);
                if (!workTypeExists)
                {
                    return ResultOf<Contract>.Error(_ctx.Text["선택한 공사 종류가 유효하지 않습니다.|Selected work type is invalid."]);
                }

                var contractMethodExists = await db.Repo<ContractCategory>().Query()
                    .AnyAsync(x => x.IsActive && x.Name == model.ContractMethod);
                if (!contractMethodExists)
                {
                    return ResultOf<Contract>.Error(_ctx.Text["선택한 계약방법이 유효하지 않습니다.|Selected contract method is invalid."]);
                }

                var contract = model.ToEntity(customerId);
                contract.UserCreated = currentUser.UserName ?? "System";
                contract.UserModified = currentUser.UserName ?? "System";
                contract.DateCreated = DateTime.UtcNow;
                contract.DateModified = DateTime.UtcNow;

                await db.Repo<Contract>().Insert(contract);

                _log.LogInformation(
                    "Contract created successfully. ContractId={ContractId}, CustomerId={CustomerId}, ProjectName={ProjectName}",
                    contract.Id,
                    contract.CustomerId,
                    contract.ProjectName);

                return ResultOf<Contract>.Ok(contract);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating contract");
                return ResultOf<Contract>.Error(_ctx.Text["저장 중 오류가 발생했습니다.|An error occurred while saving."]);
            }
        }
    }

    private static void Normalize(ContractWizardDto model)
    {
        model.ProjectName = ContractWizardValidator.NormalizeText(model.ProjectName, 255);
        model.Contractor = ContractWizardValidator.NormalizeText(model.Contractor, 255);
        model.Client = ContractWizardValidator.NormalizeText(model.Client, 255);
        model.WorkType = ContractWizardValidator.NormalizeText(model.WorkType, 50);
        model.ContractMethod = ContractWizardValidator.NormalizeText(model.ContractMethod, 50);
        model.PreparedBy = ContractWizardValidator.NormalizeText(model.PreparedBy, 255);
        model.PreviousMonth = ContractWizardValidator.NormalizeText(model.PreviousMonth, 7);
    }
    private static ContractWizardDto ReadExcelContractSample(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            return new ContractWizardDto();
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var firstRow = worksheet.FirstRowUsed();
        var lastRow = worksheet.LastRowUsed();

        if (firstRow == null || lastRow == null)
        {
            return new ContractWizardDto();
        }

        var firstCell = GetExcelCellText(firstRow.Cell(1));
        var secondCell = GetExcelCellText(firstRow.Cell(2));

        // Dạng dọc: 항목 | 내용 hoặc Item | Value
        if (IsVerticalTemplate(firstCell, secondCell))
        {
            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var key = NormalizeKey(GetExcelCellText(row.Cell(1)));
                var value = GetExcelCellText(row.Cell(2));

                if (!string.IsNullOrWhiteSpace(key))
                {
                    values[key] = value;
                }
            }

            return MapDictionaryToContractWizard(values);
        }

        // Dạng ngang: tìm dòng header thật
        var headerRow = FindExcelHeaderRow(worksheet);

        if (headerRow == null)
        {
            return new ContractWizardDto();
        }

        var dataRow = worksheet.Row(headerRow.RowNumber() + 1);

        var lastColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (var col = 1; col <= lastColumn; col++)
        {
            var key = NormalizeKey(GetExcelCellText(headerRow.Cell(col)));
            var value = GetExcelCellText(dataRow.Cell(col));

            if (!string.IsNullOrWhiteSpace(key))
            {
                values[key] = value;
            }
        }

        return MapDictionaryToContractWizard(values);
    }
    private static IXLRow? FindExcelHeaderRow(IXLWorksheet worksheet)
    {
        var knownHeaders = new[]
        {
        "프로젝트명",
        "공사명",
        "Project Name",
        "시공사",
        "시공사명",
        "Contractor",
        "발주자",
        "발주처명",
        "Client",
        "공사 종류",
        "Construction Type",
        "계약 방식",
        "계약방법",
        "Contract Method",
        "계약 금액",
        "계약금액",
        "Contract Amount"
    };

        foreach (var row in worksheet.RowsUsed())
        {
            var cells = row.CellsUsed()
                .Select(x => NormalizeKey(GetExcelCellText(x)))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var matchedCount = cells.Count(cell =>
                knownHeaders.Any(header =>
                    string.Equals(cell, header, StringComparison.OrdinalIgnoreCase)));

            if (matchedCount >= 2)
            {
                return row;
            }
        }

        return worksheet.FirstRowUsed();
    }
    private static bool IsVerticalTemplate(string firstCell, string secondCell)
    {
        return firstCell.Equals("항목", StringComparison.OrdinalIgnoreCase)
            || firstCell.Equals("Item", StringComparison.OrdinalIgnoreCase)
            || secondCell.Equals("내용", StringComparison.OrdinalIgnoreCase)
            || secondCell.Equals("Value", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetExcelCellText(IXLCell cell)
    {
        if (cell == null || cell.IsEmpty())
        {
            return string.Empty;
        }

        if (cell.DataType == XLDataType.DateTime)
        {
            return cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (cell.DataType == XLDataType.Number)
        {
            var number = cell.GetDouble();

            if (Math.Abs(number % 1) < 0.0000001)
            {
                return ((long)number).ToString(CultureInfo.InvariantCulture);
            }

            return number.ToString(CultureInfo.InvariantCulture);
        }

        return cell.GetString()?.Trim() ?? string.Empty;
    }
    private static ContractWizardDto MapDictionaryToContractWizard(Dictionary<string, string> values)
    {
        return new ContractWizardDto
        {
            ProjectName = StringHelper.GetValue(values, "ProjectName", "Project Name", "프로젝트명", "공사명"),
            Contractor = StringHelper.GetValue(values, "Contractor", "시공사명", "시공사"),
            Client = StringHelper.GetValue(values, "Client", "Investor", "발주처명", "발주자"),

            WorkType = StringHelper.GetValue(values, "WorkType", "Construction Type", "Work Type", "공사 종류", "공사종류"),
            ContractMethod = StringHelper.GetValue(values, "ContractMethod", "Contract Method", "계약방법", "계약 방식"),
            PreparedBy = StringHelper.GetValue(values, "PreparedBy", "Prepared By", "작성기관", "작성자"),

            BidRate = StringHelper.GetDecimalValue(values, "BidRate", "Winning Rate (%)", "Bid Rate (%)", "낙찰율 (%)", "낙찰율(%)", "낙찰율"),
            ContractAmount = StringHelper.GetLongValue(values, "ContractAmount", "Contract Value", "Contract Amount", "계약금액", "계약 금액"),
            AdvanceAmt = StringHelper.GetLongValue(values, "AdvanceAmt", "Advance Payment", "선금액", "선금"),
            ExcludedAmt = StringHelper.GetLongValue(values, "ExcludedAmt", "Excluded Value", "Exclusion Amount", "적용제외금액(원)", "적용제외금액", "제외 금액"),

            ContractDate = StringHelper.GetDateValue(values, "ContractDate", "Contract Date", "계약체결일", "계약일"),
            BidDate = StringHelper.GetDateValue(values, "BidDate", "Bid Date", "입찰일"),
            StartDate = StringHelper.GetDateValue(values, "StartDate", "착공일"),
            CompletionDate = StringHelper.GetDateValue(values, "CompletionDate", "준공예정일", "준공일"),
            CompareDate = StringHelper.GetDateValue(values, "CompareDate", "Adjustment Base Date", "조정기준일", "조정 기준일"),

            ThresholdRate = StringHelper.GetDecimalValue(values, "ThresholdRate", "Escalation Rate", "Fluctuation Rate", "등락율 기준 (%)", "등락율 기준", "변동률"),
            ThresholdDays = StringHelper.GetIntValue(values, "ThresholdDays", "Threshold Days", "경과기간 기준 (일)", "경과기간 기준", "임계일수"),
            PreviousMonth = StringHelper.GetValue(values, "PreviousMonth", "Previous Month", "직전연월", "전월")
        };
    }
    private static ContractWizardDto ReadPdfContractSample(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var document = PdfDocument.Open(stream);

        var rawText = string.Join(
            "",
            document.GetPages().Select(x => x.Text)
        );

        var values = ExtractPdfValuesByLabels(rawText);

        return MapDictionaryToContractWizard(values);
    }
    private static Dictionary<string, string> ExtractPdfValuesByLabels(string rawText)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(rawText))
        {
            return values;
        }

        var text = NormalizePdfText(rawText);

        var fields = GetPdfFieldLabels();

        var foundLabels = new List<(string Key, string Label, int Index)>();

        foreach (var field in fields)
        {
            foreach (var label in field.Labels)
            {
                var index = text.IndexOf(label, StringComparison.OrdinalIgnoreCase);

                if (index >= 0)
                {
                    foundLabels.Add((field.Key, label, index));
                    break;
                }
            }
        }

        var ordered = foundLabels
            .OrderBy(x => x.Index)
            .ToList();

        for (var i = 0; i < ordered.Count; i++)
        {
            var current = ordered[i];

            var valueStart = current.Index + current.Label.Length;

            while (valueStart < text.Length &&
                   (text[valueStart] == ':' ||
                    text[valueStart] == '：' ||
                    char.IsWhiteSpace(text[valueStart])))
            {
                valueStart++;
            }

            var valueEnd = i + 1 < ordered.Count
                ? ordered[i + 1].Index
                : text.Length;

            if (valueEnd <= valueStart)
            {
                values[current.Key] = string.Empty;
                continue;
            }

            var value = text.Substring(valueStart, valueEnd - valueStart).Trim();

            values[current.Key] = value;
        }

        return values;
    }
    private static List<(string Key, string[] Labels)> GetPdfFieldLabels()
    {
        return new List<(string Key, string[] Labels)>
    {
        ("ProjectName", new[] { "Project Name", "프로젝트명", "공사명" }),
        ("Contractor", new[] { "Contractor", "시공사명", "시공사" }),
        ("Client", new[] { "Investor", "Client", "발주처명", "발주자" }),

        ("WorkType", new[] { "Construction Type", "Work Type", "공사 종류", "공사종류" }),
        ("ContractMethod", new[] { "Contract Method", "계약방법", "계약 방식" }),
        ("BidRate", new[] { "Winning Rate (%)", "Bid Rate (%)", "낙찰율 (%)", "낙찰율(%)", "낙찰율" }),
        ("PreparedBy", new[] { "Prepared By", "작성기관", "작성자" }),

        ("ContractAmount", new[] { "Contract Value", "Contract Amount", "계약금액", "계약 금액" }),
        ("AdvanceAmt", new[] { "Advance Payment", "선금액", "선금" }),

        ("ContractDate", new[] { "Contract Date", "계약체결일", "계약일" }),
        ("BidDate", new[] { "Bid Date", "입찰일" }),
        ("StartDate", new[] { "Start Date", "착공일" }),
        ("CompletionDate", new[] { "Completion Date", "준공예정일", "준공일" }),
        ("CompareDate", new[] { "Adjustment Base Date", "조정기준일", "조정 기준일" }),

        ("ThresholdRate", new[] { "Escalation Rate", "Fluctuation Rate", "등락율 기준 (%)", "등락율 기준", "변동률" }),
        ("ThresholdDays", new[] { "Threshold Days", "경과기간 기준 (일)", "경과기간 기준", "임계일수" }),

        ("ExcludedAmt", new[] { "Excluded Value", "Exclusion Amount", "적용제외금액(원)", "적용제외금액", "제외 금액" }),
        ("PreviousMonth", new[] { "Previous Month", "직전연월", "전월" })
    };
    }
    private static string ExtractValueBetweenLabels(string text, string label, string[] allLabels)
    {
        var startIndex = text.IndexOf(label, StringComparison.OrdinalIgnoreCase);

        if (startIndex < 0)
        {
            return string.Empty;
        }

        var valueStart = startIndex + label.Length;

        while (valueStart < text.Length &&
               (text[valueStart] == ':' ||
                text[valueStart] == '：' ||
                char.IsWhiteSpace(text[valueStart])))
        {
            valueStart++;
        }

        var nextIndex = -1;

        foreach (var nextLabel in allLabels.OrderByDescending(x => x.Length))
        {
            if (nextLabel == label)
            {
                continue;
            }

            var index = text.IndexOf(nextLabel, valueStart, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                continue;
            }

            if (nextIndex == -1 || index < nextIndex)
            {
                nextIndex = index;
            }
        }

        if (nextIndex > valueStart)
        {
            return text.Substring(valueStart, nextIndex - valueStart).Trim();
        }

        return text.Substring(valueStart).Trim();
    }
    private static bool IsPdfHeaderOrDescriptionLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return true;
        }

        return line.Contains("ESC 데이터 입력 템플릿")
            || line.Contains("이 템플릿은")
            || line.Contains("각 항목 값을")
            || line.Equals("항목 내용", StringComparison.OrdinalIgnoreCase)
            || line.Equals("항목", StringComparison.OrdinalIgnoreCase)
            || line.Equals("내용", StringComparison.OrdinalIgnoreCase)
            || line.Equals("Item Value", StringComparison.OrdinalIgnoreCase)
            || line.Equals("Item", StringComparison.OrdinalIgnoreCase)
            || line.Equals("Value", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePdfText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return text
            .Replace("\r\n", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("항목내용", "")
            .Replace("항목 내용", "")
            .Replace("ItemValue", "")
            .Replace("Item Value", "")
            .Trim();
    }
    public async Task<ResultOf<ContractWizardDto>> ReadSampleFileAsync([Body] ContractSampleFileRequest request)
    {
        try
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.FileName) ||
                string.IsNullOrWhiteSpace(request.ContentBase64))
            {
                return ResultOf<ContractWizardDto>.Error(
                    _ctx.Text["파일 정보가 없습니다.|File information is missing."]
                );
            }

            var bytes = Convert.FromBase64String(request.ContentBase64);
            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

            ContractWizardDto model;

            if (extension == ".xlsx" || extension == ".xls")
            {
                model = ReadExcelContractSample(bytes);
            }
            else if (extension == ".pdf")
            {
                model = ReadPdfContractSample(bytes);
            }
            else
            {
                return ResultOf<ContractWizardDto>.Error(
                    _ctx.Text["지원하지 않는 파일 형식입니다.|Unsupported file type."]
                );
            }

            Normalize(model);

            return ResultOf<ContractWizardDto>.Ok(model);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error reading contract sample file");

            return ResultOf<ContractWizardDto>.Error(
                _ctx.Text["샘플 파일을 읽는 중 오류가 발생했습니다.|An error occurred while reading the sample file."]
            );
        }
    }
    //helper parse 
    private static string NormalizeKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Regex.Replace(value.Trim(), @"\s+", " ");
    }
}

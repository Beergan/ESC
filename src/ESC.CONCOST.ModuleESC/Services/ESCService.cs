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

        var worksheet = workbook.Worksheets.First();

        var headerRow = worksheet.Row(1);
        var dataRow = worksheet.Row(2);

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var lastColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (var col = 1; col <= lastColumn; col++)
        {
            var key = headerRow.Cell(col).GetString().Trim();
            var value = dataRow.Cell(col).GetString().Trim();

            if (!string.IsNullOrWhiteSpace(key))
            {
                values[key] = value;
            }
        }

        return MapDictionaryToContractWizard(values);
    }
    private static ContractWizardDto MapDictionaryToContractWizard(Dictionary<string, string> values)
    {
        return new ContractWizardDto
        {
            ProjectName = StringHelper.GetValue(values, "Project Name", "프로젝트명", "공사명"),
            Contractor = StringHelper.GetValue(values, "Contractor", "시공사", "시공사명"),
            Client = StringHelper.GetValue(values, "Investor", "Client", "발주자", "발주처명"),
            WorkType = StringHelper.GetValue(values, "Construction Type", "Work Type", "공사 종류", "공사종류"),
            ContractMethod = StringHelper.GetValue(values, "Contract Method", "계약 방식", "계약방법"),
            PreparedBy = StringHelper.GetValue(values, "Prepared By", "작성자", "작성기관"),

            BidRate = StringHelper.GetDecimalValue(values, "Winning Rate (%)", "Bid Rate (%)", "낙찰율 (%)", "낙찰율(%)"),
            ContractAmount = StringHelper.GetLongValue(values, "Contract Value", "Contract Amount", "계약 금액", "계약금액"),
            AdvanceAmt = StringHelper.GetLongValue(values, "Advance Payment", "선금", "선금액"),
            ExcludedAmt = StringHelper.GetLongValue(values, "Excluded Value", "Exclusion Amount", "제외 금액", "적용제외금액"),

            ContractDate = StringHelper.GetDateValue(values, "Contract Date", "계약일", "계약체결일"),
            BidDate = StringHelper.GetDateValue(values, "Bid Date", "입찰일"),
            StartDate = StringHelper.GetDateValue(values, "Start Date", "착공일"),
            CompletionDate = StringHelper.GetDateValue(values, "Completion Date", "준공일", "준공예정일"),
            CompareDate = StringHelper.GetDateValue(values, "Adjustment Base Date", "조정 기준일", "조정기준일"),

            ThresholdRate = StringHelper.GetDecimalValue(values, "Escalation Rate", "Fluctuation Rate", "변동률", "등락율 기준"),
            ThresholdDays = StringHelper.GetIntValue(values, "Threshold Days", "임계일수", "경과기간 기준"),
            PreviousMonth = StringHelper.GetValue(values, "Previous Month", "전월", "직전연월")
        };
    }
    private static ContractWizardDto ReadPdfContractSample(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var document = PdfDocument.Open(stream);

        var text = string.Join(
            Environment.NewLine,
            document.GetPages().Select(x => x.Text)
        );

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AddPdfValue(values, text, "Project Name", "프로젝트명", "공사명");
        AddPdfValue(values, text, "Contractor", "시공사", "시공사명");
        AddPdfValue(values, text, "Investor", "Client", "발주자", "발주처명");
        AddPdfValue(values, text, "Construction Type", "Work Type", "공사 종류", "공사종류");
        AddPdfValue(values, text, "Contract Method", "계약 방식", "계약방법");
        AddPdfValue(values, text, "Winning Rate (%)", "Bid Rate (%)", "낙찰율 (%)", "낙찰율(%)");
        AddPdfValue(values, text, "Prepared By", "작성자", "작성기관");
        AddPdfValue(values, text, "Contract Value", "Contract Amount", "계약 금액", "계약금액");
        AddPdfValue(values, text, "Advance Payment", "선금", "선금액");
        AddPdfValue(values, text, "Contract Date", "계약일", "계약체결일");
        AddPdfValue(values, text, "Bid Date", "입찰일");
        AddPdfValue(values, text, "Start Date", "착공일");
        AddPdfValue(values, text, "Completion Date", "준공일", "준공예정일");
        AddPdfValue(values, text, "Adjustment Base Date", "조정 기준일", "조정기준일");
        AddPdfValue(values, text, "Escalation Rate", "Fluctuation Rate", "변동률", "등락율 기준");
        AddPdfValue(values, text, "Threshold Days", "임계일수", "경과기간 기준");
        AddPdfValue(values, text, "Excluded Value", "Exclusion Amount", "제외 금액", "적용제외금액");
        AddPdfValue(values, text, "Previous Month", "전월", "직전연월");

        return MapDictionaryToContractWizard(values);
    }
    private static void AddPdfValue(
    Dictionary<string, string> values,
    string text,
    params string[] labels)
    {
        foreach (var label in labels)
        {
            var pattern = Regex.Escape(label) + @"\s*[:：]\s*(.+)";
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                values[label] = match.Groups[1].Value.Trim();
                return;
            }
        }
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

}

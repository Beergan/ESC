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
}

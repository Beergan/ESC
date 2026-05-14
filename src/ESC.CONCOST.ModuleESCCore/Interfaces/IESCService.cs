using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore.Models;

namespace ESC.CONCOST.ModuleESCCore;

[BasePath("api/ESC")]
public interface IESCService : IServiceBase
{
    [Get("GetContracts")]
    Task<List<Contract>> GetContractsAsync();
    [Get("GetContract/{guid}")]
    Task<ResultOf<ContractWizardDto>> GetContractAsync([Path] Guid guid);

    [Put("UpdateContract/{guid}")]
    Task<ResultOf<Contract>> UpdateContractAsync([Path] Guid guid, [Body] ContractWizardDto model);
    [Post("CreateContract")]
    Task<ResultOf<Contract>> CreateContractAsync(ContractWizardDto model);

    [Post("ReadSampleFile")]
    Task<ResultOf<ContractWizardDto>> ReadSampleFileAsync([Body] ContractSampleFileRequest request);

    [Delete("DeleteContract/{guid}")]
    Task<Result> DeleteContractAsync([Path] Guid guid);
}

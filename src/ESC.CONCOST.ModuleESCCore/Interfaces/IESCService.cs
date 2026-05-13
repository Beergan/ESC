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

    [Post("CreateContract")]
    Task<ResultOf<Contract>> CreateContractAsync(ContractWizardDto model);

    [Post("ReadSampleFile")]
    Task<ResultOf<ContractWizardDto>> ReadSampleFileAsync([Body] ContractSampleFileRequest request);
}

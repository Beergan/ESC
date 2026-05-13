using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RestEase;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleEmployeeCore;

[BasePath("api/Employee")]
public interface IEmployeeService : IServiceBase
{
    [Get(nameof(Get))]
    Task<ResultOf<EntityEmployee>> Get(Guid guid);

    [Get(nameof(GetProfile))]
    Task<ResultOf<ModelEmployeeProfile>> GetProfile(Guid guid);

    [Get(nameof(GetList))]
    Task<ResultsOf<EntityEmployee>> GetList();


    [Post(nameof(Save))]
    Task<Result> Save([Body] EntityEmployee info);


    [Get(nameof(SetToActiveEmployee))]
    Task<Result> SetToActiveEmployee(int id, Guid guidEmployee);


    [Post(nameof(Delete))]
    Task<Result> Delete(int id);
}
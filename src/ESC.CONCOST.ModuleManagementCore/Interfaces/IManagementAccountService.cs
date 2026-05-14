using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleManagementCore.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleManagementCore;

[BasePath("api/Management/Account")]
public interface IManagementAccountService : IServiceBase
{
    [Post(nameof(GetListRoles))]
    Task<ResultsOf<OptionItem<Guid>>> GetListRoles();

    [Get(nameof(GetList))]
    Task<ResultsOf<ModelUserAccount>> GetList();

    [Get(nameof(GetListEmployeeAccount))]
    Task<ResultsOf<ModelEmployeeAccount>> GetListEmployeeAccount();

    [Post(nameof(CreateUser))]
    Task<ResultOf<string>> CreateUser([Body] ModelUserAccount model, string BaseUri);

    [Post(nameof(ChangePassword))]
    Task<Result> ChangePassword(ModelPasswordChange model);

    [Post(nameof(CheckPermissionAdmin))]
    Task<bool> CheckPermissionAdmin();
    [Get(nameof(GetListAccounts))]
    Task<ResultsOf<ModelAccountListItem>> GetListAccounts();
}
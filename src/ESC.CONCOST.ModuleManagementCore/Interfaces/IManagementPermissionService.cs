using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleManagementCore;

[BasePath("api/Management/Permission")]
public interface IManagementPermissionService : IServiceBase
{
    [Post(nameof(CreateNewRole))]
    Task<Result> CreateNewRole(ModelRole model);

    [Post(nameof(GetListRoles))]
    Task<ResultsOf<ModelRole>> GetListRoles();

    [Post(nameof(GetListRoleClaims))]
    Task<Dictionary<string, long>> GetListRoleClaims(string roleId);
    
    [Post(nameof(UpdateRoleClaims))]

    Task<Result> UpdateRoleClaims(string roleId, [Body] Dictionary<string, long> dict);

    [Post(nameof(GetListRoleAccount))]
    Task<List<ModelPermissionsAccount>> GetListRoleAccount(string roleId);

    [Post(nameof(DeleteUserRole))]
    Task<Result> DeleteUserRole(string roleId, string userId);

    [Post(nameof(DeleteUserRole))]
    Task<Result> DeleteRoles(string roleId);

    [Post(nameof(GetListUserActive))]
    Task<List<ModelPermissionsAccount>> GetListUserActive(string roleId);

    [Post(nameof(CreateListUserRole))]
    Task<Result> CreateListUserRole(string roleId, string[] userIds);

    [Post(nameof(CheckPermissionAdmin))]
    Task<bool> CheckPermissionAdmin();
}
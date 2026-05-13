using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;

namespace ESC.CONCOST.ModuleManagement;

public class ManagementPermissionService : MyServiceBase, IManagementPermissionService
{
    private readonly ILogger<ManagementPermissionService> _log;
    private readonly IPermissionVersionService _versionSvc;

    public ManagementPermissionService(IMyContext ctx, ILogger<ManagementPermissionService> logger, IPermissionVersionService versionSvc) : base(ctx)
    {
        _log = logger;
        _versionSvc = versionSvc;
    }

    public async Task<Result> CreateNewRole(ModelRole model)
    {
        try
        {
            var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();

            if (model.Id == null || model.Id == "")
            {
                var entity = new IdentityRole();
                entity.Id = Guid.NewGuid().ToString();
                entity.Name = model.Name;
                entity.NormalizedName = model.NormalizedName;
                entity.ConcurrencyStamp = model.ConcurrencyStamp;
                await roleMgr.CreateAsync(entity);
            }
            else
            {
                var entityUpdate = _ctx.Set<IdentityRole>().Find(model.Id);
                entityUpdate.Name = model.Name;
                entityUpdate.NormalizedName = model.NormalizedName;
                entityUpdate.ConcurrencyStamp = model.ConcurrencyStamp;

                await roleMgr.UpdateAsync(entityUpdate);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, _ctx.Summary);
            return Result.Error("Đã có lỗi xảy ra!");
        }
    }

    public async Task<ResultsOf<ModelRole>> GetListRoles()
    {
        var list = await _ctx.Repo<IdentityRole>()
            .Query()
            .Select(x => new ModelRole
            {
                Id = x.Id,
                Name = x.Name,
                NormalizedName = x.NormalizedName,
                ConcurrencyStamp = x.ConcurrencyStamp
            })
        .AsNoTracking()
        .ToListAsync();

        return ResultsOf<ModelRole>.Ok(list);
    }

    public async Task<Dictionary<string, long>> GetListRoleClaims(string roleId)
    {
        var dict = await _ctx.Repo<IdentityRoleClaim<string>>()
            .Query(x => x.RoleId == roleId)
            .AsNoTracking()
            .ToDictionaryAsync(x => x.ClaimType, x => Int64.Parse(x.ClaimValue));

        return dict;
    }

    [Post]
    public async Task<Result> UpdateRoleClaims(string roleId, [Body] Dictionary<string, long> dict)
    {
        var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();

        var role = await roleMgr.FindByIdAsync(roleId);
        var list = await _ctx.Repo<IdentityRoleClaim<string>>()
            .Query(x => x.RoleId == roleId)
            .AsNoTracking()
            .ToListAsync();

        try
        {
            foreach (var item in list)
            {
                await roleMgr.RemoveClaimAsync(role, new System.Security.Claims.Claim(item.ClaimType, Convert.ToString(item.ClaimValue)));
            }

            foreach (var item in dict)
            {
                var claim = new System.Security.Claims.Claim(item.Key, Convert.ToString(item.Value));
                await roleMgr.AddClaimAsync(role, claim);
            }

            // Real-time update trigger
            _versionSvc.IncrementRoleVersion(roleId);
            _ctx.PublishEventStatic("FORCE_LOGOUT", roleId);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return Result.Error("Đã có lỗi xảy ra");
        }
    }

    public async Task<List<ModelPermissionsAccount>> GetListRoleAccount(string roleId)
    {
        return await _ctx.Repo<IdentityUserRole<string>>().Query(x => x.RoleId == roleId).Join(_ctx.Repo<SA_USER>().Query(), ur => ur.UserId, u => u.Id, (ur, u) => new ModelPermissionsAccount
        {
            UserId = u.Id,
            UserName = u.UserName,
            RoleId = ur.RoleId,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            LastName = u.LastName,
            FirstName = u.FirstName,
            Avatar = u.Avatar,            
        }).AsNoTracking().ToListAsync();
    }
    public async Task<Result> DeleteUserRole(string roleId, string userId)
    {
        try
        {
            var userMgr = _ctx.GetService<UserManager<SA_USER>>();
            var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();

            var role = await roleMgr.FindByIdAsync(roleId);

            var user = await userMgr.FindByIdAsync(userId);
            await userMgr.RemoveFromRoleAsync(user, role.Name);
            var claims = await userMgr.GetClaimsAsync(user);

            foreach (var claim in claims.EmptyIfNull())
            {
                await userMgr.RemoveClaimAsync(user, claim);
            }

            // Real-time update trigger
            _versionSvc.IncrementUserVersion(user.UserName);
            _ctx.PublishEventStatic("FORCE_LOGOUT", user.UserName);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return Result.Error("Đã có lỗi xảy ra!");
        }
    }

    public async Task<Result> DeleteRoles(string id)
    {
        try
        {
            var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();
            var role = await roleMgr.FindByIdAsync(id);
            await roleMgr.DeleteAsync(role);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return Result.Error("Đã có lỗi xảy ra!");
        }
    }

    public async Task<List<ModelPermissionsAccount>> GetListUserActive(string roleId)
    {
        return await _ctx.Repo<SA_USER>().Query(x => x.Id != "6f449fa3-3964-474b-b94b-efff192ef2ca").Where(x =>
            !_ctx.Repo<IdentityUserClaim<string>>().Query().Any(x1 => x1.UserId == x.Id ))
        .Select(x => new ModelPermissionsAccount
        {
            UserId = x.Id,
            UserName = x.UserName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            LastName = x.LastName,
            FirstName = x.FirstName,
            Avatar = x.Avatar
        }).AsNoTracking().ToListAsync();
    }

    public async Task<Result> CreateListUserRole(string roleId, string[] userIds)
    {
        try
        {
            var userMgr = _ctx.GetService<UserManager<SA_USER>>();
            var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();
            var role = await roleMgr.FindByIdAsync(roleId);

            foreach (var userId in userIds)
            {
                var user = await userMgr.FindByIdAsync(userId);
                await userMgr.AddToRoleAsync(user, role.Name);
                await userMgr.AddClaimAsync(user, new System.Security.Claims.Claim(nameof(user.GuidEmployee), user.GuidEmployee.ToString()));
            }

            // Real-time update trigger
            _versionSvc.IncrementRoleVersion(roleId);
            _ctx.PublishEventStatic("FORCE_LOGOUT", roleId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return Result.Error("Đã có lỗi xảy ra!");
        }
    }

    public Task<bool> CheckPermissionAdmin()
    {
        return Task.FromResult(_ctx.CheckPermission(PERMISSION.ADMIN_ACCOUNTS));
    }
}

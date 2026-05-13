using System.Collections.Concurrent;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebHost.Services;

public class PermissionVersionService : IPermissionVersionService
{
    private readonly ConcurrentDictionary<string, int> _roleVersions = new();
    private readonly ConcurrentDictionary<string, int> _userVersions = new();

    public int GetRoleVersion(string roleId)
    {
        if (string.IsNullOrEmpty(roleId)) return 0;
        return _roleVersions.GetOrAdd(roleId, 1);
    }

    public void IncrementRoleVersion(string roleId)
    {
        if (string.IsNullOrEmpty(roleId)) return;
        _roleVersions.AddOrUpdate(roleId, 1, (key, old) => old + 1);
    }

    public int GetUserVersion(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return 0;
        return _userVersions.GetOrAdd(userId, 1);
    }

    public void IncrementUserVersion(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return;
        _userVersions.AddOrUpdate(userId, 1, (key, old) => old + 1);
    }
}

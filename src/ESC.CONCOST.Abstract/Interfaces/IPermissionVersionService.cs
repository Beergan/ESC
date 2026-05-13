namespace ESC.CONCOST.Abstract;

public interface IPermissionVersionService
{
    int GetRoleVersion(string roleId);
    void IncrementRoleVersion(string roleId);
    int GetUserVersion(string userId);
    void IncrementUserVersion(string userId);
}

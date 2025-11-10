namespace Authentication.Application.Abstractions.Services;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(string userId);
}

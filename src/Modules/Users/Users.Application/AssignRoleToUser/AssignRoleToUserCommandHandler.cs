using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Application.Abstractions.Repositories;
using Users.Domain.Entities;
using Users.Domain.Errors;

namespace Users.Application.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository) : ICommandHandler<AssignRoleToUserCommand>
{
    private readonly IUserRepository m_userRepository = userRepository;
    private readonly IRoleRepository m_roleRepository = roleRepository;

    public async Task<Result> HandleAsync(AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByIdWithRolesAsync(command.UserId, cancellationToken);

        if (user == null)
        {
            return UserErrors.UserNotFoundById(command.UserId);
        }

        Role? role = await m_roleRepository.GetRoleByNameAsync(command.RoleName, cancellationToken);

        if (role == null)
        {
            return RoleErrors.RoleNotFound(command.RoleName);
        }

        Result assignResult = user.AssignRole(role);

        if (assignResult.IsFailure)
        {
            return assignResult;
        }

        Result updateResult = await m_userRepository.UpdateUserAsync(user, cancellationToken);

        return updateResult;
    }
}

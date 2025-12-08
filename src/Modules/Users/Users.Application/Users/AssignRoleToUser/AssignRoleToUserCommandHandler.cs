using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Users.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository) 
    : ICommandHandler<AssignRoleToUserCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<Result> HandleAsync(AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user == null)
        {
            return UserErrors.UserNotFoundById(command.UserId);
        }

        Role? role = await _roleRepository.GetByName(command.RoleName);

        if (role == null)
        {
            return RoleErrors.NotFound(command.RoleName);
        }

        Result assignResult = user.AssignRole(role);

        if (assignResult.IsFailure)
        {
            return assignResult;
        }

        return Result.Success();
    }
}

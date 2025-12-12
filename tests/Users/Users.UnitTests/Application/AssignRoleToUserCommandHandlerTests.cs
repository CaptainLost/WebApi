using FluentAssertions;
using Users.Application.Users.AssignRoleToUser;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class AssignRoleToUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AssignRoleToUserCommandHandler _handler;

    public AssignRoleToUserCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _roleRepository = A.Fake<IRoleRepository>();
        _handler = new AssignRoleToUserCommandHandler(_userRepository, _roleRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldAssignRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Administrator");
        var user = CreateValidUser(userId);
        var role = Role.Administrator;

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns(role);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Roles.Should().Contain(role);
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Administrator");

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFoundById(userId));

        A.CallTo(() => _roleRepository.GetByName(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task HandleAsync_WhenRoleNotFound_ShouldReturnRoleNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "NonExistentRole");
        var user = CreateValidUser(userId);

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns((Role?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound(command.RoleName));
    }

    [Fact]
    public async Task HandleAsync_WhenUserAlreadyHasRole_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Registered");
        var user = CreateValidUser(userId);
        var role = Role.Registered;

        user.Roles.Add(role); // User already has this role

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns(role);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.AlreadyHasRole(role.Name));
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoriesInCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Administrator");
        var user = CreateValidUser(userId);
        var role = Role.Administrator;

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns(role);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Administrator");
        var user = CreateValidUser(userId);
        var role = Role.Administrator;
        var cancellationToken = new CancellationToken();

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns(role);

        // Act
        await _handler.HandleAsync(command, cancellationToken);

        // Assert
        A.CallTo(() => _userRepository.GetByIdAsync(userId, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WhenAssignRoleFails_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AssignRoleToUserCommand(userId, "Registered");
        var user = CreateValidUser(userId);
        var role = Role.Registered;

        // Pre-assign the role to simulate failure
        user.Roles.Add(role);

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => _roleRepository.GetByName(command.RoleName, A<CancellationToken>._))
            .Returns(role);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    private static User CreateValidUser(Guid? id = null)
    {
        var username = Username.Create("testuser").Value;
        var email = Email.Create("test@example.com").Value;
        var nickname = Nickname.Create("TestNick").Value;
        var password = Password.Create(
            new string('A', PasswordHashingConstants.HashHexLength),
            new byte[PasswordHashingConstants.SaltSize]).Value;

        return User.Create(id ?? Guid.NewGuid(), username, email, password, nickname).Value;
    }
}

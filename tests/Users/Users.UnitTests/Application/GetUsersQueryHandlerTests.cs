using Core.Domain.Pagination;
using FluentAssertions;
using Users.Application.Users.GetUsers;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new GetUsersQueryHandler(_userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnPagedUsers()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var users = new List<User>
        {
            CreateValidUser(Guid.NewGuid(), "user1"),
            CreateValidUser(Guid.NewGuid(), "user2"),
            CreateValidUser(Guid.NewGuid(), "user3")
        };

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((users, 3));

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Count.Should().Be(3);
        result.Value.Metadata.TotalCount.Should().Be(3);
        result.Value.Metadata.PageNumber.Should().Be(1);
        result.Value.Metadata.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapUsersToUserDtos()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var users = new List<User>
        {
            CreateValidUser(userId1, "user1"),
            CreateValidUser(userId2, "user2")
        };

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((users, 2));

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var items = result.Value.Items.ToList();
        items.Count.Should().Be(2);
        items[0].Id.Should().Be(userId1);
        items[0].Username.Should().Be("user1");
        items[1].Id.Should().Be(userId2);
        items[1].Username.Should().Be("user2");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.Metadata.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithSearchTer_ShouldPassToRepository()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10, SearchTerm: "test");

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetPagedAsync(
            A<PageRequest>.That.Matches(pr => pr.SearchTerm == "test"),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WithSortParameters_ShouldPassToRepository()
    {
        // Arrange
        var query = new GetUsersQuery(
            PageNumber: 1,
            PageSize: 10,
            SortBy: "Username",
            SortDescending: true);

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetPagedAsync(
            A<PageRequest>.That.Matches(pr =>
                pr.SortBy == "Username" &&
                pr.SortDescending == true),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WithDefaultPagination_ShouldUseDefaultValues()
    {
        // Arrange
        var query = new GetUsersQuery();

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetPagedAsync(
            A<PageRequest>._,
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreatePageRequestFromQuery()
    {
        // Arrange
        var query = new GetUsersQuery(
            PageNumber: 2,
            PageSize: 20,
            SearchTerm: "search",
            SortBy: "Id",
            SortDescending: false);

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetPagedAsync(
            A<PageRequest>.That.Matches(pr =>
                pr.PageNumber == 2 &&
                pr.PageSize == 20 &&
                pr.SearchTerm == "search" &&
                pr.SortBy == "Id" &&
                pr.SortDescending == false),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateCorrectPaginationMetadata()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 2, PageSize: 5);
        var users = new List<User>
        {
            CreateValidUser(Guid.NewGuid(), "user1"),
            CreateValidUser(Guid.NewGuid(), "user2")
        };

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((users, 12)); // 12 total users

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Metadata.TotalCount.Should().Be(12);
        result.Value.Metadata.PageNumber.Should().Be(2);
        result.Value.Metadata.PageSize.Should().Be(5);
        result.Value.Metadata.TotalPages.Should().Be(3); // 12 / 5 = 3 pages
        result.Value.Metadata.HasPreviousPage.Should().BeTrue();
        result.Value.Metadata.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WithLastPage_ShouldIndicateNoNextPage()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 3, PageSize: 5);
        var users = new List<User>
        {
            CreateValidUser(Guid.NewGuid(), "user1")
        };

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((users, 11)); // 11 total users, 3 pages

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Metadata.HasPreviousPage.Should().BeTrue();
        result.Value.Metadata.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var cancellationToken = new CancellationToken();

        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, A<CancellationToken>._))
            .Returns((Enumerable.Empty<User>(), 0));

        // Act
        await _handler.HandleAsync(query, cancellationToken);

        // Assert
        A.CallTo(() => _userRepository.GetPagedAsync(A<PageRequest>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static User CreateValidUser(Guid id, string usernameValue)
    {
        var username = Username.Create(usernameValue).Value;
        var email = Email.Create($"{usernameValue}@example.com").Value;
        var nickname = Nickname.Create($"{usernameValue}Nick").Value;
        var password = Password.Create(
            new string('A', PasswordHashingConstants.HashHexLength),
            new byte[PasswordHashingConstants.SaltSize]).Value;

        return User.Create(id, username, email, password, nickname).Value;
    }
}

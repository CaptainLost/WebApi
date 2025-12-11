using Core.Domain.Messaging;
using Core.Domain.Pagination;
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
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Items.Count);
        Assert.Equal(3, result.Value.Metadata.TotalCount);
        Assert.Equal(1, result.Value.Metadata.PageNumber);
        Assert.Equal(10, result.Value.Metadata.PageSize);
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
        Assert.True(result.IsSuccess);
        var items = result.Value.Items.ToList();
        Assert.Equal(2, items.Count);
        Assert.Equal(userId1, items[0].Id);
        Assert.Equal("user1", items[0].Username);
        Assert.Equal(userId2, items[1].Id);
        Assert.Equal("user2", items[1].Username);
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
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.Metadata.TotalCount);
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
        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value.Metadata.TotalCount);
        Assert.Equal(2, result.Value.Metadata.PageNumber);
        Assert.Equal(5, result.Value.Metadata.PageSize);
        Assert.Equal(3, result.Value.Metadata.TotalPages); // 12 / 5 = 3 pages
        Assert.True(result.Value.Metadata.HasPreviousPage);
        Assert.True(result.Value.Metadata.HasNextPage);
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
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Metadata.HasPreviousPage);
        Assert.False(result.Value.Metadata.HasNextPage);
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
        const string passwordHash = "hash";
        byte[] passwordSalt = [1, 2, 3, 4];

        return User.Create(id, username, email, passwordHash, passwordSalt, nickname).Value;
    }
}

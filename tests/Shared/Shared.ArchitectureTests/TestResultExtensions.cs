using FluentAssertions;
using NetArchTest.Rules;

namespace Common.ArchitectureTests;

public static class TestResultExtensions
{
    public static void FailingTypesShouldBeEmpty(this TestResult testResult)
    {
        testResult.FailingTypes?
            .Should()
            .BeEmpty();
    }

    public static void ShouldBeSuccessful(this TestResult testResult)
    {
        testResult.IsSuccessful.Should().BeTrue(
            because: testResult.FailingTypes != null && testResult.FailingTypes.Any()
                ? $"the following types failed the test: {string.Join(", ", testResult.FailingTypes.Select(t => t.FullName))}"
                : "all types should pass the architecture test");
    }
}

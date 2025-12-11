using FluentAssertions;
using NetArchTest.Rules;

namespace Common.ArchitectureTests;

public static class TestResultExtensions
{
    public static void ShouldBeSuccessful(this TestResult testResult)
    {
        testResult.FailingTypes?
            .Should()
            .BeEmpty();
    }
}

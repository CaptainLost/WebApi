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
}

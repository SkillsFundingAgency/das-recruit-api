using FluentAssertions.Equivalency;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public static class Tolerate
{
    private const int Tolerance = 10;
    
    public static Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> SqlDateTime<T>()
    {
        return options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(Tolerance)))
            .WhenTypeIs<DateTime>();
    }

    public static EquivalencyAssertionOptions<T> TolerateSqlDateTime<T>(this EquivalencyAssertionOptions<T> options)
    {
        return options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(Tolerance)))
            .WhenTypeIs<DateTime>();
    }
}
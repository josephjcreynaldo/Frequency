using Frequency;
using Xunit;

namespace Frequency.Tests;

public class GroupingTests
{
    [Fact]
    public void BuildRows_GroupedData_UsesExplicitRangeBounds()
    {
        var observations = new double[] { 20, 21, 23, 30, 35, 40 };

        var rows = FrequencyCalculator.BuildRows(observations.ToList(), useGroupedData: true, rangeFrom: 20, rangeTo: 29);

        Assert.Equal(3, rows.Count);
        Assert.Equal("20-29", rows[0].Label);
        Assert.Equal(3, rows[0].Frequency);
        Assert.Equal("30-39", rows[1].Label);
        Assert.Equal(2, rows[1].Frequency);
        Assert.Equal("40-49", rows[2].Label);
        Assert.Equal(1, rows[2].Frequency);
    }
}

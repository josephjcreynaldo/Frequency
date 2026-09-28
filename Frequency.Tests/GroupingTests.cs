using Frequency;
using Xunit;

namespace Frequency.Tests;

public class GroupingTests
{
    [Fact]
    public void BuildRows_GroupedData_UsesNumberOfIntervals()
    {
        var observations = new double[] { 20, 21, 23, 30, 35, 40 };

        var rows = FrequencyCalculator.BuildRows(observations.ToList(), useGroupedData: true, numberOfIntervals: 3);

        Assert.Equal(3, rows.Count);
        Assert.Equal("20-26.6", rows[0].Label);
        Assert.Equal(3, rows[0].Frequency);
        Assert.Equal("26.7-33.3", rows[1].Label);
        Assert.Equal(1, rows[1].Frequency);
        Assert.Equal("33.4-40", rows[2].Label);
        Assert.Equal(2, rows[2].Frequency);
    }

    [Fact]
    public void BuildRows_GroupedData_UsesObservedRangeAndNumberOfIntervals()
    {
        var observations = new double[] { 12, 14, 14, 15, 16, 16, 16, 18, 19, 21, 21, 23 };

        var rows = FrequencyCalculator.BuildRows(observations.ToList(), useGroupedData: true, numberOfIntervals: 2);

        Assert.Equal(2, rows.Count);
        Assert.Equal("12-17.4", rows[0].Label);
        Assert.Equal(7, rows[0].Frequency);
        Assert.Equal("17.5-23", rows[1].Label);
        Assert.Equal(5, rows[1].Frequency);
        Assert.Equal(observations.Length, rows.Sum(row => row.Frequency));
    }

    [Fact]
    public void BuildRows_GroupedData_UsesNumberOfIntervalsForDecimalData()
    {
        var observations = new double[] { 2.7, 3.0, 3.2, 3.5, 3.8, 4.1, 4.2, 4.5 };

        var rows = FrequencyCalculator.BuildRows(observations.ToList(), useGroupedData: true, numberOfIntervals: 4);

        Assert.Equal(4, rows.Count);
        Assert.Equal("2.7-3.1", rows[0].Label);
        Assert.Equal(2, rows[0].Frequency);
        Assert.Equal("3.2-3.6", rows[1].Label);
        Assert.Equal(2, rows[1].Frequency);
        Assert.Equal("3.7-4.1", rows[2].Label);
        Assert.Equal(2, rows[2].Frequency);
        Assert.Equal("4.2-4.6", rows[3].Label);
        Assert.Equal(2, rows[3].Frequency);
        Assert.NotEqual(rows[0].Label[^3..], rows[1].Label[..3]);
        Assert.Equal(observations.Length, rows.Sum(row => row.Frequency));
    }

    [Fact]
    public void BuildRows_GroupedData_UsesRequestedSevenIntervalPattern()
    {
        var observations = new double[] { 2.7, 3.2, 3.7, 4.2, 4.7, 5.2, 5.7, 6.0 };

        var rows = FrequencyCalculator.BuildRows(observations, useGroupedData: true, numberOfIntervals: 7);

        Assert.Equal(["2.7-3.1", "3.2-3.6", "3.7-4.1", "4.2-4.6", "4.7-5.1", "5.2-5.6", "5.7-6.1"], rows.Select(row => row.Label));
        Assert.Equal(observations.Length, rows.Sum(row => row.Frequency));
    }
}

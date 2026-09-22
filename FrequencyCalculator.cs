using System.Globalization;

namespace Frequency;

public sealed class FrequencyCalculator
{
    public static IReadOnlyList<FrequencyRow> BuildRows(IReadOnlyList<double> observations, bool useGroupedData = false, int rangeFrom = 20, int rangeTo = 29)
    {
        if (observations.Count == 0)
        {
            return Array.Empty<FrequencyRow>();
        }

        if (!useGroupedData)
        {
            var cumulativeForExact = 0;
            var totalCount = observations.Count;
            return observations
                .GroupBy(value => value)
                .OrderBy(group => group.Key)
                .Select(group =>
                {
                    var frequency = group.Count();
                    cumulativeForExact += frequency;
                    var relativeFrequency = (double)frequency / totalCount;
                    var moreThanCumulativeFrequency = totalCount - cumulativeForExact + frequency;
                    return new FrequencyRow(
                        group.Key,
                        frequency,
                        FormatValue(group.Key),
                        group.Key,
                        group.Key,
                        cumulativeForExact,
                        moreThanCumulativeFrequency,
                        relativeFrequency,
                        (double)cumulativeForExact / totalCount,
                        (double)moreThanCumulativeFrequency / totalCount);
                })
                .ToList();
        }

        var normalizedFrom = Math.Min(rangeFrom, rangeTo);
        var normalizedTo = Math.Max(rangeFrom, rangeTo);
        var intervalSize = Math.Max(1, normalizedTo - normalizedFrom + 1);
        var start = normalizedFrom;
        var rows = new List<FrequencyRow>();
        var totalCountForGrouped = observations.Count;
        var cumulativeForGrouped = 0;

        while (start <= observations.Max())
        {
            var end = start + intervalSize - 1;
            var valuesInInterval = observations
                .Where(value => value >= start && value <= end)
                .ToList();

            var frequency = valuesInInterval.Count;
            cumulativeForGrouped += frequency;
            var relativeFrequency = totalCountForGrouped == 0 ? 0 : (double)frequency / totalCountForGrouped;
            var moreThanCumulativeFrequency = totalCountForGrouped - cumulativeForGrouped + frequency;
            var midpoint = valuesInInterval.Count == 0 ? start : valuesInInterval.Average();
            var label = $"{FormatValue(start)}-{FormatValue(end)}";

            rows.Add(new FrequencyRow(
                midpoint,
                frequency,
                label,
                start,
                end,
                cumulativeForGrouped,
                moreThanCumulativeFrequency,
                relativeFrequency,
                totalCountForGrouped == 0 ? 0 : (double)cumulativeForGrouped / totalCountForGrouped,
                totalCountForGrouped == 0 ? 0 : (double)moreThanCumulativeFrequency / totalCountForGrouped));

            start += intervalSize;
        }

        return rows;
    }

    private static string FormatValue(double value)
    {
        return value.ToString("G4", CultureInfo.InvariantCulture);
    }
}

public sealed class FrequencyRow
{
    public FrequencyRow(
        double value,
        int frequency,
        string label,
        double lowerBound,
        double upperBound,
        int lessThanCumulativeFrequency,
        int moreThanCumulativeFrequency,
        double relativeFrequency,
        double lessThanRelativeFrequency,
        double moreThanRelativeFrequency)
    {
        Value = value;
        Frequency = frequency;
        Label = label;
        LowerBound = lowerBound;
        UpperBound = upperBound;
        LessThanCumulativeFrequency = lessThanCumulativeFrequency;
        MoreThanCumulativeFrequency = moreThanCumulativeFrequency;
        RelativeFrequency = relativeFrequency;
        LessThanRelativeFrequency = lessThanRelativeFrequency;
        MoreThanRelativeFrequency = moreThanRelativeFrequency;
    }

    public double Value { get; }
    public int Frequency { get; }
    public string Label { get; }
    public double LowerBound { get; }
    public double UpperBound { get; }
    public int LessThanCumulativeFrequency { get; }
    public int MoreThanCumulativeFrequency { get; }
    public double RelativeFrequency { get; }
    public double LessThanRelativeFrequency { get; }
    public double MoreThanRelativeFrequency { get; }
}

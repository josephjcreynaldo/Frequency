using System.Globalization;

namespace Frequency;

public sealed class FrequencyCalculator
{
    public static IReadOnlyList<FrequencyRow> BuildRows(IReadOnlyList<double> observations, bool useGroupedData = false, int numberOfIntervals = 5)
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

        var minObservedValue = observations.Min();
        var maxObservedValue = observations.Max();
        var range = maxObservedValue - minObservedValue;
        var classCount = Math.Max(1, numberOfIntervals);
        var observationPrecision = observations.Max(DecimalPlaces);
        var intervalPrecision = Math.Max(1, observationPrecision);
        var intervalScale = Math.Pow(10, intervalPrecision);
        var intervalSize = range == 0
            ? 1d
            : Math.Ceiling(range / classCount * intervalScale) / intervalScale;
        var boundaryPrecision = Math.Max(observations.Max(DecimalPlaces), DecimalPlaces(intervalSize));
        var displayUnit = Math.Pow(10, -boundaryPrecision);
        var rows = new List<FrequencyRow>();
        var totalCountForGrouped = observations.Count;
        var cumulativeForGrouped = 0;

        for (var index = 0; index < classCount; index++)
        {
            var start = minObservedValue + index * intervalSize;
            var calculatedEnd = start + intervalSize;
            var end = index == classCount - 1 && Math.Abs(calculatedEnd - maxObservedValue) < 0.0000001
                ? maxObservedValue
                : calculatedEnd;
            var valuesInInterval = observations
                .Where(value => value >= start && (index == classCount - 1 ? value <= end : value < end))
                .ToList();

            var frequency = valuesInInterval.Count;
            cumulativeForGrouped += frequency;
            var relativeFrequency = totalCountForGrouped == 0 ? 0 : (double)frequency / totalCountForGrouped;
            var moreThanCumulativeFrequency = totalCountForGrouped - cumulativeForGrouped + frequency;
            var midpoint = valuesInInterval.Count == 0 ? start : valuesInInterval.Average();
            var displayStart = index == 0
                ? TruncateToOneDecimal(start)
                : TruncateToOneDecimal(start - displayUnit) + 0.1d;
            var finalEndMatchesObservedMaximum = index == classCount - 1 && Math.Abs(end - maxObservedValue) < 0.0000001;
            var displayEnd = finalEndMatchesObservedMaximum
                ? TruncateToOneDecimal(end)
                : TruncateToOneDecimal(end - displayUnit);
            var label = $"{FormatValue(displayStart)}-{FormatValue(displayEnd)}";

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
        }

        return rows;
    }

    private static string FormatValue(double value)
    {
        return value.ToString(value == Math.Truncate(value) ? "0" : "0.0", CultureInfo.InvariantCulture);
    }

    private static double TruncateToOneDecimal(double value)
    {
        return Math.Truncate((value + 0.000000001d) * 10d) / 10d;
    }

    private static int DecimalPlaces(double value)
    {
        var text = value.ToString("G15", CultureInfo.InvariantCulture);
        var decimalIndex = text.IndexOf('.');
        return decimalIndex >= 0 ? text.Length - decimalIndex - 1 : 0;
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

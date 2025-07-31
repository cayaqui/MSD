namespace ValueObjects.Common;

/// <summary>
/// Value object representing a date range
/// </summary>
public record DateRange
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after or equal to start date", nameof(endDate));

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public int Days => (EndDate - StartDate).Days + 1;
    public int Months => ((EndDate.Year - StartDate.Year) * 12) + EndDate.Month - StartDate.Month + 1;

    public bool Contains(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate && checkDate <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public DateRange? Intersection(DateRange other)
    {
        if (!Overlaps(other))
            return null;

        var start = StartDate > other.StartDate ? StartDate : other.StartDate;
        var end = EndDate < other.EndDate ? EndDate : other.EndDate;

        return new DateRange(start, end);
    }

    public static DateRange ForMonth(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return new DateRange(start, end);
    }

    public static DateRange ForYear(int year)
    {
        return new DateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
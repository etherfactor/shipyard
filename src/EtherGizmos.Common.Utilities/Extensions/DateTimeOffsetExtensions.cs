namespace EtherGizmos.Common.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset Ceiling(
        this DateTimeOffset @this,
        TimeSpan interval)
    {
        long ticks = (@this.Ticks + interval.Ticks - 1) / interval.Ticks;

        return new DateTimeOffset(ticks * interval.Ticks, @this.Offset);
    }

    public static DateTimeOffset Floor(
        this DateTimeOffset @this,
        TimeSpan interval)
    {
        long ticks = @this.Ticks / interval.Ticks;

        return new DateTimeOffset(ticks * interval.Ticks, @this.Offset);
    }

    public static DateTimeOffset Round(
        this DateTimeOffset @this,
        TimeSpan interval)
    {
        long ticks = (@this.Ticks + interval.Ticks / 2 + 1) / interval.Ticks;

        return new DateTimeOffset(ticks * interval.Ticks, @this.Offset);
    }
}

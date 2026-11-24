using System.Globalization;
using Booking.Domain.Common;

namespace Booking.Domain.Resources;

public sealed class OpeningHours
{
    private static readonly TimeSpan QuarterHour = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan EndOfDay = TimeSpan.FromDays(1);
    public Guid Id { get; }
    public DayOfWeek DayOfWeek { get; }
    public TimeOnly OpensAt { get; }
    public TimeOnly ClosesAt { get; }
    public TimeSpan StartOffset => OpensAt.ToTimeSpan();
    public TimeSpan EndOffset => ToEndOffset(ClosesAt);

    public OpeningHours(DayOfWeek dayOfWeek, TimeOnly opensAt, TimeOnly closesAt)
    {
        if (!Enum.IsDefined(dayOfWeek))
            throw new DomainException($"Invalid day of week: {(int)dayOfWeek}");

        if (!IsAlignedToQuarterHour(opensAt) || !IsAlignedToQuarterHour(closesAt))
            throw new DomainException(
                $"Opening hours must be aligned to 15 minutes, got {Format(opensAt)}–{Format(closesAt)}.");

        if (opensAt.ToTimeSpan() >= ToEndOffset(closesAt))
            throw new DomainException(
                $"Opening time {Format(opensAt)} must be before closing time {Format(closesAt)}.");

        DayOfWeek = dayOfWeek;
        OpensAt = opensAt;
        ClosesAt = closesAt;
        Id = Guid.CreateVersion7();
    }

    private OpeningHours()
    {
    }

    public override string ToString() =>
        $"{DayOfWeek} {Format(OpensAt)}–{(EndOffset == EndOfDay ? "24:00:00" : Format(ClosesAt))}";

    public bool Overlaps(OpeningHours other) =>
        DayOfWeek == other.DayOfWeek
        && StartOffset < other.EndOffset
        && other.StartOffset < EndOffset;

    private static bool IsAlignedToQuarterHour(TimeOnly time) =>
        time.Ticks % QuarterHour.Ticks == 0;

    private static string Format(TimeOnly time) =>
        time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

    private static TimeSpan ToEndOffset(TimeOnly closesAt) =>
        closesAt == TimeOnly.MinValue ? EndOfDay : closesAt.ToTimeSpan();
}
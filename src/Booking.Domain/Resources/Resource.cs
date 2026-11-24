using Booking.Domain.Common;

namespace Booking.Domain.Resources;

public sealed class Resource
{
    public const int NameMaxLength = 200;

    private readonly List<OpeningHours> _openingHours = [];

    public Guid Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ResourceType Type { get; private set; }
    public int? Capacity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyCollection<OpeningHours> OpeningHours => _openingHours.AsReadOnly();

    public Resource(string name, ResourceType type, string? description, int? capacity, DateTimeOffset createdAt)
    {
        if (!Enum.IsDefined(type))
            throw new DomainException($"Resource type {type} is not defined.");

        if (capacity is <= 0)
            throw new DomainException($"Resource capacity must be greater than zero, got {capacity}.");

        if (createdAt.Offset != TimeSpan.Zero)
            throw new DomainException($"Created at {createdAt} is negative.");

        Name = NormalizeName(name);
        Type = type;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Capacity = capacity;
        CreatedAt = createdAt;
        IsActive = true;
        Id = Guid.CreateVersion7();
    }

    private Resource()
    {
        Name = null!;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void SetOpeningHours(IEnumerable<OpeningHours> openingHours)
    {
        ArgumentNullException.ThrowIfNull(openingHours);
        var newHours = openingHours.ToList();
        for (var i = 0; i < newHours.Count; i++)
        {
            for (var j = i + 1; j < newHours.Count; j++)
            {
                if (newHours[i].Overlaps(newHours[j]))
                    throw new DomainException(
                        $"Opening hours overlap: {newHours[i]} and {newHours[j]}.");
            }
        }

        _openingHours.Clear();
        _openingHours.AddRange(newHours);
    }

    private static string NormalizeName(string? name)
    {
        var normalized = name?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
            throw new DomainException("Resource name is required.");

        if (normalized.Length > NameMaxLength)
            throw new DomainException(
                $"Resource name cannot exceed {NameMaxLength} characters, got {normalized.Length}.");

        return normalized;
    }
}
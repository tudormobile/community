namespace Tudormobile.CommunityService;

/// <summary>
/// Represents a collection of community location matches from a query.
/// </summary>
public class CommunityMatchCollection
{
    /// <summary>
    /// Gets a value indicating whether the match is exact or partial.
    /// </summary>
    public bool IsExact { get; init; }

    /// <summary>
    /// Gets the collection of matching community locations.
    /// </summary>
    public IReadOnlyList<CommunityLocation> Matches { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityMatchCollection"/> class with no matches.
    /// </summary>
    internal CommunityMatchCollection()
    {
        Matches = [];
        IsExact = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityMatchCollection"/> class with the specified matches.
    /// </summary>
    /// <param name="matches">The collection of matching locations.</param>
    /// <param name="isExactMatch">Indicates whether the matches are exact.</param>
    public CommunityMatchCollection(IEnumerable<CommunityLocation> matches, bool isExactMatch = false)
    {
        Matches = [.. matches];
        IsExact = isExactMatch;
    }
}

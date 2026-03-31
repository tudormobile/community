namespace Tudormobile.CommunityService;

public class CommunityMatchCollection
{
    public bool IsExact { get; init; }
    public IReadOnlyList<CommunityLocation> Matches { get; init; }
    public CommunityMatchCollection()
    {
        Matches = [];
        IsExact = false;
    }

    public CommunityMatchCollection(IEnumerable<CommunityLocation> matches, bool isExactMatch = false)
    {
        Matches = [.. matches];
        IsExact = isExactMatch;
    }
}

namespace Tudormobile.CommunityService;

public class CommunityMatchCollection
{
    public bool Exact { get; init; }
    public IReadOnlyList<CommunityLocation> Matches { get; init; }
    public CommunityMatchCollection()
    {
        Matches = [];
        Exact = false;
    }

    public CommunityMatchCollection(IEnumerable<CommunityLocation> matches, bool isExactMatch = false)
    {
        Matches = [.. matches];
        Exact = isExactMatch;
    }
}

namespace People.Domain.ValueObjects;

public sealed record PersonName(string First, string Last)
{
    public override string ToString() => $"{First} {Last}".Trim();
}



namespace People.Domain.ValueObjects;

public sealed record Email(string Value)
{
    public override string ToString() => Value;
}

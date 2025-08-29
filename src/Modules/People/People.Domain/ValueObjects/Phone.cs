

namespace People.Domain.ValueObjects;
public sealed record Phone(string Value)
{
    public override string ToString() => Value;
}

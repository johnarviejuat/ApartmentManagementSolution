namespace Leasing.Application.Common
{
    public sealed record AddressDto(
        string Line1, 
        string City, 
        string State, 
        string PostalCode
    );
}

using System.Security.Cryptography;

namespace ApartmentManagement.Domain.Leasing.Payments
{
    public static class PaymentReference
    {
        public static string New()
        {
            var bytes = RandomNumberGenerator.GetBytes(5);
            var token = Convert.ToHexString(bytes)[..6];
            return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{token}";
        }
    }
}

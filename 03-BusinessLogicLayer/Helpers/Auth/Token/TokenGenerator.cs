using System;

namespace HolyShift
{
    public class TokenGenerator
    {
        public static string GenerateStringToken(EmployeeModel employee, int minutesToExpiration)
        {
            byte[] privateKey = KeyStore.PrivateKey;
            using (AccessTokenCreator accessTokenCreator = new AccessTokenCreator(KeyStore.PrivateKey))
            {
                return accessTokenCreator.Encode(minutesToExpiration > 0 ? DateTime.UtcNow.AddMinutes(minutesToExpiration) : DateTime.MaxValue, employee.Email, employee.EmployeeId.ToString(), employee.Role);
            }
        }
    }
}

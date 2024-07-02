using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    public class AuthLogic : BaseLogic
    {
        public AuthLogic(HolyShiftContext db) : base(db) { }
        public bool IsAuthenticated(CredentialsModel creds)
        {
            Employee employee = DB.Employees.FirstOrDefault(p => p.Username == creds.Username);
            if (employee == null) { return false; }
            if (!VerifyPasswordHash(creds.Password, employee.PasswordHash)) return false;
            return true;
        }


        private static bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (storedHash == null) return false;
            try
            {
                byte[] computedHash = EncryptDecryptHelper.DerivePublicKey(password);
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

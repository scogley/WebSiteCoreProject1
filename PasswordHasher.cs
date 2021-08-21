using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
// See https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0

namespace WebSiteCoreProject1
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashedPassword { get; }
        public string Salt { get; private set; }

        public bool IsHashMatch { get; private set; }

        
        public PasswordHasher(string clearTextPass)
        {
            HashedPassword = this.HashPassword(clearTextPass);
        }

        public PasswordHasher(string clearTextPass, string correctHash, string salt)
        {
            this.VerifyPassword(clearTextPass, correctHash, salt);
        }

        private string HashPassword(string password)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string hashed = CreateHash(password, salt);
            Salt = Convert.ToBase64String(salt);

            return hashed;
        }

        private static string CreateHash(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public void VerifyPassword(string clearTextPass, string correctHash, string salt)
        {
            // Convert the salt string to a byte array and run the hashing algorithm
            // Compare correctHash with result and return bool
            byte[] saltByteArray = Convert.FromBase64String(salt);
            string hashed = CreateHash(clearTextPass, saltByteArray);
            IsHashMatch = hashed == correctHash;
        }
    }
}

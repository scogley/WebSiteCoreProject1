using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebSiteCoreProject1
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashedPassword { get; }
        public string Salt { get; private set; }

        public PasswordHasher(string password)
        {
            HashedPassword = this.HashPassword(password);
        }

        private string HashPassword(string password)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Loop through each byte in the array and format each as hexadecimal string.
            string saltString = "";
            foreach (byte b in salt)
            {
                saltString += b.ToString() + "x2";
            }
            Salt = saltString;

            return hashed;
        }

        public bool VerifyPassword(string password, string correctHash)
        {
            return false;
        }
    }


}

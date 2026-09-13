using System;
using System.Security.Cryptography;
using System.Text;

namespace EBM2x.Modules
{
    public static class CryptoTool
    {
        private const int SaltSize = 31;
        private const string defaultHashMethod = "SHA";

        public static string createNewSalt(int size = SaltSize)
        {
            // use the crypto random number generator to create
            // a new random salt 
            using (var rng = new RNGCryptoServiceProvider())
            {
                // dont allow very small salt
                var data = new byte[size < 7 ? 7 : size + 1];
                // fill the array
                rng.GetBytes(data);
                // convert to B64 for saving as text
                return Convert.ToBase64String(data);
            }
        }

        public static string getMD5Hash(string theInput, string theSalt)
        {
            using (var hasher = MD5.Create())    // create hash object
            {
                var dbytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput + theSalt));
                return Convert.ToBase64String(dbytes);
            }
        }

        public static bool checkMD5Hash(string hashedStr, string newInput, string theSalt)
        {
            // get the hash value of user input: 
            string newHash = getMD5Hash(newInput, theSalt);

            // return comparison
            return string.Compare(newHash, hashedStr, true) == 0;
        }

        public static string getSHAHash(string theInput, string theSalt)
        {
            string tmp = theInput + theSalt;

            // or SHA512Managed
            using (HashAlgorithm hash = new SHA256Managed())
            {
                // convert pw+salt to bytes:
                var saltyPW = Encoding.UTF8.GetBytes(tmp);
                // hash the pw+salt bytes:
                var hBytes = hash.ComputeHash(saltyPW);
                // return a B64 string so it can be saved as text 
                return Convert.ToBase64String(hBytes);
            }
        }

        public static bool checkSHAHash(string hashedStr, string newInput, string theSalt)
        {
            // get the hash value of user input: 
            string newHash = getSHAHash(newInput, theSalt);

            // return comparison
            return string.Compare(newHash, hashedStr, true) == 0;
        }

        public static string encryptPassword(string strPlainPass, string method = defaultHashMethod)
        {
            string salt = createNewSalt();
            if ((method ?? "") == "SHA")
                return salt + ":" + getSHAHash(strPlainPass, salt);
            else
                return salt + ":" + getMD5Hash(strPlainPass, salt);
        }

        public static bool comparePassword(string strPassword, string realEncryptedPasswordwithSalt, string method = defaultHashMethod)
        {
            var part = realEncryptedPasswordwithSalt.Split(':');
            if (part.Length == 1)
                // password is not encrypted, use plain text comparison
                return (strPassword ?? "") == (realEncryptedPasswordwithSalt ?? "");
            else if ((method ?? "") == "SHA")
                return checkSHAHash(part[1], strPassword, part[0]);
            else
                return checkMD5Hash(part[1], strPassword, part[0]);
        }
    }
}

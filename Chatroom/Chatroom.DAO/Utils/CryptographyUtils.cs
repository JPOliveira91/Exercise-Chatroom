using System.Security.Cryptography;
using System.Text;

namespace Chatroom.DAO
{
    public class CryptographyUtils
    {
        public string EncryptPassword(string password)
        {
            // Source: https://www.ti-enxame.com/pt/c%23/como-posso-sha512-uma-string-em-c/1068643647/
            var bytes = Encoding.UTF8.GetBytes(password);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new StringBuilder(128);

                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));

                return hashedInputStringBuilder.ToString();
            }
        }
    }
}
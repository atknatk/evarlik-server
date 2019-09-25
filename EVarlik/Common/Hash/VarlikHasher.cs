using System;
using System.Security.Cryptography;
using System.Text;

namespace EVarlik.Common.Hash
{
    public class VarlikHasher
    {

        public static string Hash(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString.ToUpper();
        }

    }
}
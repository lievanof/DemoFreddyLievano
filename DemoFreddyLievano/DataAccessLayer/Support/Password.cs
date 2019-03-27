using System;

namespace Demo.DataAccessLayer.Utils
{
    public static class PasswordGenerator
    {

        public static string Encrypt(string userPassword)
        {

            byte[] data = System.Text.Encoding.ASCII.GetBytes(userPassword);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);

            return hash;
        }
    }
}
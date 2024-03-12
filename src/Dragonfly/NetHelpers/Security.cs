namespace Dragonfly.NetHelpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Security
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Security";

        public static string MD5Hash(string data)
        {
            string ReturnHashed = "";
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(data);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            ReturnHashed = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return ReturnHashed;
        }

        /// <summary>
        /// take any string and encrypt it using MD5 then
        /// return the encrypted data 
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        public static string GetMD5HashData(string data)
        {
            //original code from http://www.codeproject.com/Articles/38951/How-To-Hash-Data-Using-MD5-and-SHA1
            //create new instance of md5
            MD5 md5 = MD5.Create();

            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();

        }

        /// <summary>
        /// encrypt input text using MD5 and compare it with
        /// the stored encrypted text
        /// </summary>
        /// <param name="inputData">input text you will enterd to encrypt it</param>
        /// <param name="storedHashData">the encrypted text
        ///         stored on file or database ... etc</param>
        /// <returns>true or false depending on input validation</returns>
        public static bool ValidateMD5HashData(string inputData, string storedHashData)
        {
            //original code from http://www.codeproject.com/Articles/38951/How-To-Hash-Data-Using-MD5-and-SHA1
            //hash input text and save it string variable
            string getHashInputData = GetMD5HashData(inputData);

            if (string.Compare(getHashInputData, storedHashData) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
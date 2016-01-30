using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Encryption
    {
        /// <summary>
        /// iteration count for deriving key material
        /// </summary>
        private const int KEY_DERIVATION_ITERATION = 4816;
        /// <summary>
        /// Encryption key size (in bits)
        /// </summary>
        private const int KEY_SIZE = 256;
        /// <summary>
        /// IV size (in bits)
        /// </summary>
        private const int IV_SIZE = 128;
        /// <summary>
        /// IV length (in bytes)
        /// </summary>
        private const int IV_LENGTH = IV_SIZE / 8;
        /// <summary>
        /// Salt length (in bytes)
        /// </summary>
        private const int SALT_LENGTH = 32;
        /// <summary>
        /// Name of algorithm in use
        /// </summary>
        private const string ALGORITHM_NAME = "AES256";


        /// <summary>
        /// Generates a key from a password and salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltBytes"></param>
        /// <returns></returns>
        private static ParametersWithIV GenerateKey(string password, byte[] saltBytes)
        {
            var passBytes = PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password.ToCharArray());

            //create key generator
            var generator = new Pkcs5S2ParametersGenerator();
            //initialize
            generator.Init(passBytes, saltBytes, KEY_DERIVATION_ITERATION);

            //generate with a 256bit key, and a 128bit IV
            var kp = (ParametersWithIV)generator.GenerateDerivedParameters(ALGORITHM_NAME, KEY_SIZE, IV_SIZE);

            return kp;
        }

        /// <summary>
        /// Generates a key from a password and salt and IV
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltBytes"></param>
        /// <param name="ivBytes"></param>
        /// <returns></returns>
        private static ParametersWithIV GenerateKey(string password, byte[] saltBytes, byte[] ivBytes)
        {
            var passBytes = PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password.ToCharArray());

            //create key generator
            var generator = new Pkcs5S2ParametersGenerator();
            //initialize
            generator.Init(passBytes, saltBytes, KEY_DERIVATION_ITERATION);

            //generate with a 256bit key, and a 128bit IV
            var kp = new ParametersWithIV(generator.GenerateDerivedParameters(ALGORITHM_NAME, KEY_SIZE), ivBytes);

            return kp;
        }

        /// <summary>
        /// Encrypts using AES256Cbc and a password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string plainText, string password)
        {
            // If text to encrypt is null, return null
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return null;
            }


            byte[] saltBytes = new byte[SALT_LENGTH];

            //create random byte generator
            var rand = new SecureRandom();
            //get random bytes for our salt
            rand.NextBytes(saltBytes);

            //create cipher engine
            var cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(
                    new AesEngine()));

            //get the key parameters from the password
            var key = GenerateKey(password, saltBytes);

            //initialize for encryption with the key
            cipher.Init(true, key);

            //get the message as bytes
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            MemoryStream cipherStream;
            //process the input
            using (cipherStream = new MemoryStream())
            {
                //write iv
                cipherStream.Write(key.GetIV(), 0, key.GetIV().Length);
                //write salt
                cipherStream.Write(saltBytes, 0, saltBytes.Length);

                byte[] outputBytes;
                //get output
                outputBytes = cipher.ProcessBytes(plainBytes);

                if (outputBytes != null)
                {
                    //write the data to the stream
                    cipherStream.Write(outputBytes, 0, outputBytes.Length);
                }
                
                //do the final block
                outputBytes = cipher.DoFinal();

                if (outputBytes != null)
                {
                    //write the data to the stream
                    cipherStream.Write(outputBytes, 0, outputBytes.Length);
                }

            }

            //return the bytes
            return cipherStream.ToArray();
        }

        /// <summary>
        /// Decrypts cypher data
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] cipherData, string password)
        {
            // If there is no cipher data, return null
            if (cipherData == null)
            {
                return null;
            }

            //extract the iv and salt
            byte[] ivBytes = new byte[IV_LENGTH];
            byte[] saltBytes = new byte[SALT_LENGTH];
            byte[] cipherBytes = new byte[cipherData.Length - (ivBytes.Length + saltBytes.Length)];

            //process the input
            using (var cipherStream = new MemoryStream(cipherData))
            {
                //read iv
                cipherStream.Read(ivBytes, 0, ivBytes.Length);
                //read salt
                cipherStream.Read(saltBytes, 0, saltBytes.Length);
                //read cipher bytes
                cipherStream.Read(cipherBytes, 0, cipherBytes.Length);

            }

            //create cipher engine
            var cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(
                    new AesEngine()));

            //get the key parameters from the password
            var key = GenerateKey(password, saltBytes, ivBytes);

            //initialize for decryption with the key
            cipher.Init(false, key);

            MemoryStream plainStream;
            //process the input
            using (plainStream = new MemoryStream())
            {
                byte[] outputBytes;
                //get output
                outputBytes = cipher.ProcessBytes(cipherBytes);

                if (outputBytes != null)
                {
                    //write the data to the stream
                    plainStream.Write(outputBytes, 0, outputBytes.Length);
                }

                //do the final block
                outputBytes = cipher.DoFinal();

                if (outputBytes != null)
                {
                    //write the data to the stream
                    plainStream.Write(outputBytes, 0, outputBytes.Length);
                }

            }


            return Encoding.UTF8.GetString(plainStream.ToArray());
        }


        #region Helpers
        /// <summary>
        /// Encrypts data and returns a base64 string
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptToString(string plainText, string password)
        {
            var data = Encrypt(plainText, password);
            if (data != null)
            {
                return Convert.ToBase64String(data);
            }

            return null;
        }

        /// <summary>
        /// Decrypt data from a base64 string
        /// </summary>
        /// <param name="cypherString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DecryptFromString(string cypherString, string password)
        {
            if (!string.IsNullOrWhiteSpace(cypherString))
            {
                return Decrypt(Convert.FromBase64String(cypherString), password);
            }

            return null;
        }
        #endregion
    }
}
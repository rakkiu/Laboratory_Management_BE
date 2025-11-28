using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Security
{
    /// <summary>
    /// encryption helper class for encrypting and decrypting data
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// The key bytes
        /// </summary>
        private static byte[]? _keyBytes;

        /// <summary>
        /// Configures the encryption key (base64 string from environment).
        /// Must be called once on startup.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Encryption key cannot be null or empty.
        /// or
        /// Encryption key must be 32 bytes (Base64 of AES-256 key).
        /// </exception>
        public static void ConfigureKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Encryption key cannot be null or empty.");

            var keyBytes = Convert.FromBase64String(key);
            if (keyBytes.Length != 32)
                throw new InvalidOperationException("Encryption key must be 32 bytes (Base64 of AES-256 key).");

            _keyBytes = keyBytes;
        }

        /// <summary>
        /// Encrypts plain text using AES with random IV (more secure, non-deterministic).
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Encryption key not configured.</exception>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            if (_keyBytes is null)
                throw new InvalidOperationException("Encryption key not configured.");

            using var aes = Aes.Create();
            aes.Key = _keyBytes;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts AES-encrypted text (with IV included in the front).
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Encryption key not configured.</exception>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
            if (_keyBytes is null)
                throw new InvalidOperationException("Encryption key not configured.");

            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _keyBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Deterministic AES encryption (fixed IV = all zeros).
        /// Use only for searchable fields like Email.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Encryption key not configured.</exception>
        public static string EncryptDeterministic(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            if (_keyBytes is null)
                throw new InvalidOperationException("Encryption key not configured.");

            using var aes = Aes.Create();
            aes.Key = _keyBytes;
            aes.IV = new byte[16]; // fixed IV (deterministic)
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
        /// <summary>
        /// Decrypts the deterministic.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        public static string DecryptDeterministic(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var cipherBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _keyBytes!;
            aes.IV = new byte[16]; // fixed IV = zeros
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}

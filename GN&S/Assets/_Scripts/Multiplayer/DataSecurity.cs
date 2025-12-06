using System;
using System.IO;
using System.Security.Cryptography;

namespace _Scripts.Multiplayer
{
    public static class DataSecurity
{
    
    //define key and IV for encryption and decryption
    private static readonly byte[] EncryptionKey = Convert.FromBase64String("V2VIY2w4dUZ0cGV6U01yWjZ3R0ZkM2Z3Szl2UG5rU3I="); 
    
    private static readonly byte[] EncryptionIV = Convert.FromBase64String("SGF2ZSBhIGdvb2QgZGF5IQ=="); // The original IV is valid
    

    /// <summary>
    /// Encrypts the input data using AES-256 with defined Key and IV.
    /// </summary>
    /// <param name="dataToEncrypt">The raw byte array.</param>
    /// <returns>The encrypted byte array.</returns>
    public static byte[] Encrypt(byte[] dataToEncrypt)
    {
        return EncryptAes(dataToEncrypt, EncryptionKey, EncryptionIV);
    }

    /// <summary>
    /// Decrypts the input data using AES-256 with defined Key and IV.
    /// </summary>
    /// <param name="dataToDecrypt">The encrypted byte array.</param>
    /// <returns>The decrypted byte array</returns>
    public static byte[] Decrypt(byte[] dataToDecrypt)
    {
        return DecryptAes(dataToDecrypt, EncryptionKey, EncryptionIV);
    }
    
    private static byte[] EncryptAes(byte[] dataToEncrypt, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;
        aesAlg.Mode = CipherMode.CBC; 

        aesAlg.Padding = PaddingMode.PKCS7;
        
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new MemoryStream();
        using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
        csEncrypt.FlushFinalBlock();
        return msEncrypt.ToArray(); // Returns the encrypted bytes
    }

    private static byte[] DecryptAes(byte[] dataToDecrypt, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;
        aesAlg.Mode = CipherMode.CBC;

        aesAlg.Padding = PaddingMode.PKCS7;
        
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        try
        {
            using (MemoryStream msDecrypt = new MemoryStream(dataToDecrypt))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream msOriginal = new MemoryStream())
                    {
                        // Read the decrypted bytes from the CryptoStream
                        csDecrypt.CopyTo(msOriginal); 
                        return msOriginal.ToArray(); // Returns the original, decrypted bytes
                    }
                }
            }
        }
        catch (CryptographicException)
        {
            throw new CryptographicException("Decryption failed. The file is corrupted or tampered with.");
        }
    }
}
}
using Marketing_system.BL.Contracts.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Service;

public class EncryptionService : IEncryptionService
{
    private readonly string _encryptionKey; // Ključ za šifrovanje

    public EncryptionService(string encryptionKey)
    {
        _encryptionKey = encryptionKey;
    }

    public string Encrypt(string input)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aes.IV = new byte[16]; // Inicijalizacioni vektor

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] encryptedBytes;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                cs.Write(inputBytes, 0, inputBytes.Length);
            }
            encryptedBytes = ms.ToArray();
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string input)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aes.IV = new byte[16]; // Inicijalizacioni vektor

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        byte[] decryptedBytes;
        using (var ms = new MemoryStream(Convert.FromBase64String(input)))
        {
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using (var reader = new StreamReader(cs))
                {
                    decryptedBytes = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                }
            }
        }

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}

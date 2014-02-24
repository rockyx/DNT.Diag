using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace DNT.Diag.DB
{
  public abstract class AbstractDB
  {
    byte[] AES_CBC_KEY = 
    {
      0xFA, 0xC2, 0xCC, 0x82,
      0x8C, 0xFD, 0x42, 0x17,
      0xA0, 0xB2, 0x97, 0x4D,
      0x19, 0xC8, 0xA4, 0xB1,
      0xF5, 0x73, 0x23, 0x7C,
      0xB1, 0xC4, 0xC0, 0x38,
      0xC9, 0x80, 0xB9, 0xF7,
      0xC3, 0x3E, 0xC9, 0x12
    };

    byte[] AES_CBC_IV = 
    {
      0x7C, 0xF4, 0xF0, 0x7D,
      0x3B, 0x0D, 0xA1, 0xC6,
      0x35, 0x74, 0x18, 0xB3,
      0x51, 0xA3, 0x87, 0x8E
    };

    static Dictionary<string, byte[]> _encrypt;

    static AbstractDB()
    {
      _encrypt = new Dictionary<string, byte[]>();
    }

    protected byte[] DecryptToBytes(byte[] cipher)
    {
      if ((cipher == null) || (cipher.Length <= 0))
        throw new ArgumentNullException("CipherText");

      // Declare the string used to hold
      // the decrypted text.
      byte[] plainBytes = null;

      // Create an Aes object
      // with the specified key and IV.
      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = AES_CBC_KEY;
        aesAlg.IV = AES_CBC_IV;
        aesAlg.Padding = PaddingMode.Zeros;

        // Create a decryptor to perform the stream transform.
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for decryption.
        using (MemoryStream msDecrypt = new MemoryStream(cipher))
        {
          byte[] buffer = new byte[1024];
          int readBytes = 0;
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (MemoryStream utf8Memory = new MemoryStream())
            {
              while ((readBytes = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
              {
                utf8Memory.Write(buffer, 0, readBytes);
              }

              plainBytes = utf8Memory.ToArray();
            }
          }
        }
      }

      return plainBytes;
    }

    protected string DecryptToString(byte[] cipher)
    {
      var plainBytes = DecryptToBytes(cipher);
      string temp = UTF8Encoding.UTF8.GetString(plainBytes);
      return temp.TrimEnd((char)0x00);
    }

    protected byte[] Encrypt(byte[] plain)
    {
      if ((plain == null) || (plain.Length <= 0))
        throw new ArgumentNullException("Plain");

      byte[] encrypted;

      // Create an Aes object
      // with the specified key and IV.
      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = AES_CBC_KEY;
        aesAlg.IV = AES_CBC_IV;
        aesAlg.Padding = PaddingMode.Zeros;

        // Create a encryptor to perform the stream transform.
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        // Create the streams used for encryption.
        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            csEncrypt.Write(plain, 0, plain.Length);
            csEncrypt.FlushFinalBlock();
            encrypted = msEncrypt.ToArray();
          }
        }
      }

      // Return the encrypted bytes from the memory stream.
      return encrypted;
    }

    protected byte[] Encrypt(string plain)
    {
      if (String.IsNullOrEmpty(plain))
        throw new ArgumentNullException("Plain");

      if (!_encrypt.ContainsKey(plain))
      {
        byte[] plainBytes = UTF8Encoding.UTF8.GetBytes(plain);
        _encrypt[plain] = Encrypt(plainBytes);
      }

      return _encrypt[plain];
    }
  }
}

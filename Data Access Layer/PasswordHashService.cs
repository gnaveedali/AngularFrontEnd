﻿using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using System.Text;

namespace Data_Access_Layer
{
    public class PasswordHashService
    {
        private readonly SecureRandom Random = new SecureRandom();

        // Pre-configured Encryption Parameters
        public readonly int NonceBitSize = 128;
        public readonly int MacBitSize = 128;
        public readonly int KeyBitSize = 256;

        public PasswordHashService() { }

        public byte[] NewKey()
        {
            var key = new byte[KeyBitSize / 8];
            Random.NextBytes(key);
            return key;
        }

        public byte[] NewIv()
        {
            var iv = new byte[NonceBitSize / 8];
            Random.NextBytes(iv);
            return iv;
        }

        public Byte[] HexToByte(string hexStr)
        {
            byte[] bArray = new byte[hexStr.Length / 2];
            for (int i = 0; i < (hexStr.Length / 2); i++)
            {
                byte firstNibble = Byte.Parse(hexStr.Substring((2 * i), 1),
                                   System.Globalization.NumberStyles.HexNumber); // [x,y)
                byte secondNibble = Byte.Parse(hexStr.Substring((2 * i) + 1, 1),
                                    System.Globalization.NumberStyles.HexNumber);
                int finalByte = (secondNibble) | (firstNibble << 4); // bit-operations 
                                                                     // only with numbers, not bytes.
                bArray[i] = (byte)finalByte;
            }
            return bArray;
        }

        public string toHex(byte[] data)
        {
            string hex = string.Empty;
            foreach (byte c in data)
            {
                hex += c.ToString("X2");
            }
            return hex;
        }

        public string toHex(string asciiString)
        {
            string hex = string.Empty;
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public string encrypt(string PlainText, byte[] key, byte[] iv)
        {
            string sR = string.Empty;
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(PlainText);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters =
                             new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(true, parameters);

                byte[] encryptedBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
                Int32 retLen = cipher.ProcessBytes
                               (plainBytes, 0, plainBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, retLen);
                sR = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return sR;
        }

        public string decrypt(string EncryptedText, byte[] key, byte[] iv)
        {
            string sR = string.Empty;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(EncryptedText);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters =
                          new AeadParameters(new KeyParameter(key), 128, iv, null);
                //ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(key), iv);

                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                Int32 retLen = cipher.ProcessBytes
                               (encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return sR;
        }
    }
}

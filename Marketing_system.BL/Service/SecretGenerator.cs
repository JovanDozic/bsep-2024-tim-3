using System.Security.Cryptography;
using System.Text;

namespace Marketing_system.BL.Service
{
    public static class SecretGenerator
    {
        private const string Base32AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string GenerateSecretKey(int length = 16)
        {
            using var random = RandomNumberGenerator.Create();
            byte[] tokenData = new byte[length];
            random.GetBytes(tokenData);
            return Base32Encode(tokenData);
        }

        private static string Base32Encode(byte[] data)
        {
            int inByteSize = 8;
            int outByteSize = 5;
            int bitLength = data.Length * inByteSize;
            StringBuilder result = new(bitLength / outByteSize);

            int buffer = data[0];
            int next = 1;
            int bitsLeft = inByteSize;
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < outByteSize)
                {
                    if (next < data.Length)
                    {
                        buffer <<= inByteSize;
                        buffer |= data[next++] & 0xff;
                        bitsLeft += inByteSize;
                    }
                    else
                    {
                        int pad = outByteSize - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = 0x1F & (buffer >> (bitsLeft - outByteSize));
                bitsLeft -= outByteSize;
                result.Append(Base32AllowedChars[index]);
            }
            return result.ToString();
        }
    }
}

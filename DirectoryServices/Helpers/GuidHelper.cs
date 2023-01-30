using System;

namespace DirectoryServices.Helpers
{
    /// <summary>
    /// A helper class to decode and encode guid strings
    /// </summary>
    public class GuidHelper
    {
        public static byte[] Encode(string? guidString)
        {
            var guid = new Guid(guidString);

            return Encode(guid);
        }


        /// <summary>
        /// Decode a binary guid from LDAP into a nice dashed guid string.
        /// </summary>
        /// <param name="bytes">Binary guid</param>
        /// <returns>A guid string</returns>
        public static string? Decode(byte[] bytes)
        {
            return (PrefixZeros((int)bytes[3] & 0xFF)
                + PrefixZeros((int)bytes[2] & 0xFF)
                + PrefixZeros((int)bytes[1] & 0xFF)
                + PrefixZeros((int)bytes[0] & 0xFF)
                + "-"
                + PrefixZeros((int)bytes[5] & 0xFF)
                + PrefixZeros((int)bytes[4] & 0xFF)
                + "-"
                + PrefixZeros((int)bytes[7] & 0xFF)
                + PrefixZeros((int)bytes[6] & 0xFF)
                + "-"
                + PrefixZeros((int)bytes[8] & 0xFF)
                + PrefixZeros((int)bytes[9] & 0xFF)
                + "-"
                + PrefixZeros((int)bytes[10] & 0xFF)
                + PrefixZeros((int)bytes[11] & 0xFF)
                + PrefixZeros((int)bytes[12] & 0xFF)
                + PrefixZeros((int)bytes[13] & 0xFF)
                + PrefixZeros((int)bytes[14] & 0xFF)
                + PrefixZeros((int)bytes[15] & 0xFF)).ToLower();
        }

        private static byte[] Encode(Guid guid)
        {
            return guid.ToByteArray();
        }

        private static string? PrefixZeros(int value)
        {
            if (value <= 0xF)
            {
                return "0" + value.ToString("X");
            }

            return value.ToString("X");
        }
    }
}
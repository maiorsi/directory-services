using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DirectoryServices.Helpers
{
    /// <summary>
    /// Helper class to decode secure identifiers.
    /// </summary>
    public static class SecureIdentifierHelper
    {
        private const string SID_PATTERN = "^[sS]-\\d-\\d{1,13}(?:-\\d{1,10})*$";

        /// <summary>
        /// Decode a secure identifier (SId) into string format 'S-1-*'.
        /// </summary>
        /// <param name="secureIdentifierBytes">The binary secure identifier</param>
        /// <returns>The string format of the secure identifier</returns>
        public static string? Decode(byte[] secureIdentifierBytes)
        {
            var sid = new StringBuilder("S-");

            // Add SId revision
            sid.Append(secureIdentifierBytes[0]);

            var subAuthorityCount = secureIdentifierBytes[1] & 0xFF;

            // bytes 2-7 - 48 Bit authority ([big-endian])
            var authority = 0L;

            // String rid = ""
            for (var i = 2; i <= 7; i++)
            {
                authority |= ((long)secureIdentifierBytes[i]) << (8 * (5 - (i - 2)));
            }

            sid.Append('-');
            sid.Append(string.Format("{0:X}", authority));

            var offset = 8;
            var size = 4; // 4 bytes for each sub authority

            for (var j = 0; j < subAuthorityCount; j++)
            {
                var subAuthority = 0L;

                for (var k = 0; k < size; k++)
                {
                    subAuthority |= (long)(secureIdentifierBytes[offset + k] & 0xFF) << (8 * k);
                }

                // Format it!
                sid.Append('-');
                sid.Append(subAuthority);

                offset += size;
            }

            return sid.ToString();
        }

        /// <summary>
        /// Encode a SId string format secure identifier to binary format.
        /// </summary>
        /// <param name="secureIdentifierString"></param>
        /// <returns>Binary format (byte[]) of secure identifier (SId)</returns>
        public static byte[] Encode(string? secureIdentifierString)
        {
            byte[] bytes = new byte[0];

            if (string.IsNullOrEmpty(secureIdentifierString))
            {
                return bytes;
            }

            if (!Regex.IsMatch(secureIdentifierString, SID_PATTERN))
            {
                return bytes;
            }

            string[] secureIdentifierSections = secureIdentifierString.Split("-");

            // Init with sub authority count
            var count = secureIdentifierSections.Length - 3;

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            // Big endian to little endian.
            // By default BitConverter/BinaryWriter work as little endian but we expect the first 8 bytes to be big endian.
            byte[] endianBytes = BitConverter.GetBytes(long.Parse(secureIdentifierSections[2]));

            Array.Reverse(endianBytes);

            binaryWriter.Write(endianBytes);

            for (var i = 0; i < count; i++)
            {
                binaryWriter.Write((int)long.Parse(secureIdentifierSections[i + 3]));
            }

            bytes = memoryStream.ToArray();

            bytes[0] = (byte)short.Parse(secureIdentifierSections[1]);
            bytes[1] = (byte)count;

            return bytes;
        }
    }
}
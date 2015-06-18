using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicesLayer
{
    /// <summary>
    /// Utility class for manipulation with strings.
    /// </summary>
    public class StringUtil
    {

        /// <summary>
        /// Gets byte array or specified string.
        /// </summary>
        /// <param name="str">string to be converted</param>
        /// <returns>byte array of the string</returns>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Gets string from specified byte array.
        /// </summary>
        /// <param name="bytes">bytes to be converted</param>
        /// <returns>string from the byte array</returns>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}

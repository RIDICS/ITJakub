using System;
using System.IO;
using System.Reflection;

namespace Jewelery
{
    public static class MyExtensions
    {
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs != null && attribs.Length > 0 ? attribs[0].StringValue : null;
        }


        /// <summary>
        /// Converts selected stream to byte array.
        /// </summary>
        /// <param name="input">stream to be converted</param>
        /// <returns>stream converted to byte array</returns>
        public static byte[] StreamToByteArray(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            MemoryStream ms = new MemoryStream();

            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            byte[] arr = ms.ToArray();
            ms.Close();
            return arr;
        }
    }
}

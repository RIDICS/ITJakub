using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace IT_Jakub.Classes.Utils {
    /// <summary>
    /// This class generates data for some GUI elements and also can count a hash for password.
    /// </summary>
    static class Generator {

        /// <summary>
        /// Gets the days in month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public static int[] getDaysInMonth(int month, int year) {
            int days = DateTime.DaysInMonth(year, month);
            int[] toReturn = new int[days];

            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = i + 1;
            }
            return toReturn;
        }

        /// <summary>
        /// Gets the months.
        /// </summary>
        /// <returns></returns>
        public static int[] getMonths() {
            int[] toReturn = new int[12];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = i + 1;
            }
            return toReturn;
        }

        /// <summary>
        /// Gets the past count years.
        /// </summary>
        /// <param name="years">The count of past years.</param>
        /// <returns></returns>
        public static int[] getPastYears(int years) {
            int yearNow = DateTime.Now.Year;
            int[] toReturn = new int[years];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = yearNow - i;
            }
            return toReturn;

        }

        /// <summary>
        /// Gets the next years.
        /// </summary>
        /// <param name="years">The count of next years.</param>
        /// <returns></returns>
        public static int[] getNextYears(int years) {
            int yearNow = DateTime.Now.Year;
            int[] toReturn = new int[years];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = yearNow + i;
            }
            return toReturn;

        }

        /// <summary>
        /// Generates the hash for password.
        /// </summary>
        /// <param name="s">The unhashed password string</param>
        /// <returns></returns>
        public static string generateHashForPassword(string s) {
            HashAlgorithmProvider sha256 = HashAlgorithmProvider.OpenAlgorithm("SHA256");

            IBuffer dataBuffer = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            IBuffer hashBuffer = sha256.HashData(dataBuffer);

            string toReturn = CryptographicBuffer.EncodeToHexString(hashBuffer);
            return toReturn;
        }


    }
}

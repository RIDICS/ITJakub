using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace IT_Jakub.Classes.Utils {
    static class Generator {

        public static int[] getDaysInMonth(int month, int year) {
            int days = DateTime.DaysInMonth(year, month);
            int[] toReturn = new int[days];

            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = i + 1;
            }
            return toReturn;
        }

        public static int[] getMonths() {
            int[] toReturn = new int[12];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = i + 1;
            }
            return toReturn;
        }

        public static int[] getPastYears(int years) {
            int yearNow = DateTime.Now.Year;
            int[] toReturn = new int[years];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = yearNow - i;
            }
            return toReturn;

        }

        public static int[] getNextYears(int years) {
            int yearNow = DateTime.Now.Year;
            int[] toReturn = new int[years];
            for (int i = 0; i < toReturn.Length; i++) {
                toReturn[i] = yearNow + i;
            }
            return toReturn;

        }

        public static string generateHashForPassword(string s) {
            HashAlgorithmProvider sha256 = HashAlgorithmProvider.OpenAlgorithm("SHA256");

            IBuffer dataBuffer = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            IBuffer hashBuffer = sha256.HashData(dataBuffer);

            string toReturn = CryptographicBuffer.EncodeToHexString(hashBuffer);
            return toReturn;
        }


    }
}

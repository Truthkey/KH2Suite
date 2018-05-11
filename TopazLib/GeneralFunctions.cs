using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace TopazLib
{
    public class GeneralFunctions
    {
        //A cleaner, simpler way or reading bytes.
        public static byte[] readBytes(string fileName, int toSkip, int toTake)
        {
            return File.ReadAllBytes(fileName).Skip(toSkip).Take(toTake).ToArray();
        }

        // To get a specific int in a specific byte space
        public static int getIntValue(byte[] byteToGet, int valueBits)
        {
            int Ex = 0;

            if (valueBits == 16)
            {
                Ex = BitConverter.ToInt16(byteToGet, 0);
            }

            if (valueBits == 32)
            {
                Ex = BitConverter.ToInt32(byteToGet, 0);
            }

            return Ex;
        }

        // Reads bytes, reverses them, converts them to int.
        public static int readBytetoInt32(string fileName, int toSkip, int toTake)
        {
            int toExport = 0;
            toExport = BitConverter.ToInt32(File.ReadAllBytes(fileName).Skip(toSkip).Take(toTake).ToArray(), 0);
            return toExport;
        }


        public static short readBytetoInt16(string fileName, int toSkip, int toTake)
        {
            short toExport = 0;
            toExport = BitConverter.ToInt16(File.ReadAllBytes(fileName).Skip(toSkip).Take(toTake).ToArray(), 0);
            return toExport;
        }

        // Adds comboBox entries.
        public static void entryLoop(ComboBox toChange, int condition)
        {
            int conditionInt = 0;

            if (toChange.Items.Count == 0)
            {
                while (conditionInt < condition)
                {
                    toChange.Items.Add(conditionInt);
                    conditionInt++;
                }
            }
        }

        public static void entryLoop(ComboBox toChange, int condition, string FileName, int toSkip, int toProcess, int toTake)
        {
            int conditionInt = 0;
            byte[] toParse = new byte[] { 0 };

            if (toChange.Items.Count == 0)
            {
                while (conditionInt < condition)
                {
                    toParse = File.ReadAllBytes(FileName).Skip(toSkip + toProcess * conditionInt).Take(toTake).Reverse().ToArray();
                    toChange.Items.Add(BitConverter.ToString(toParse).Replace("-", ""));
                    conditionInt++;
                }
            }
        }


        // Converts byte to string, cleanly.
        public static string getByteString(byte toConvert)
        {
            return toConvert.ToString("X2");
        }

        // Converts an Int16 to a HEX String
        public static string getInt16String(short toConvert)
        {
            return toConvert.ToString("X4").Replace("-", "");
        }

        // Converts hexadecimal strings to byte array
        public static byte[] hexToByteArray(string hexCode)
        {
            byte[] toExport =  Enumerable.Range(0, hexCode.Length)
                              .Where(x => x % 2 == 0)
                              .Select(x => Convert.ToByte(hexCode.Substring(x, 2), 16))
                              .ToArray();
            Array.Reverse(toExport);
            return toExport;
        }

        // Convert a string to byte to int
        public static short hexToInteger(string hexCode)
        {
            return BitConverter.ToInt16(hexToByteArray(hexCode), 0);
        }

        public static byte[] intToByteArray(short toConvert)
        {
            byte[] toExport = BitConverter.GetBytes(toConvert);
            return toExport;
        }

        public static byte[] intToByteArray(int toConvert)
        {
            byte[] toExport = BitConverter.GetBytes(toConvert);
            return toExport;
        }

        public static byte readByte(string Filename, int toSkip)
        {
            int toConvert = 0;
            byte[] toExport = null;
            
            using (FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(toSkip, SeekOrigin.Begin);
                toConvert = fs.ReadByte();
            }

            toExport = BitConverter.GetBytes(toConvert);
            return toExport[0];
        }

        public static byte hexToByte(string hexCode)
        {
            byte[] toExport = new byte[] { byte.Parse(hexCode, NumberStyles.HexNumber) };
            return toExport[0];
        }
    }
}

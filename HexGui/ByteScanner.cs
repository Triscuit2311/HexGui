using System;
using System.Collections.Generic;
using System.Linq;

namespace HexGui
{
    public static class ByteScanner
    {
        public static int Scan(IList<byte> arr, ScanQuery query)
        {
            for (var i = 0; i < arr.Count; i++)
                if (!query.Pattern.Where((t, j) => query.Mask[j] != '?' && t != arr[i + j]).Any())
                    return i;
            return -1;
        }

/*
        public static bool Patch(IList<byte> arr, int loc, IList<byte> patch)
        {
            if (loc >= arr.Count + patch.Count)
            {
                Console.WriteLine("Cannot patch: Location overrun.");
                return false;
            }

            foreach (var b in patch)
            {
                arr[loc] = b;
                loc++;
            }

            return true;
        }
*/

        private static string ParseSafeAob(string combo)
        {
            return combo
                .Replace("??", "?")
                .Replace("**", "?")
                .Replace("*", "?")
                .Replace("XX", "?")
                .Replace("xx", "?")
                .Replace("0x", " ")
                .Replace("0X", " ")
                .Replace("%", " ")
                .Replace(",", " ")
                .Replace(";", " ")
                .Replace("\n", " ")
                .Replace("\\n", " ")
                .Replace("\\r", " ")
                .Replace("\t", " ")
                .Replace("\\t", " ")
                .Replace("\\x", " ")
                .Replace("          ", " ")
                .Replace("         ", " ")
                .Replace("        ", " ")
                .Replace("       ", " ")
                .Replace("      ", " ")
                .Replace("     ", " ")
                .Replace("    ", " ")
                .Replace("   ", " ")
                .Replace("  ", " ")
                .Trim();
        }

        public struct ScanQuery
        {
            public byte[] Pattern { get; }
            public char[] Mask { get; }
            public readonly string InputStr;

/*
            public ScanQuery(byte[] pattern, char[] mask)
            {
                Pattern = pattern;
                Mask = mask;
                InputStr = "";
            }
*/
            public ScanQuery(string combo)
            {
                var fixedCombo = ParseSafeAob(combo);
                var strArr = fixedCombo.Split(' ');
                InputStr = fixedCombo;

                var pattern = new byte[strArr.Length];
                var mask = new char[strArr.Length];

                for (var i = 0; i < strArr.Length; i++)
                {
                    var s = strArr[i];
                    if (s == "?")
                    {
                        pattern[i] = byte.MinValue;
                        mask[i] = '?';
                        continue;
                    }

                    pattern[i] = (byte) Convert.ToInt32(strArr[i], 16);
                    mask[i] = 'x';
                }

                Pattern = pattern;
                Mask = mask;
            }
        }
    }
}
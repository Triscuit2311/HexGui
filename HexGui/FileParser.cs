using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HexGui
{
    public static class FileParser
    {
        private static string _path;

        static FileParser()
        {
            Table = new DataTable();
            Table.Columns.Add("Offset");
            Table.Columns.Add("Hex");
            Table.Columns.Add("Ascii");
            Table.Rows.Add("", "No File Loaded", "");
        }

        public static int BytesPerLine { get; private set; } = 20;
        public static List<byte> FileBytes { get; private set; } = new List<byte>();

        public static DataTable Table { get; }

        public static void Refresh(int bytesPerLine)
        {
            BytesPerLine = bytesPerLine;
            ReloadFile();
        }

        public static void ReadNewFile(string path)
        {
            _path = path;
            ReloadFile();
        }

        public static void ReloadFile()
        {
            FileBytes.Clear();
            FileBytes = File.ReadAllBytes(_path).ToList();
            Table.Clear();
            ParseFile();
        }


        private static void ParseFile()
        {
            var lines = FileBytes.Count / BytesPerLine + 1;

            var hexStringBuilder = new StringBuilder();
            var asciiStringBuilder = new StringBuilder();

            for (var i = 0; i < lines; i++)
            {
                hexStringBuilder.Clear();
                asciiStringBuilder.Clear();

                for (var j = 0; j < BytesPerLine; j++)
                {
                    if (j + i * BytesPerLine >= FileBytes.Count) break;

                    // Get the current byte
                    var thisByte = FileBytes[j + i * BytesPerLine];

                    // Append byte to hex side
                    hexStringBuilder.Append($"{thisByte:X2}");

                    // Append char to char side (ignore breaks)
                    asciiStringBuilder.Append(thisByte < 20 || thisByte == 0x5C || thisByte == 0x7F
                        ? '.'
                        : (char) thisByte);

                    if (j >= BytesPerLine - 1) continue;
                    hexStringBuilder.Append(' ');
                    asciiStringBuilder.Append(' ');
                }

                Table.Rows.Add(
                    (i * BytesPerLine).ToString("X8"),
                    hexStringBuilder.ToString(),
                    asciiStringBuilder.ToString());
            }
        }
    }
}
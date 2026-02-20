using System;
using System.Collections.Generic;

namespace ComputerHardwareInfo.Utilities
{
    public class ConsoleTableFormatter
    {
        private readonly List<string> headers;
        private readonly List<List<string>> rows;
        private readonly int[] columnWidths;

        public ConsoleTableFormatter(params string[] headers)
        {
            this.headers = new List<string>(headers);
            this.rows = new List<List<string>>();
            this.columnWidths = new int[headers.Length];

            for (int i = 0; i < headers.Length; i++)
                columnWidths[i] = headers[i].Length + 2;
        }

        public void AddRow(params string[] values)
        {
            if (values.Length != headers.Count)
                throw new ArgumentException($"Row must have {headers.Count} columns");

            rows.Add(new List<string>(values));

            for (int i = 0; i < values.Length; i++)
            {
                int len = (values[i] ?? string.Empty).Length + 2;
                if (len > columnWidths[i])
                    columnWidths[i] = len;
            }
        }

        public void RenderToConsole()
        {
            Console.WriteLine();
            RenderHorizontalLine();
            RenderHeaderRow();
            RenderHorizontalLine();
            foreach (var row in rows)
                RenderDataRow(row);
            RenderHorizontalLine();
            Console.WriteLine();
        }

        private void RenderHorizontalLine()
        {
            string line = "+";
            for (int i = 0; i < columnWidths.Length; i++)
                line += new string('-', columnWidths[i]) + "+";
            Console.WriteLine(line);
        }

        private void RenderHeaderRow()
        {
            string row = "|";
            for (int i = 0; i < headers.Count; i++)
                row += PadCell(headers[i], columnWidths[i]) + "|";
            Console.WriteLine(row);
        }

        private void RenderDataRow(List<string> values)
        {
            string row = "|";
            for (int i = 0; i < values.Count; i++)
                row += PadCell(values[i] ?? string.Empty, columnWidths[i]) + "|";
            Console.WriteLine(row);
        }

        private string PadCell(string value, int width) => value.PadRight(width - 1, ' ') + " ";
    }
}

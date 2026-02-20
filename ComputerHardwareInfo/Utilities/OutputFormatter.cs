using System;
using System.IO;
using System.Text;

namespace ComputerHardwareInfo.Utilities
{
    public class HardwareReportWriter
    {
        private StringBuilder reportContent;
        private readonly string outputFilePath;
        private readonly OutputType outputDestination;

        public HardwareReportWriter(string targetComputerName, OutputType outputDestination)
        {
            this.outputDestination = outputDestination;
            this.reportContent = new StringBuilder();
            string outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.outputFilePath = Path.Combine(
                outputDirectory,
                $"pc_hardware_info_{targetComputerName}.txt");
        }

        public string GetOutputFilePath() => outputFilePath;

        public void AppendSectionHeader(string sectionTitle)
        {
            reportContent.Clear();
            reportContent.Append($"{Environment.NewLine}==============={Environment.NewLine}");
            reportContent.Append($"{sectionTitle}{Environment.NewLine}");
            reportContent.Append($"==============={Environment.NewLine}");
        }

        public void AppendContentLine(string content)
        {
            reportContent.Append($"{content}{Environment.NewLine}");
        }

        public void FlushReportToOutput()
        {
            Console.WriteLine(reportContent.ToString());
            if (outputDestination == OutputType.File)
            {
                WriteReportToFile(reportContent.ToString());
                Console.WriteLine($"[Report saved to: {outputFilePath}]");
            }
        }

        private void WriteReportToFile(string reportText)
        {
            using (var writer = new StreamWriter(outputFilePath, true))
                writer.WriteLine(reportText);
        }
    }
}

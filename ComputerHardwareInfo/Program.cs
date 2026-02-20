using ComputerHardwareInfo.Utilities;
using System;

namespace ComputerHardwareInfo
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var argumentParser = new CommandLineArgumentParser(args);
                var targetComputerNames = argumentParser.ParseTargetComputerNames();

                if (targetComputerNames.Count == 0)
                {
                    argumentParser.PromptToContinueOrExit();
                    return 1;
                }

                argumentParser.DisplayProcessingSummary(targetComputerNames);

                int successfulCollections = 0;
                var resultsTable = new ConsoleTableFormatter("Computer", "Status", "Details");

                for (int i = 0; i < targetComputerNames.Count; i++)
                {
                    string computerName = targetComputerNames[i];
                    argumentParser.DisplayProcessingStarted(computerName, i + 1, targetComputerNames.Count);

                    try
                    {
                        var hardwareInfoCollector = new ComputerInfo(computerName, OutputType.File);
                        hardwareInfoCollector.Execute();
                        resultsTable.AddRow(computerName, "√ Success", "Data saved to Desktop");
                        successfulCollections++;
                        argumentParser.DisplayProcessingCompleted(computerName, true, "Data saved to Desktop");
                    }
                    catch (Exception ex)
                    {
                        resultsTable.AddRow(computerName, "X Failed", ex.Message);
                        argumentParser.DisplayProcessingCompleted(computerName, false, $"Error: {ex.Message}");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== PROCESSING SUMMARY ===");
                Console.ResetColor();
                resultsTable.RenderToConsole();

                argumentParser.DisplayCompletionSummary(successfulCollections, targetComputerNames.Count - successfulCollections, targetComputerNames);
                argumentParser.PromptToContinueOrExit();

                return successfulCollections > 0 ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fatal Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
                Console.ReadKey();
                return 2;
            }
        }
    }
}



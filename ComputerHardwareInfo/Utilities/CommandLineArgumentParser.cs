using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerHardwareInfo.Utilities
{
    public class CommandLineArgumentParser
    {
        private readonly string[] _commandLineArguments;

        public CommandLineArgumentParser(string[] commandLineArguments) => _commandLineArguments = commandLineArguments ?? new string[0];

        public List<string> ParseTargetComputerNames()
        {
            DisplayWelcomeBanner();
            var computerNames = ExtractComputerNamesFromArguments();

            if (computerNames.Count == 0)
                computerNames = PromptUserForComputerNames();

            if (computerNames.Count == 0)
            {
                DisplayErrorMessage("No computer names provided. Application will exit.");
                return new List<string>();
            }

            return ValidateAndCleanComputerNames(computerNames);
        }

        private void DisplayWelcomeBanner()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("+=================================================+");
            Console.WriteLine("|   Computer Hardware Information Collector       |");
            Console.WriteLine("|   Gathers detailed system information via WMI   |");
            Console.WriteLine("+=================================================+");
            Console.WriteLine();
            Console.ResetColor();
        }

        private List<string> ExtractComputerNamesFromArguments()
        {
            var computerNames = new List<string>();

            foreach (var argument in _commandLineArguments)
            {
                if (argument.Equals("/?", StringComparison.OrdinalIgnoreCase) ||
                    argument.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
                    argument.Equals("-h", StringComparison.OrdinalIgnoreCase))
                {
                    DisplayHelpInformation();
                    continue;
                }

                if (argument.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!string.IsNullOrWhiteSpace(argument))
                {
                    if (argument.Contains(","))
                    {
                        foreach (var name in argument.Split(','))
                        {
                            var trimmed = name.Trim();
                            if (!string.IsNullOrWhiteSpace(trimmed))
                                computerNames.Add(trimmed);
                        }
                    }
                    else
                        computerNames.Add(argument.Trim());
                }
            }

            return computerNames;
        }

        private List<string> PromptUserForComputerNames()
        {
            var computerNames = new List<string>();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No computer names provided via command-line arguments.");
            Console.WriteLine();
            Console.ResetColor();

            while (true)
            {
                Console.Write("Enter a computer name (or 'done'/'exit' to start/exit): ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (input.Equals("done", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayWarningMessage("Computer name cannot be empty. Please try again.");
                    continue;
                }

                if (input.Length > 255)
                {
                    DisplayWarningMessage("Computer name is too long (max 255 characters). Please try again.");
                    continue;
                }

                if (!IsValidComputerName(input))
                {
                    DisplayWarningMessage("Invalid computer name format. Use alphanumeric characters, hyphens (-), underscores (_), or dots (.)");
                    Console.WriteLine("  Example: 'DESKTOP-PC', 'server01', 'ys0044n6', 'server.example.com'");
                    continue;
                }

                if (computerNames.Contains(input, StringComparer.OrdinalIgnoreCase))
                {
                    DisplayWarningMessage("This computer name has already been added.");
                    continue;
                }

                computerNames.Add(input);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"| Added: {input}");
                Console.ResetColor();
            }

            return computerNames;
        }

        private bool IsValidComputerName(string computerName)
        {
            if (string.IsNullOrWhiteSpace(computerName) || computerName.Length > 255)
                return false;

            foreach (char c in computerName)
            {
                bool isAlphaNum = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
                bool isSpecial = c == '-' || c == '.' || c == '_';
                if (!isAlphaNum && !isSpecial)
                    return false;
            }

            return !computerName.EndsWith("-");
        }

        private List<string> ValidateAndCleanComputerNames(List<string> computerNames)
        {
            var validated = new List<string>();

            foreach (var name in computerNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (IsValidComputerName(name))
                    validated.Add(name);
                else
                    DisplayWarningMessage($"Skipping invalid computer name: '{name}'");
            }

            return validated;
        }

        private void DisplayHelpInformation()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("  ComputerHardwareInfo.exe [computer1] [computer2] ...");
            Console.WriteLine("  ComputerHardwareInfo.exe \"computer1,computer2,computer3\"");
            Console.WriteLine();
            Console.WriteLine("ARGUMENTS:");
            Console.WriteLine("  computer1, computer2, ...   Computer names to collect information from");
            Console.WriteLine("                              Use SPACE-separated for multiple individual args");
            Console.WriteLine("                              Use COMMA-separated within quotes for grouped names");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("  /?, --help, -h              Display this help message");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("  ComputerHardwareInfo.exe DESKTOP-01");
            Console.WriteLine("  ComputerHardwareInfo.exe ys0044n6 ys0044m5");
            Console.WriteLine("  ComputerHardwareInfo.exe server01 server02 server03");
            Console.WriteLine("  ComputerHardwareInfo.exe \"server01,server02,server03\"");
            Console.WriteLine("  ComputerHardwareInfo.exe DESKTOP-01 \"LAPTOP-02,LAPTOP-03\"");
            Console.WriteLine("  ComputerHardwareInfo.exe \"192.168.1.10,192.168.1.11\"");
            Console.WriteLine();
            Console.WriteLine("OUTPUT:");
            Console.WriteLine("  Hardware information is saved to:");
            Console.WriteLine("  %USERPROFILE%\\Desktop\\pc_hardware_info_[COMPUTER_NAME].txt");
            Console.WriteLine();
            Console.WriteLine("INTERACTIVE MODE:");
            Console.WriteLine("  If no computer names are provided, you will be prompted to enter them.");
            Console.WriteLine("  Enter each computer name and press Enter.");
            Console.WriteLine("  Type 'done' to start processing the computers you entered.");
            Console.WriteLine("  Type 'exit' to exit the application without processing.");
            Console.WriteLine();
            Console.ResetColor();
        }

        private void DisplayWarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"| WARNING: {message}");
            Console.ResetColor();
        }

        private void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"| ERROR: {message}");
            Console.ResetColor();
        }

        public void DisplayProcessingSummary(List<string> computerNames)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("===============================================================");
            Console.WriteLine($"Processing {computerNames.Count} computer(s):");
            Console.WriteLine("===============================================================");
            Console.ResetColor();

            for (int i = 0; i < computerNames.Count; i++)
                Console.WriteLine($"  {i + 1}. {computerNames[i]}");

            Console.WriteLine();
        }

        public void DisplayProcessingStarted(string computerName, int currentIndex, int totalComputers)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{currentIndex}/{totalComputers}] Processing: {computerName}");
            Console.ResetColor();
        }

        public void DisplayProcessingCompleted(string computerName, bool success, string message = "")
        {
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"| Completed: {computerName}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"| Failed: {computerName}");
            }

            if (!string.IsNullOrEmpty(message))
                Console.WriteLine($"  {message}");

            Console.ResetColor();
        }

        public void DisplayCompletionSummary(int successCount, int failureCount, List<string> computerNames)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("===============================================================");
            Console.WriteLine("Collection Complete!");
            Console.WriteLine("===============================================================");
            Console.ResetColor();

            Console.WriteLine($"Successfully processed: {successCount} computer(s)");

            if (failureCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Failed to process: {failureCount} computer(s)");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Report files have been saved to your Desktop:");
            foreach (var computerName in computerNames)
                Console.WriteLine($"  | pc_hardware_info_{computerName}.txt");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void PromptToContinueOrExit()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}

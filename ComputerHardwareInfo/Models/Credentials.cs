using System;
using System.Management;

namespace ComputerHardwareInfo.Models
{
    public class RemoteConnectionCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        public static RemoteConnectionCredentials PromptUserForRemoteCredentials()
        {
            Console.WriteLine("Access denied. Please provide credentials to connect to the remote computer.");
            Console.Write("Domain\\Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadMaskedPassword();

            var parts = username.Split(new[] { '\\' }, StringSplitOptions.None);
            string domain = parts.Length == 2 ? parts[0] : ".";
            string user = parts.Length == 2 ? parts[1] : username;

            return new RemoteConnectionCredentials { Domain = domain, Username = user, Password = password };
        }

        public ConnectionOptions BuildWmiConnectionOptions()
        {
            var options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                EnablePrivileges = true,
                Authentication = AuthenticationLevel.Default
            };

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                options.Authentication = AuthenticationLevel.PacketPrivacy;

            return options;
        }

        private static string ReadMaskedPassword()
        {
            string password = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                        password = password.Substring(0, password.Length - 1);
                }
                else
                    password += key.KeyChar;
            }
            Console.WriteLine();
            return password;
        }
    }
}

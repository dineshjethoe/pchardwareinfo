using System;
using System.IO;
using System.Management;
using System.Text;

namespace ComputerHardwareInfo
{
    public class ComputerInfo
    {
        readonly string fileName;
        readonly string _computerName;
        readonly OutputType _outType;
        readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        StringBuilder stringBuilder;

        public ComputerInfo(string computerName, OutputType outType)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            _computerName = computerName;
            _outType = outType;
            stringBuilder = new StringBuilder();
            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"pc_hardware_info_{_computerName}.txt");
        }

        public void Execute()
        {
            var connectionOption = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                EnablePrivileges = true
            };

            var mgtScope = new ManagementScope(@"\\" + _computerName + @"\ROOT\CIMV2", connectionOption);

            Formatter("OS", true);
            var objectQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Computer Name: {m["csname"].ToString()}");
                    Formatter($"Caption: {m["Caption"]}");
                    Formatter($"Name: { m["name"].ToString()}");
                    Formatter($"WindowsDirectory: {m["WindowsDirectory"]}");
                    Formatter($"ProductType: {m["ProductType"]}");
                    Formatter($"SerialNumber: {m["SerialNumber"]}");
                    Formatter($"SystemDirectory: {m["SystemDirectory"]}");
                    Formatter($"CountryCode: {m["CountryCode"]}");
                    Formatter($"CurrentTimeZone: {m["CurrentTimeZone"]}");
                    Formatter($"EncryptionLevel: {m["EncryptionLevel"]}");
                    Formatter($"OSType: {m["OSType"]}");
                    Formatter($"Manufacture: {m["Manufacturer"].ToString()}");
                    Formatter($"Version: {m["Version"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Installed applications", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_Product");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject mo in objectSearcher.Get())
                {
                    Formatter($"{mo["Name"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("CPU", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_Processor");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Name: {m["Name"]}");
                    Formatter($"Device ID: {m["DeviceID"]}");
                    Formatter($"Processor ID: {m["ProcessorID"]}");
                    Formatter($"Manufacturer: {m["Manufacturer"]}");
                    Formatter($"Current Clock Speed: {m["CurrentClockSpeed"]}");
                    Formatter($"Max Clock Speed: {m["MaxClockSpeed"]}");
                    Formatter($"Caption: {m["Caption"]}");
                    Formatter($"Number of Cores: {m["NumberOfCores"]}");
                    //Formatter($"NumberOfEnabledCore: {m["NumberOfEnabledCore"]);
                    Formatter($"Number of Logical Processors: {m["NumberOfLogicalProcessors"]}");
                    Formatter($"Architecture: {m["Architecture"]}");
                    Formatter($"Family: {m["Family"]}");
                    Formatter($"Processor Type: {m["ProcessorType"]}");
                    //Formatter($"Characteristics: {m["Characteristics"]);
                    Formatter($"Address Width: {m["AddressWidth"]}");
                    Formatter($"Revision: {m["Revision"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("RAM", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                UInt64 total = 0;
                foreach (ManagementObject ram in objectSearcher.Get())
                {
                    total += (UInt64)ram.GetPropertyValue("Capacity");
                }

                Formatter($"RAM: {total / 1073741824} GB");
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("HDD", true);
            //ManagementClass partitionsClass = new ManagementClass("Win32_LogicalDisk");
            //ManagementObjectCollection partitions = partitionsClass.GetInstances();
            //partitionsClass.Scope = mgtScope;
            //Formatter("Partitions:");
            //foreach (ManagementObject partion in partitions)
            //{
            //    Formatter(Convert.ToString(partion["VolumeSerialNumber"]));
            //    break;
            //}
            //Console.WriteLine(stringBuilder.ToString());

            objectQuery = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Disk Name: {m["Name"]}");
                    Formatter($"Disk Size: { SizeSuffix(Convert.ToInt64(m["Size"]))}");
                    Formatter($"FreeSpace: { SizeSuffix(Convert.ToInt64(m["FreeSpace"]))}");
                    Formatter($"Disk DeviceID: {m["DeviceID"]}");
                    Formatter($"Disk VolumeName: {m["VolumeName"]}");
                    Formatter($"Disk SystemName: {m["SystemName"]}");
                    Formatter($"Disk VolumeSerialNumber: {m["VolumeSerialNumber"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Pagefile", true);
            objectQuery = new ObjectQuery("SELECT AllocatedBaseSize FROM Win32_PageFileUsage");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementBaseObject obj in objectSearcher.Get())
                {
                    uint used = (uint)obj.GetPropertyValue("AllocatedBaseSize");
                    Formatter($"AllocatedBaseSize: {SizeSuffix(Convert.ToInt64(used))}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Pagefile setting", true);
            objectQuery = new ObjectQuery("SELECT MaximumSize FROM Win32_PageFileSetting");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementBaseObject obj in objectSearcher.Get())
                {
                    uint max = (uint)obj.GetPropertyValue("MaximumSize");
                    Formatter($"MaximumSize: {SizeSuffix(Convert.ToInt64(max))}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Network", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                ManagementObjectCollection objectCollection = objectSearcher.Get();

                Formatter($"Network adapaters: {objectCollection.Count}");

                foreach (ManagementObject networkAdapter in objectCollection)
                {
                    PropertyDataCollection networkAdapterProperties = networkAdapter.Properties;
                    foreach (PropertyData networkAdapterProperty in networkAdapterProperties)
                    {
                        if (networkAdapterProperty.Value != null)
                        {
                            Formatter($"Network adapter property name: {networkAdapterProperty.Name}");
                            Formatter($"Network adapter property value: {networkAdapterProperty.Value}");
                        }
                    }
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Video", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_VideoController");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Name: {m["Name"]}");
                    Formatter($"Status: {m["Status"]}");
                    Formatter($"Caption: {m["Caption"]}");
                    Formatter($"DeviceID: {m["DeviceID"]}");
                    Formatter($"AdapterRAM: {SizeSuffix(Convert.ToInt64(m["AdapterRAM"]))}");
                    Formatter($"AdapterDACType: {m["AdapterDACType"]}");
                    Formatter($"Monochrome: {m["Monochrome"]}");
                    Formatter($"InstalledDisplayDrivers: {m["InstalledDisplayDrivers"]}");
                    Formatter($"DriverVersion: {m["DriverVersion"]}");
                    Formatter($"VideoProcessor: {m["VideoProcessor"]}");
                    Formatter($"VideoArchitecture: {m["VideoArchitecture"]}");
                    Formatter($"VideoMemoryType: {m["VideoMemoryType"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Sound", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_SoundDevice");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Name: {m["Name"]}");
                    Formatter($"ProductName: {m["ProductName"]}");
                    Formatter($"Availability: {m["Availability"]}");
                    Formatter($"DeviceID: {m["DeviceID"]}");
                    Formatter($"PowerManagementSupported: {m["PowerManagementSupported"]}");
                    Formatter($"Status: {m["Status"]}");
                    Formatter($"StatusInfo: {m["StatusInfo"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }

            Formatter("Printer(s)", true);
            objectQuery = new ObjectQuery("SELECT * FROM Win32_Printer");
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgtScope, objectQuery))
            {
                foreach (ManagementObject m in objectSearcher.Get())
                {
                    Formatter($"Name: {m["Name"]}");
                    Formatter($"Network: {m["Network"]}");
                    Formatter($"Availability: {m["Availability"]}");
                    Formatter($"Is default printer: {m["Default"]}");
                    Formatter($"DeviceID: {m["DeviceID"]}");
                    Formatter($"Status: {m["Status"]}");
                }
            }
            Console.WriteLine(stringBuilder.ToString());
            if (_outType == OutputType.File)
            {
                FileWriter(stringBuilder.ToString());
            }
        }

        #region helpers
        private string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        private void FileWriter(string text)
        {
            using (StreamWriter file = new StreamWriter(fileName, true))
            {
                file.WriteLine(text);
            }
        }

        private void Formatter(string text, bool isSectionHeader = false)
        {
            if (isSectionHeader)
            {
                stringBuilder.Clear();
                stringBuilder.Append($"{Environment.NewLine}==============={Environment.NewLine}");
            }
            stringBuilder.Append($"{text}{Environment.NewLine}");
            if (isSectionHeader)
            {
                stringBuilder.Append($"==============={Environment.NewLine}");
            }
        }
        #endregion
    }
}

using ComputerHardwareInfo.Models;
using ComputerHardwareInfo.Utilities;
using System;
using System.Management;

namespace ComputerHardwareInfo.Collectors
{
    public class RemoteComputerHardwareCollector
    {
        private readonly string targetComputerName;
        private readonly HardwareReportWriter reportWriter;
        private readonly BytesSizeConverter bytesSizeConverter;
        private ManagementScope wmiConnectionScope;

        public RemoteComputerHardwareCollector(string targetComputerName, OutputType outputType)
        {
            this.targetComputerName = targetComputerName;
            this.reportWriter = new HardwareReportWriter(targetComputerName, outputType);
            this.bytesSizeConverter = new BytesSizeConverter();
        }

        public void CollectAllHardwareInformation()
        {
            if (!EstablishRemoteConnection())
                return;

            CollectOperatingSystemInformation();
            CollectInstalledApplicationsList();
            CollectProcessorInformation();
            CollectMemoryInformation();
            CollectStorageDevicesInformation();
            CollectPageFileInformation();
            CollectPageFileSettingInformation();
            CollectNetworkAdaptersInformation();
            CollectVideoControllerInformation();
            CollectAudioDevicesInformation();
            CollectPrintersInformation();
        }

        private bool EstablishRemoteConnection()
        {
            try
            {
                var options = new ConnectionOptions
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                };

                wmiConnectionScope = new ManagementScope(@"\\" + targetComputerName + @"\ROOT\CIMV2", options);
                wmiConnectionScope.Connect();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                var credentials = RemoteConnectionCredentials.PromptUserForRemoteCredentials();
                try
                {
                    var options = credentials.BuildWmiConnectionOptions();
                    wmiConnectionScope = new ManagementScope(@"\\" + targetComputerName + @"\ROOT\CIMV2", options);
                    wmiConnectionScope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
                    wmiConnectionScope.Options.Impersonation = ImpersonationLevel.Impersonate;
                    wmiConnectionScope.Connect();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect with provided credentials: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        private void CollectOperatingSystemInformation()
        {
            reportWriter.AppendSectionHeader("Operating System");
            wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_OperatingSystem", osInfo =>
            {
                reportWriter.AppendContentLine($"Computer Name: {osInfo.GetPropertyValueAsString("csname")}");
                reportWriter.AppendContentLine($"Caption: {osInfo.GetPropertyValueAsString("Caption")}");
                reportWriter.AppendContentLine($"Name: {osInfo.GetPropertyValueAsString("name")}");
                reportWriter.AppendContentLine($"Windows Directory: {osInfo.GetPropertyValueAsString("WindowsDirectory")}");
                reportWriter.AppendContentLine($"Product Type: {osInfo.GetPropertyValueAsString("ProductType")}");
                reportWriter.AppendContentLine($"Serial Number: {osInfo.GetPropertyValueAsString("SerialNumber")}");
                reportWriter.AppendContentLine($"System Directory: {osInfo.GetPropertyValueAsString("SystemDirectory")}");
                reportWriter.AppendContentLine($"Country Code: {osInfo.GetPropertyValueAsString("CountryCode")}");
                reportWriter.AppendContentLine($"Current Time Zone: {osInfo.GetPropertyValueAsString("CurrentTimeZone")}");
                reportWriter.AppendContentLine($"Encryption Level: {osInfo.GetPropertyValueAsString("EncryptionLevel")}");
                reportWriter.AppendContentLine($"OS Type: {osInfo.GetPropertyValueAsString("OSType")}");
                reportWriter.AppendContentLine($"Manufacturer: {osInfo.GetPropertyValueAsString("Manufacturer")}");
                reportWriter.AppendContentLine($"Version: {osInfo.GetPropertyValueAsString("Version")}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectInstalledApplicationsList()
        {
            reportWriter.AppendSectionHeader("Installed Applications");
            try
            {
                wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_Product",
                    app => reportWriter.AppendContentLine(app.GetPropertyValueAsString("Name")));
            }
            catch (Exception ex)
            {
                reportWriter.AppendContentLine($"Note: Unable to retrieve installed applications: {ex.Message}");
            }
            reportWriter.FlushReportToOutput();
        }

        private void CollectProcessorInformation()
        {
            reportWriter.AppendSectionHeader("Processor (CPU)");
            wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_Processor", cpu =>
            {
                reportWriter.AppendContentLine($"Name: {cpu.GetPropertyValueAsString("Name")}");
                reportWriter.AppendContentLine($"Device ID: {cpu.GetPropertyValueAsString("DeviceID")}");
                reportWriter.AppendContentLine($"Processor ID: {cpu.GetPropertyValueAsString("ProcessorID")}");
                reportWriter.AppendContentLine($"Manufacturer: {cpu.GetPropertyValueAsString("Manufacturer")}");
                reportWriter.AppendContentLine($"Current Clock Speed (MHz): {cpu.GetPropertyValueAsString("CurrentClockSpeed")}");
                reportWriter.AppendContentLine($"Max Clock Speed (MHz): {cpu.GetPropertyValueAsString("MaxClockSpeed")}");
                reportWriter.AppendContentLine($"Caption: {cpu.GetPropertyValueAsString("Caption")}");
                reportWriter.AppendContentLine($"Number of Cores: {cpu.GetPropertyValueAsString("NumberOfCores")}");
                reportWriter.AppendContentLine($"Number of Logical Processors: {cpu.GetPropertyValueAsString("NumberOfLogicalProcessors")}");
                reportWriter.AppendContentLine($"Architecture: {cpu.GetPropertyValueAsString("Architecture")}");
                reportWriter.AppendContentLine($"Family: {cpu.GetPropertyValueAsString("Family")}");
                reportWriter.AppendContentLine($"Processor Type: {cpu.GetPropertyValueAsString("ProcessorType")}");
                reportWriter.AppendContentLine($"Address Width: {cpu.GetPropertyValueAsString("AddressWidth")}");
                reportWriter.AppendContentLine($"Revision: {cpu.GetPropertyValueAsString("Revision")}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectMemoryInformation()
        {
            reportWriter.AppendSectionHeader("Memory (RAM)");
            ulong totalMemory = 0;
            wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_PhysicalMemory",
                mem => totalMemory += mem.GetPropertyValue("Capacity", 0UL));
            reportWriter.AppendContentLine($"Total RAM: {totalMemory / 1073741824} GB");
            reportWriter.FlushReportToOutput();
        }

        private void CollectStorageDevicesInformation()
        {
            reportWriter.AppendSectionHeader("Storage Devices (HDD/SSD)");
            wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_LogicalDisk", disk =>
            {
                reportWriter.AppendContentLine($"Disk Name: {disk.GetPropertyValueAsString("Name")}");
                long diskSize = disk.GetPropertyValue("Size", 0L);
                long freeSpace = disk.GetPropertyValue("FreeSpace", 0L);
                reportWriter.AppendContentLine($"Total Size: {bytesSizeConverter.ConvertBytesToReadableSize(diskSize)}");
                reportWriter.AppendContentLine($"Free Space: {bytesSizeConverter.ConvertBytesToReadableSize(freeSpace)}");
                reportWriter.AppendContentLine($"Device ID: {disk.GetPropertyValueAsString("DeviceID")}");
                reportWriter.AppendContentLine($"Volume Name: {disk.GetPropertyValueAsString("VolumeName")}");
                reportWriter.AppendContentLine($"System Name: {disk.GetPropertyValueAsString("SystemName")}");
                reportWriter.AppendContentLine($"Volume Serial Number: {disk.GetPropertyValueAsString("VolumeSerialNumber")}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectPageFileInformation()
        {
            reportWriter.AppendSectionHeader("Page File Usage");
            wmiConnectionScope.ExecuteWmiQuery("SELECT AllocatedBaseSize FROM Win32_PageFileUsage", pf =>
            {
                uint size = pf.GetPropertyValue("AllocatedBaseSize", 0U);
                reportWriter.AppendContentLine($"Allocated Base Size: {bytesSizeConverter.ConvertBytesToReadableSize(size)}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectPageFileSettingInformation()
        {
            reportWriter.AppendSectionHeader("Page File Settings");
            wmiConnectionScope.ExecuteWmiQuery("SELECT MaximumSize FROM Win32_PageFileSetting", pfs =>
            {
                uint size = pfs.GetPropertyValue("MaximumSize", 0U);
                reportWriter.AppendContentLine($"Maximum Size: {bytesSizeConverter.ConvertBytesToReadableSize(size)}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectNetworkAdaptersInformation()
        {
            reportWriter.AppendSectionHeader("Network Adapters");
            try
            {
                wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_NetworkAdapterConfiguration", nic =>
                {
                    foreach (PropertyData prop in nic.Properties)
                    {
                        if (prop.Value != null)
                            reportWriter.AppendContentLine($"{prop.Name}: {prop.Value}");
                    }
                });
            }
            catch (Exception ex)
            {
                reportWriter.AppendContentLine($"Note: Unable to retrieve network adapter information: {ex.Message}");
            }
            reportWriter.FlushReportToOutput();
        }

        private void CollectVideoControllerInformation()
        {
            reportWriter.AppendSectionHeader("Video Controller (GPU)");
            wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_VideoController", gpu =>
            {
                reportWriter.AppendContentLine($"Name: {gpu.GetPropertyValueAsString("Name")}");
                reportWriter.AppendContentLine($"Status: {gpu.GetPropertyValueAsString("Status")}");
                reportWriter.AppendContentLine($"Caption: {gpu.GetPropertyValueAsString("Caption")}");
                reportWriter.AppendContentLine($"Device ID: {gpu.GetPropertyValueAsString("DeviceID")}");
                long memory = gpu.GetPropertyValue("AdapterRAM", 0L);
                reportWriter.AppendContentLine($"Adapter RAM: {bytesSizeConverter.ConvertBytesToReadableSize(memory)}");
                reportWriter.AppendContentLine($"DAC Type: {gpu.GetPropertyValueAsString("AdapterDACType")}");
                reportWriter.AppendContentLine($"Monochrome: {gpu.GetPropertyValueAsString("Monochrome")}");
                reportWriter.AppendContentLine($"Installed Display Drivers: {gpu.GetPropertyValueAsString("InstalledDisplayDrivers")}");
                reportWriter.AppendContentLine($"Driver Version: {gpu.GetPropertyValueAsString("DriverVersion")}");
                reportWriter.AppendContentLine($"Video Processor: {gpu.GetPropertyValueAsString("VideoProcessor")}");
                reportWriter.AppendContentLine($"Video Architecture: {gpu.GetPropertyValueAsString("VideoArchitecture")}");
                reportWriter.AppendContentLine($"Video Memory Type: {gpu.GetPropertyValueAsString("VideoMemoryType")}");
            });
            reportWriter.FlushReportToOutput();
        }

        private void CollectAudioDevicesInformation()
        {
            reportWriter.AppendSectionHeader("Audio Devices (Sound)");
            try
            {
                wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_SoundDevice", sound =>
                {
                    reportWriter.AppendContentLine($"Name: {sound.GetPropertyValueAsString("Name")}");
                    reportWriter.AppendContentLine($"Product Name: {sound.GetPropertyValueAsString("ProductName")}");
                    reportWriter.AppendContentLine($"Availability: {sound.GetPropertyValueAsString("Availability")}");
                    reportWriter.AppendContentLine($"Device ID: {sound.GetPropertyValueAsString("DeviceID")}");
                    reportWriter.AppendContentLine($"Power Management Supported: {sound.GetPropertyValueAsString("PowerManagementSupported")}");
                    reportWriter.AppendContentLine($"Status: {sound.GetPropertyValueAsString("Status")}");
                    reportWriter.AppendContentLine($"Status Info: {sound.GetPropertyValueAsString("StatusInfo")}");
                });
            }
            catch (Exception ex)
            {
                reportWriter.AppendContentLine($"Note: Unable to retrieve audio device information: {ex.Message}");
            }
            reportWriter.FlushReportToOutput();
        }

        private void CollectPrintersInformation()
        {
            reportWriter.AppendSectionHeader("Printers");
            try
            {
                wmiConnectionScope.ExecuteWmiQuery("SELECT * FROM Win32_Printer", printer =>
                {
                    reportWriter.AppendContentLine($"Name: {printer.GetPropertyValueAsString("Name")}");
                    reportWriter.AppendContentLine($"Network: {printer.GetPropertyValueAsString("Network")}");
                    reportWriter.AppendContentLine($"Availability: {printer.GetPropertyValueAsString("Availability")}");
                    reportWriter.AppendContentLine($"Is Default Printer: {printer.GetPropertyValueAsString("Default")}");
                    reportWriter.AppendContentLine($"Device ID: {printer.GetPropertyValueAsString("DeviceID")}");
                    reportWriter.AppendContentLine($"Status: {printer.GetPropertyValueAsString("Status")}");
                });
            }
            catch (Exception ex)
            {
                reportWriter.AppendContentLine($"Note: Unable to retrieve printer information: {ex.Message}");
            }
            reportWriter.FlushReportToOutput();
        }
    }
}

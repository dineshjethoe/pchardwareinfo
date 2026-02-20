# PC Hardware Info

> Automate remote hardware information collection from multiple Windows computers.

## Why I Created It

Back in 2019, I needed to collect detailed hardware information from multiple remote computers for a project. Instead of logging into each machine individually, I automated the process using WMI to gather comprehensive system specs in one go.

## Tech Stack

- **Framework**: .NET Framework 4.0
- **Language**: C#
- **Library**: Windows Management Instrumentation (WMI)
- **Version**: 1.0.0.0

## Quick Start

```bash
GetHardwareInfo.exe COMPUTER1 COMPUTER2 COMPUTER3
```

Reports are automatically saved to your Desktop.

## What It Collects

- **OS**: Computer name, OS version, serial number, timezone
- **Hardware**: CPU (cores, speed, model), RAM, storage (size, free space)
- **Devices**: Network adapters, GPU, audio devices, printers
- **Software**: Installed applications
- **Other**: Page file settings

## Usage

### Single Computer
```bash
GetHardwareInfo.exe WORKSTATION-01
```

### Multiple Computers
```bash
GetHardwareInfo.exe WORKSTATION-01 WORKSTATION-02 WORKSTATION-03
```

### With Domain
```bash
GetHardwareInfo.exe DOMAIN\COMPUTERNAME
```

The app will prompt for credentials if needed. Results include a summary table showing success/failure for each computer.

## Requirements

- Windows Vista or later
- .NET Framework 4.0 or higher
- Admin privileges (recommended)
- Network access to target computers
- WMI enabled on target computers

## Notes

- All reports saved to Desktop in text format
- Handles authentication for secured networks
- Graceful error handling and continues processing even if one computer fails
- Each report includes computer name and timestamp


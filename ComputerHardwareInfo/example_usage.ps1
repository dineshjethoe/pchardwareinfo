# ============================================================================
# Computer Hardware Information Collector - PowerShell Example
# ============================================================================
# This PowerShell script demonstrates various ways to use the
# ComputerHardwareInfo.exe application.
#
# USAGE:
# 1. Right-click PowerShell and select "Run as Administrator"
# 2. Navigate to the folder containing this script
# 3. Run: .\example_usage.ps1
# ============================================================================

# Set error action preference
$ErrorActionPreference = "Continue"

# Define color functions for better readability
function Write-Success {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Red
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Yellow
}

function Write-Info {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Cyan
}

# Get the directory where the script is located
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$AppPath = Join-Path $ScriptDirectory "ComputerHardwareInfo.exe"

# Function to check if app exists
function Test-AppExists {
    if (Test-Path $AppPath) {
        Write-Success "Found ComputerHardwareInfo.exe"
        return $true
    } else {
        Write-Error-Custom "ComputerHardwareInfo.exe not found at: $AppPath"
        Write-Info "Please ensure this script is in the same folder as the executable"
        return $false
    }
}

# Function to collect from a single computer
function Collect-SingleComputer {
    param([string]$ComputerName)
    
    if ([string]::IsNullOrWhiteSpace($ComputerName)) {
        Write-Error-Custom "Computer name cannot be empty"
        return
    }
    
    Write-Info "Collecting hardware information from: $ComputerName"
    & $AppPath $ComputerName
}

# Function to collect from multiple computers
function Collect-MultipleComputers {
    param([string[]]$ComputerNames)
    
    Write-Info "Processing $($ComputerNames.Count) computer(s)"
    foreach ($Computer in $ComputerNames) {
        Write-Info "Processing: $Computer"
        & $AppPath $Computer
    }
}

# Function to collect from computers listed in a text file
function Collect-FromFile {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        Write-Error-Custom "File not found: $FilePath"
        return
    }
    
    $Computers = @(Get-Content $FilePath | Where-Object { $_ -match '\S' })
    
    if ($Computers.Count -eq 0) {
        Write-Error-Custom "No computer names found in file"
        return
    }
    
    Write-Success "Found $($Computers.Count) computer(s) in file"
    Collect-MultipleComputers $Computers
}

# Function to collect from all computers in Active Directory (Domain only)
function Collect-FromActiveDirectory {
    param([string]$Filter = "*")
    
    try {
        Write-Info "Searching Active Directory for computers (this may take a moment)..."
        $ADComputers = Get-ADComputer -Filter { Name -like $Filter } | Select-Object -ExpandProperty Name
        
        if ($ADComputers.Count -eq 0) {
            Write-Error-Custom "No computers found matching filter: $Filter"
            return
        }
        
        Write-Success "Found $($ADComputers.Count) computer(s) in Active Directory"
        Write-Warning-Custom "This will process many computers. Do you want to continue? (Y/N)"
        $Response = Read-Host
        
        if ($Response -eq "Y" -or $Response -eq "y") {
            Collect-MultipleComputers $ADComputers
        } else {
            Write-Info "Operation cancelled"
        }
    }
    catch {
        Write-Error-Custom "Failed to query Active Directory: $_"
        Write-Info "Note: This feature requires Active Directory module and domain connectivity"
    }
}

# Function to show help
function Show-Help {
    Write-Host @"
??????????????????????????????????????????????????????????????????????????????
?              Computer Hardware Information Collector                       ?
?              PowerShell Example Usage Script                               ?
??????????????????????????????????????????????????????????????????????????????

EXAMPLES:

1. Interactive Mode (Manual Entry):
   PS> Collect-SingleComputer -ComputerName "DESKTOP-01"

2. Multiple Computers:
   PS> Collect-MultipleComputers -ComputerNames @("DESKTOP-01", "SERVER-02", "LAPTOP-03")

3. From Text File (computers.txt):
   PS> Collect-FromFile -FilePath ".\computers.txt"

4. From Active Directory (Domain computers):
   PS> Collect-FromActiveDirectory -Filter "DESKTOP*"

5. Show This Help:
   PS> Show-Help

COMMAND-LINE ARGUMENTS:

To run the executable directly with PowerShell:
   PS> & "$AppPath" DESKTOP-01
   PS> & "$AppPath" DESKTOP-01 SERVER-02 LAPTOP-03

EXIT CODES:
   0 - Success (at least one computer processed)
   1 - Failure (no computers processed successfully)
   2 - Fatal error

NOTES:
- Computer names can be NetBIOS names, FQDNs, or IP addresses
- Valid characters: letters, numbers, hyphens (-), underscores (_), dots (.)
- Reports are saved to: %USERPROFILE%\Desktop\
- Run as Administrator for best results, especially for remote computers

For more information, run:
   PS> & "$AppPath" /?

"@
}

# Main menu
function Show-Menu {
    Clear-Host
    Write-Host ""
    Write-Host "??????????????????????????????????????????????????????????????????????????????"
    Write-Host "?         Computer Hardware Information Collector - PowerShell Menu          ?"
    Write-Host "??????????????????????????????????????????????????????????????????????????????"
    Write-Host ""
    Write-Host "1. Single Computer (Manual Entry)"
    Write-Host "2. Multiple Computers (Pre-defined List)"
    Write-Host "3. Computers from File (computers.txt)"
    Write-Host "4. Computers from Active Directory (Domain)"
    Write-Host "5. Interactive Mode"
    Write-Host "6. Show Help"
    Write-Host "7. Exit"
    Write-Host ""
    $Choice = Read-Host "Enter your choice (1-7)"
    
    switch ($Choice) {
        "1" {
            $ComputerName = Read-Host "Enter computer name"
            Collect-SingleComputer -ComputerName $ComputerName
            Pause
            Show-Menu
        }
        "2" {
            $Computers = @("DESKTOP-01", "SERVER-02", "LAPTOP-03")
            Write-Info "Processing example computers: $($Computers -join ', ')"
            Collect-MultipleComputers -ComputerNames $Computers
            Pause
            Show-Menu
        }
        "3" {
            $FilePath = Read-Host "Enter file path (default: .\computers.txt)"
            if ([string]::IsNullOrWhiteSpace($FilePath)) {
                $FilePath = ".\computers.txt"
            }
            Collect-FromFile -FilePath $FilePath
            Pause
            Show-Menu
        }
        "4" {
            $Filter = Read-Host "Enter computer name filter (default: *)"
            if ([string]::IsNullOrWhiteSpace($Filter)) {
                $Filter = "*"
            }
            Collect-FromActiveDirectory -Filter $Filter
            Pause
            Show-Menu
        }
        "5" {
            Write-Info "Running in interactive mode..."
            & $AppPath
            Show-Menu
        }
        "6" {
            Show-Help
            Pause
            Show-Menu
        }
        "7" {
            Write-Success "Thank you for using Computer Hardware Information Collector!"
            exit
        }
        default {
            Write-Error-Custom "Invalid choice"
            Start-Sleep -Seconds 2
            Show-Menu
        }
    }
}

# Main script execution
Write-Host ""
Write-Host "??????????????????????????????????????????????????????????????????????????????"
Write-Host "?         Computer Hardware Information Collector                           ?"
write-Host "?         PowerShell Helper Script                                          ?"
Write-Host "??????????????????????????????????????????????????????????????????????????????"
Write-Host ""

# Check if app exists
if (-not (Test-AppExists)) {
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Success "Ready to collect hardware information"
Write-Host ""

# Show menu
Show-Menu

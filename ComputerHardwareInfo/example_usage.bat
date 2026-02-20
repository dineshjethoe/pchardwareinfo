@echo off
REM ============================================================================
REM Computer Hardware Information Collector - Example Usage
REM ============================================================================
REM This batch file demonstrates how to use the ComputerHardwareInfo.exe
REM application with various scenarios.
REM
REM USAGE: Copy this file to the same folder as ComputerHardwareInfo.exe
REM        or adjust the path to match your installation.
REM ============================================================================

setlocal enabledelayedexpansion

REM Set the path to the executable (adjust if needed)
set "APP_PATH=ComputerHardwareInfo.exe"

REM Color codes for output
set "COLOR_INFO=0A"
set "COLOR_SUCCESS=0A"
set "COLOR_ERROR=0C"
set "COLOR_WARNING=0E"

echo.
echo ============================================================================
echo Computer Hardware Information Collector - Example Batch Script
echo ============================================================================
echo.

REM Check if the application exists
if not exist "%APP_PATH%" (
    color %COLOR_ERROR%
    echo ERROR: ComputerHardwareInfo.exe not found!
    echo Please copy this script to the same folder as ComputerHardwareInfo.exe
    echo.
    pause
    exit /b 1
)

REM Display menu
:MENU
cls
echo.
echo ============================================================================
echo Choose an option:
echo ============================================================================
echo.
echo 1. Interactive Mode - Enter computer names when prompted
echo 2. Single Computer - Collect from one computer
echo 3. Multiple Computers - Collect from several computers
echo 4. Process Computers from File - Read from computers.txt
echo 5. Advanced Mode - Show all available options
echo.
echo 0. Exit
echo.

set /p CHOICE="Enter your choice (0-5): "

if "%CHOICE%"=="0" goto EXIT
if "%CHOICE%"=="1" goto INTERACTIVE_MODE
if "%CHOICE%"=="2" goto SINGLE_COMPUTER
if "%CHOICE%"=="3" goto MULTIPLE_COMPUTERS
if "%CHOICE%"=="4" goto PROCESS_FROM_FILE
if "%CHOICE%"=="5" goto ADVANCED_MODE

echo Invalid choice. Please try again.
timeout /t 2 /nobreak
goto MENU

REM ============================================================================
REM INTERACTIVE MODE
REM ============================================================================
:INTERACTIVE_MODE
cls
echo.
echo Running in Interactive Mode...
echo You will be prompted to enter computer names.
echo.
pause

"%APP_PATH%"
goto MENU

REM ============================================================================
REM SINGLE COMPUTER
REM ============================================================================
:SINGLE_COMPUTER
cls
echo.
set /p COMPUTER_NAME="Enter the computer name to scan: "

if "!COMPUTER_NAME!"=="" (
    echo No computer name entered.
    goto MENU
)

echo.
echo Processing: !COMPUTER_NAME!
echo.

"%APP_PATH%" !COMPUTER_NAME!

if %ERRORLEVEL% == 0 (
    echo.
    echo Success! Report saved to Desktop.
    echo.
) else (
    echo.
    echo Failed to collect information from !COMPUTER_NAME!
    echo.
)

timeout /t 2 /nobreak
goto MENU

REM ============================================================================
REM MULTIPLE COMPUTERS
REM ============================================================================
:MULTIPLE_COMPUTERS
cls
echo.
echo Processing multiple computers...
echo.

REM Example: Collect from three computers
REM Modify these computer names as needed
set COMPUTERS=DESKTOP-01 SERVER-02 LAPTOP-03

echo Computers to process:
for %%C in (%COMPUTERS%) do echo   - %%C
echo.
pause

"%APP_PATH%" %COMPUTERS%

echo.
echo Batch processing complete!
echo.
timeout /t 2 /nobreak
goto MENU

REM ============================================================================
REM PROCESS FROM FILE
REM ============================================================================
:PROCESS_FROM_FILE
cls
echo.

REM Check if computers.txt exists
if not exist "computers.txt" (
    echo ERROR: computers.txt not found!
    echo.
    echo Please create a file named "computers.txt" in the same folder with one
    echo computer name per line. Example:
    echo.
    echo     DESKTOP-01
    echo     DESKTOP-02
    echo     SERVER-01
    echo     SERVER-02
    echo.
    pause
    goto MENU
)

echo Processing computers from computers.txt...
echo.

setlocal enabledelayedexpansion
set COUNT=0
for /f "delims=" %%I in (computers.txt) do (
    set /a COUNT=!COUNT!+1
    echo [!COUNT!] Processing: %%I
    "%APP_PATH%" %%I
    echo.
)

echo.
echo All !COUNT! computer(s) processed!
echo Reports saved to Desktop.
echo.
pause
goto MENU

REM ============================================================================
REM ADVANCED MODE
REM ============================================================================
:ADVANCED_MODE
cls
echo.
echo Advanced Options:
echo.
echo 1. Show Help Information
echo 2. Run with Custom Computer List
echo 3. Run as Administrator
echo 4. Back to Main Menu
echo.

set /p ADVANCED_CHOICE="Enter your choice (1-4): "

if "%ADVANCED_CHOICE%"=="1" goto SHOW_HELP
if "%ADVANCED_CHOICE%"=="2" goto CUSTOM_LIST
if "%ADVANCED_CHOICE%"=="3" goto RUN_AS_ADMIN
if "%ADVANCED_CHOICE%"=="4" goto MENU

goto ADVANCED_MODE

:SHOW_HELP
cls
echo.
"%APP_PATH%" /?
echo.
pause
goto ADVANCED_MODE

:CUSTOM_LIST
cls
echo.
echo Enter computer names separated by spaces:
echo Example: DESKTOP-01 SERVER-02 LAPTOP-03
echo.
set /p CUSTOM_COMPUTERS="Computer names: "

if "!CUSTOM_COMPUTERS!"=="" (
    echo No computer names entered.
    goto ADVANCED_MODE
)

echo.
echo Processing: !CUSTOM_COMPUTERS!
echo.

"%APP_PATH%" !CUSTOM_COMPUTERS!

echo.
timeout /t 2 /nobreak
goto ADVANCED_MODE

:RUN_AS_ADMIN
cls
echo.
echo This script will attempt to elevate privileges...
echo You may be prompted by User Account Control.
echo.

REM Check if running as administrator
net session >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo Elevation required. Please allow the prompt.
    powershell -Command "Start-Process cmd -ArgumentList '/c cd /d %CD% ^& %APP_PATH%' -Verb RunAs"
    goto ADVANCED_MODE
) else (
    echo Already running as administrator.
    "%APP_PATH%"
    goto ADVANCED_MODE
)

REM ============================================================================
REM EXIT
REM ============================================================================
:EXIT
cls
echo.
echo Thank you for using Computer Hardware Information Collector!
echo.
echo Reports have been saved to your Desktop.
echo.
pause
exit /b 0

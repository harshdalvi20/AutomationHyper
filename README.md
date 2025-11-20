# Hyperspectral Automation Project

## Overview
This project automates the testing of a mobile application using Appium and NUnit. It includes workflows for login, patient registration, and dashboard navigation.

## Prerequisites
Before setting up the project, ensure you have the following installed:

1. **.NET 8 SDK**
   - Download and install from [Microsoft .NET](https://dotnet.microsoft.com/).

2. **Appium**
   - Install Appium globally (recommend v2.19.0 or later): ```sh
 npm install -g appium@2.19.0appium -v # Expected: 2.19.0  - Download UIAutomator2 Appium Driver for Android: ```sh
 appium driver install uiautomator23. **Android Emulator or Device**
  - Set up an Android emulator or connect a physical Android device.
  - Ensure the device is visible via `adb devices`.

4. **ChromeDriver**
  - Download the appropriate ChromeDriver version for your WebView.
  - Place it in the directory specified in `Config/appiumSettings.json`.

5. **Environment Variables**
  - Set the following environment variables:
    - `TEST_LOGIN_USERNAME`: Username for login.
    - `TEST_LOGIN_PASSWORD`: Password for login.
    - `TEST_INPUT_OFFSET`: Offset for generating unique patient data (optional).
    - `ANDROID_HOME`: Path to the Android SDK (e.g., `$HOME/Library/Android/sdk`).


## Required Packages

The following NuGet packages are required for this project:

| Package Name           | Version  |
|------------------------|----------|
| Appium.WebDriver       | 6.0.1    |
| coverlet.collector     | 6.0.0    |
| Microsoft.NET.Test.Sdk | 17.8.0   |
| Newtonsoft.Json        | 13.0.3   |
| NUnit                  | 3.13.3   |
| NUnit.Analyzers        | 3.9.0    |
| NUnit3TestAdapter      | 4.4.2    |
| Selenium.WebDriver     | 4.26.1   |

### Installing Packages

To install the required packages, run the following command in the project directory:dotnet restore
This will automatically download and install all the dependencies specified in the `HyperspectralAutomation.csproj` file.

## Running the Tests

1. **Start the Appium Server**
   - Ensure the Appium server is running at `http://127.0.0.1:4723/`.

2. **Launch the Emulator or Connect a Device**
   - Start an Android emulator or connect a physical device.
   - Verify the device is visible using: ```bash
 adb devices3. **Run the Tests in Visual Studio 2022**
   - Open the solution in Visual Studio 2022.
   - Build the solution to ensure there are no compilation errors.
   - Open the **Test Explorer** window (from the menu: `Test > Test Explorer`).
   - Click on **Run All** to execute all the tests in the project.
   - Alternatively, you can run individual tests by right-clicking on a test method and selecting **Run**.

4. **View Test Results**
   - The test results will be displayed in the **Test Explorer** window.
   - For detailed logs, check the **Output** window in Visual Studio.

## Automated Setup and Test Execution

### 1. Run Setup Script

This script checks environment dependencies, installs Appium drivers, verifies devices, and starts the Appium server.
chmod +x setup-mobile-automation.sh
./setup-mobile-automation.sh --platform <Android|iOS> [--apk /path/to/app.apk]
Notes:
- If the app is already installed on the device, no need to provide `--apk`.
- The script will skip installation if no file is provided.

This script will:
- Verify the presence of required tools: Node.js, npm, .NET SDK, ADB, Appium, and platform-specific drivers.
- Ensure `ANDROID_HOME` is correctly set (for Android).
- Install Appium drivers (uiautomator2 for Android) if not already installed.
- Start the Appium server in the background.
- Verify a connected emulator or physical device (for the specified platform).
- Install the APK only if a path is provided.

### 2. Run Tests

Once the environment is up, use the test runner script to execute platform-specific tests using NUnit and .NET CLI:
chmod +x run-tests.sh
# Run Android tests
./run-tests.sh --platform Android
This script:
- Builds the test project.
- Runs only tests tagged for the selected platform.
- Stores test results in the `test-results` directory in TRX format.

## Configuration

- **Appium Settings**
  - Modify `Config/appiumSettings.json` to update Appium configurations such as `appPackage`, `appActivity`, and `deviceName`.

- **Patient Data**
  - Update `Config/patientData.json` to provide base patient data for registration.

- **Generated Patients**
  - The project appends generated patient data to `Config/GeneratedPatients.json` for recordkeeping.

## Troubleshooting

- **Appium Connection Issues**
  - Ensure the Appium server is running and accessible at `http://127.0.0.1:4723/`.

- **Device Not Found**
  - Verify the device is visible using `adb devices`.

- **Test Failures**
  - Check the console output for detailed error messages.
  - Ensure the environment variables are set correctly.

using System;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace HyperspectralAutomation.Utils
{
	public static class DriverFactory
	{
		private class AppiumSettings
		{
			public string AppiumServerUrl { get; set; } = "http://127.0.0.1:4723/";
			public string PlatformName { get; set; } = "Android";
			public string DeviceName { get; set; } = "emulator-5554";
			public string AutomationName { get; set; } = "UiAutomator2";
			public string AppPackage { get; set; } = "com.iprd.anc.nigeriaoyo";
			public string AppActivity { get; set; } = "com.iprd.anc.nigeriaoyo.ui.features.login.LoginActivity";
			public bool AutoGrantPermissions { get; set; } = true;
			public int UiAutomator2ServerInstallTimeout { get; set; } = 60000;
			public int ImplicitWaitSeconds { get; set; } = 10;

			// Path to folder containing all chromedriver executables
			//public string ChromeDriverDirectory { get; set; } = @"C:\Users\harsh\Downloads\chromedriver";
		}

		public static AndroidDriver Create(bool resetApp = true)
		{
			var settings = LoadSettings();

			var options = new AppiumOptions
			{
				PlatformName = settings.PlatformName,
				DeviceName = settings.DeviceName,
				AutomationName = settings.AutomationName
			};

			options.AddAdditionalAppiumOption("appPackage", settings.AppPackage);
			options.AddAdditionalAppiumOption("appActivity", settings.AppActivity);
			options.AddAdditionalAppiumOption("autoGrantPermissions", settings.AutoGrantPermissions);
			options.AddAdditionalAppiumOption("uiautomator2ServerInstallTimeout", settings.UiAutomator2ServerInstallTimeout);

			// Automatically pick the correct ChromeDriver for the WebView version
			//options.AddAdditionalAppiumOption("chromedriverExecutableDir", settings.ChromeDriverDirectory);
			
			options.AddAdditionalAppiumOption("chromedriverAutodownload", true);

			options.AddAdditionalAppiumOption("ignoreHiddenApiPolicyError", true);

			// Optional: help Appium avoid WebView timing issues
			options.AddAdditionalAppiumOption("ensureWebviewsHavePages", true);
			options.AddAdditionalAppiumOption("newCommandTimeout", 3600);

			// Add option to not reset the app if resetApp is false
			if (!resetApp)
			{
				options.AddAdditionalAppiumOption("noReset", true);
			}

			var driver = new AndroidDriver(new Uri(settings.AppiumServerUrl), options);
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settings.ImplicitWaitSeconds);
			return driver;
		}

		private static AppiumSettings LoadSettings()
		{
			var path = Path.Combine("Config", "appiumSettings.json");
			if (!File.Exists(path)) return new AppiumSettings();
			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<AppiumSettings>(json) ?? new AppiumSettings();
		}
	}
}

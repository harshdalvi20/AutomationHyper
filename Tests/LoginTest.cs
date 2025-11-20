using HyperspectralAutomation.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace HyperspectralAutomation.Tests
{
	[TestFixture]
	public class LoginTest
	{
		private AndroidDriver? driver;

		[SetUp]
		public void Setup()
		{
			driver = DriverFactory.Create();
		}

		[Test]
		public void TapLogin_OpensWebpage_WaitsUpTo20Seconds()
		{
			Assert.IsNotNull(driver, "Driver not initialized.");

			// Tap login button
			var loginButton = driver.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/login_button"));
			loginButton.Click();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
			string? webviewContext = null;

			// ✅ Wait for WebView
			wait.Until(d =>
			{
				var contexts = ((AndroidDriver)d).Contexts;
				webviewContext = contexts.FirstOrDefault(c =>
					c.Contains("WEBVIEW", StringComparison.OrdinalIgnoreCase));
				return webviewContext != null;
			});

			Console.WriteLine("Available contexts: " + string.Join(", ", driver.Contexts));
			driver.Context = webviewContext!;
			Console.WriteLine("Switched to context: " + driver.Context);

			var webWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			bool showLoginForm = true;

			try
			{
				webWait.Until(d =>
					d.FindElements(By.CssSelector("input#username")).Any());
			}
			catch (WebDriverTimeoutException)
			{
				showLoginForm = false;  // ✅ Already logged in earlier
			}

			Console.WriteLine("Is first time login: " + showLoginForm);

			if (showLoginForm)
			{
				// Try new names first, then fall back to older names for compatibility
				var username = Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME_HYPER")?.Trim()
				               ?? Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME")?.Trim();
				var password = Environment.GetEnvironmentVariable("TEST_LOGIN_PASSWORD_HYPER")?.Trim()
				               ?? Environment.GetEnvironmentVariable("TEST_LOGIN_PASSWORD")?.Trim();

				// Debug: indicate presence of env vars (do NOT print secret values)
				bool usernamePresent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME_HYPER"));
				bool passwordPresent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_LOGIN_PASSWORD_HYPER"));
				Console.WriteLine($"Env username present: {usernamePresent}");
				Console.WriteLine($"Env password present: {passwordPresent}");

				if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				{
					Assert.Fail("Environment variables TEST_LOGIN_USERNAME_HYPER/TEST_LOGIN_USERNAME and TEST_LOGIN_PASSWORD_HYPER/TEST_LOGIN_PASSWORD must be set.");
				}

				driver.FindElement(By.CssSelector("input#username")).SendKeys(username);
				driver.FindElement(By.CssSelector("input#password")).SendKeys(password);
				driver.FindElement(By.Id("kc-login")).Click();
				Console.WriteLine("Login submitted.");
			}
			else
			{
				Console.WriteLine("Login form skipped (session already active).");
			}

			// ✅ Always switch back
			driver.Context = "NATIVE_APP";
		}

		[TearDown]
		public void Cleanup()
		{
			driver?.Quit();
			driver?.Dispose();
		}
	}
}

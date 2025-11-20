using HyperspectralAutomation.Utils;
using HyperspectralAutomation.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace HyperspectralAutomation.Tests
{
	[TestFixture]
	public class HCWDetailsTest
	{
		private AndroidDriver? driver;
		private WebDriverWait? wait;

		[SetUp]
		public void Setup()
		{
			driver = DriverFactory.Create();
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
		}

		private string NormalizeName(string name)
		{
			return name
				.ToLower()
				.Replace("_", "")
				.Replace(" ", "")
				.Trim();
		}

		[Test]
		public void SelectHCWPractitionerAfterLogin()
		{
			Assert.NotNull(driver);
			Assert.NotNull(wait);

			// ✅ Perform login
			LoginHelper.PerformLogin(driver, wait);

			// ✅ Ensure native context
			try { driver.Context = "NATIVE_APP"; } catch { }

			string expectedPractitioner =
				Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME")?.Trim()
				?? throw new Exception("TEST_LOGIN_USERNAME not set!");

			// ✅ Wait for dropdown
			var practitionerDropdown = wait.Until(d =>
			{
				try
				{
					var el = d.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/text_input_end_icon"));
					return (el.Displayed && el.Enabled) ? el : null;
				}
				catch { return null; }
			});

			// ✅ Get current practitioner text BEFORE clicking dropdown
			var selectedTextElement = driver.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/auto_complete"));
			string actualPractitioner = selectedTextElement.Text.Trim();

			Console.WriteLine($"Expected: {expectedPractitioner}");
			Console.WriteLine($"Actual: {actualPractitioner}");

			// ✅ Normalize for comparison
			string expectedNorm = NormalizeName(expectedPractitioner);
			string actualNorm = NormalizeName(actualPractitioner);

			Console.WriteLine($"Expected Normalized: {expectedNorm}");
			Console.WriteLine($"Actual Normalized: {actualNorm}");

			Assert.AreEqual(expectedNorm, actualNorm,
				$"Mismatch between environment username and HCW practitioner displayed!" +
				$"\nExpected(original): {expectedPractitioner}" +
				$"\nActual(original): {actualPractitioner}" +
				$"\nExpected(norm): {expectedNorm}" +
				$"\nActual(norm): {actualNorm}");

			practitionerDropdown.Click();

			var continueButton = wait.Until(d =>
			{
				try
				{
					var btn = d.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/continue_with_practitioner"));
					return (btn.Displayed && btn.Enabled) ? btn : null;
				}
				catch { return null; }
			});

			continueButton.Click();

			Thread.Sleep(5000);
		}

		[TearDown]
		public void Cleanup()
		{
			if (driver != null)
			{
				driver.Quit();
				driver.Dispose();
				driver = null;
			}
		}
	}
}

using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace HyperspectralAutomation.Tests
{
	public static class HCWWorkflow
	{
		public static void SelectHCWPractitioner(AndroidDriver driver, WebDriverWait wait)
		{
			if (driver == null) throw new ArgumentNullException(nameof(driver));
			if (wait == null) throw new ArgumentNullException(nameof(wait));

			// Ensure native context
			try { driver.Context = "NATIVE_APP"; } catch { }

			string expectedPractitioner =
				Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME")?.Trim()
				?? throw new Exception("Environment variable TEST_LOGIN_USERNAME not set!");

			// Wait for dropdown
			var practitionerDropdown = wait.Until(d =>
			{
				try
				{
					var el = d.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/text_input_end_icon"));
					return (el.Displayed && el.Enabled) ? el : null;
				}
				catch { return null; }
			});

			// Get current practitioner text BEFORE clicking dropdown
			var selectedTextElement = driver.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/auto_complete"));
			string actualPractitioner = selectedTextElement.Text.Trim();

			string NormalizeName(string name) =>
				name.ToLower().Replace("_", "").Replace(" ", "").Trim();

			string expectedNorm = NormalizeName(expectedPractitioner);
			string actualNorm = NormalizeName(actualPractitioner);

			if (!string.Equals(expectedNorm, actualNorm, StringComparison.Ordinal))
			{
				throw new InvalidOperationException(
					$"Mismatch between environment username and HCW practitioner displayed. Expected(norm): {expectedNorm}, Actual(norm): {actualNorm}");
			}

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
			Console.WriteLine("Continue Button clicked");

            // keep existing behavior (small delay)
            Thread.Sleep(5000);
		}
	}
}
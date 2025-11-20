using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;

namespace HyperspectralAutomation.Pages
{
	public class HCWDetailsPage
	{
		private AndroidDriver driver;
		private WebDriverWait wait;

		public HCWDetailsPage(AndroidDriver driver, WebDriverWait wait)
		{
			this.driver = driver;
			this.wait = wait;
		}

		private By PractitionerField => By.Id("com.iprd.anc.nigeriaoyo:id/auto_complete");
		private By PractitionerDropdown => By.Id("com.iprd.anc.nigeriaoyo:id/text_input_end_icon");
		private By ContinueButton => By.Id("com.iprd.anc.nigeriaoyo:id/continue_with_practitioner");

		public string GetSelectedPractitioner()
		{
			return driver.FindElement(PractitionerField).Text.Trim();
		}

		public void OpenPractitionerDropdown()
		{
			wait.Until(d => d.FindElement(PractitionerDropdown).Displayed);
			driver.FindElement(PractitionerDropdown).Click();
		}

		public void ClickContinue()
		{
			wait.Until(d => d.FindElement(ContinueButton).Enabled);
			driver.FindElement(ContinueButton).Click();
		}
	}
}

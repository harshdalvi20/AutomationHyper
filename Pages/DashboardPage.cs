using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;

namespace MobileTestDemo.Pages
{
	public class DashboardPage
	{
		private AndroidDriver driver;
		private WebDriverWait wait;

		public DashboardPage(AndroidDriver driver, WebDriverWait wait)
		{
			this.driver = driver;
			this.wait = wait;
		}

		private By PatientRegistration =>
			By.XPath("(//androidx.cardview.widget.CardView[@resource-id=\"com.iprd.anc.nigeriaoyo:id/btnPatientCheckIn\"])[1]/android.view.ViewGroup");

		private By PatientList =>
			By.XPath("(//androidx.cardview.widget.CardView[@resource-id=\"com.iprd.anc.nigeriaoyo:id/btnPatientCheckIn\"])[2]/android.view.ViewGroup");

		private By SearchPatient =>
			By.XPath("(//androidx.cardview.widget.CardView[@resource-id=\"com.iprd.anc.nigeriaoyo:id/btnPatientCheckIn\"])[3]/android.view.ViewGroup");

		private By WorkLog =>
			By.XPath("(//androidx.cardview.widget.CardView[@resource-id=\"com.iprd.anc.nigeriaoyo:id/btnPatientCheckIn\"])[4]/android.view.ViewGroup");

		public void ClickPatientRegistration()
		{
			wait.Until(d => d.FindElement(PatientRegistration).Enabled);
			driver.FindElement(PatientRegistration).Click();
		}

		public void ClickPatientList()
		{
			wait.Until(d => d.FindElement(PatientList).Enabled);
			driver.FindElement(PatientList).Click();
		}

		public void ClickSearchPatient()
		{
			wait.Until(d => d.FindElement(SearchPatient).Enabled);
			driver.FindElement(SearchPatient).Click();
		}

		public void ClickWorkLog()
		{
			wait.Until(d => d.FindElement(WorkLog).Enabled);
			driver.FindElement(WorkLog).Click();
		}
	}
}

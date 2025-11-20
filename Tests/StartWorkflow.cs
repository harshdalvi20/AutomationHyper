using HyperspectralAutomation.Utils;
using HyperspectralAutomation.Helpers;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using NUnit.Framework;
using HyperspectralAutomation.Pages;
using MobileTestDemo.Pages;
using OpenQA.Selenium;

namespace HyperspectralAutomation.Tests
{
	[TestFixture]
	public class StartWorkflow
	{
		private AndroidDriver? driver;
		private WebDriverWait? wait;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			driver = DriverFactory.Create();
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
		}

		// Optional: run before each test if you need to restore app state without closing the app.
		[SetUp]
		public void SetupPerTest()
		{
			Assert.NotNull(driver, "Driver not initialized in OneTimeSetup.");
			Assert.NotNull(wait, "Wait not initialized in OneTimeSetup.");
			// Example: bring app to foreground or reset a screen if needed:
			// driver.ActivateApp("com.iprd.anc.nigeriaoyo");
		}

		[Test, Order(1)]
		public void LoginTest()
		{
			// guard
			if (driver == null || wait == null) Assert.Fail("Test infrastructure not initialized.");

			LoginHelper.PerformLogin(driver, wait);

				 // Log success message
			
		}

		[Test, Order(2)]
		public void HCWTest()
		{
			if (driver == null || wait == null) Assert.Fail("Test infrastructure not initialized.");

			HCWWorkflow.SelectHCWPractitioner(driver, wait);
			Assert.Pass();
		}

		[Test, Order(3)]
		public void DashBoardTest()
		{
			// Create an instance of DashboardPage with required constructor parameters
			// Assuming you need to pass driver and wait to the constructor
			var dashboardPage = new DashboardPage(driver, wait);
			dashboardPage.ClickPatientRegistration();
		}

		[Test, Order(4)]
		public void TestPatientRegistration()
		{
			// Fill patient details from JSON
			PatientRegHelper.FillPatientDetails(driver, wait);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			// Quit + Dispose once after all tests in this fixture
			try { driver?.Quit(); } catch { }
			try { driver?.Dispose(); } catch { }
			driver = null;
			wait = null;
		}
	}
}
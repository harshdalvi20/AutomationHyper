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
	public class PatientRegistrationDirectTest
	{
		private AndroidDriver? driver;
		private WebDriverWait? wait;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			driver = DriverFactory.Create(resetApp: false); // Prevent app reset
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
		}

		[Test, Order(1)]
		public void TestPatientRegistrationDirectly()
		{
			// Ensure the app is already open and on the dashboard
			if (driver == null || wait == null) Assert.Fail("Test infrastructure not initialized.");

			try
			{
				// Navigate directly to the Patient Registration page
				var dashboardPage = new DashboardPage(driver, wait);
				dashboardPage.ClickPatientRegistration();

				// Fill patient details
				PatientRegHelper.FillPatientDetails(driver, wait);

				Console.WriteLine("Patient registration completed successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Test failed: {ex.Message}");
				Assert.Fail($"Test failed: {ex.Message}");
			}
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
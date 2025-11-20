using HyperspectralAutomation.Pages;
using HyperspectralAutomation.Utils;
using Newtonsoft.Json;
using System.IO;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace HyperspectralAutomation.Helpers
{
	public static class PatientRegHelper
	{
		public static void FillPatientDetails(AndroidDriver driver, WebDriverWait wait)
		{
			Console.WriteLine("---- PATIENT REGISTRATION START ----");

			int offset = 1;
			var env = Environment.GetEnvironmentVariable("TEST_INPUT_OFFSET");
			if (!string.IsNullOrEmpty(env) && int.TryParse(env, out var parsed))
				offset = Math.Max(0, parsed);

			// 1️⃣ Load data from patientData.json and apply offset
			var data = PatientDataLoader.LoadPatientDataWithOffset(offset);
			if (data == null)
				throw new InvalidOperationException("❌ Failed to load patient data from patientData.json.");

			Console.WriteLine($"📁 Loaded patient data from JSON with offset = {offset}");
			Console.WriteLine($"Patient Data: {data.FirstName} {data.MiddleName} {data.LastName} | " +
							  $"{data.Gender} | DOB: {data.DateOfBirth} | " +
							  $"{data.PhoneCode}{data.PhoneNumber}");

			// 2️⃣ Fill details in the app
			var page = new PatientRegPage(driver, wait);
			page.FillBasicDetails(
				data.FirstName,
				data.MiddleName,
				data.LastName,
				data.Gender,
				data.DateOfBirth,
				data.PhoneCode,
				data.PhoneNumber
			);

			// 3️⃣ Append used data to GeneratedPatients.json for recordkeeping
			SaveGeneratedRecord(data);

			Console.WriteLine("---- PATIENT REGISTRATION COMPLETED ----");
		}

		private static void SaveGeneratedRecord(PatientData data)
		{
			try
			{
				var dir = Path.Combine(AppContext.BaseDirectory, "Config");
				Directory.CreateDirectory(dir);
				var filePath = Path.Combine(dir, "GeneratedPatients.json");

				List<PatientData> list;
				if (File.Exists(filePath))
				{
					var existing = File.ReadAllText(filePath);
					list = JsonConvert.DeserializeObject<List<PatientData>>(existing) ?? new List<PatientData>();
				}
				else
				{
					list = new List<PatientData>();
				}

				list.Add(data);
				File.WriteAllText(filePath, JsonConvert.SerializeObject(list, Formatting.Indented));

				Console.WriteLine($"✅ Appended generated record to: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"⚠️ Failed to append generated record: {ex.Message}");
			}
		}
	}
}

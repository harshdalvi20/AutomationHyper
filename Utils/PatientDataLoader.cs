using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace HyperspectralAutomation.Utils
{
	public class PatientData
	{
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string DateOfBirth { get; set; }
		public string PhoneCode { get; set; }
		public string PhoneNumber { get; set; }
	}

	public static class PatientDataLoader
	{
		private static readonly string GeneratedFile =
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\GeneratedPatients.json"));

		private static string? ResolveFilePath(string fileName)
		{
			if (File.Exists(fileName))
				return Path.GetFullPath(fileName);

			var baseDir = AppContext.BaseDirectory;
			var currentDir = Directory.GetCurrentDirectory();

			var candidates = new[]
			{
				Path.Combine(baseDir, fileName),
				Path.Combine(baseDir, "Config", fileName),
				Path.Combine(currentDir, fileName),
				Path.Combine(currentDir, "Config", fileName),
				Path.Combine(baseDir, @"..\..\..\", "Config", fileName)
			};

			return candidates.FirstOrDefault(File.Exists);
		}

		public static PatientData LoadPatientData(string fileName = "patientData.json")
		{
			var filePath = ResolveFilePath(fileName);

			if (filePath == null)
			{
				throw new FileNotFoundException($"❌ Patient data file not found: {fileName}\n" +
					"Checked: working dir, Config folder, BaseDirectory");
			}

			Console.WriteLine($"📂 Loading patient data from: {filePath}");
			var json = File.ReadAllText(filePath);

			return JsonConvert.DeserializeObject<PatientData>(json)
				?? throw new Exception("❌ Failed to parse patientData.json");
		}

		public static PatientData LoadPatientDataWithOffset(int offset = 1, string fileName = "patientData.json")
		{
			var baseData = LoadPatientData(fileName);

			// ✅ Load existing records
			var existingRecords = LoadExistingRecords();

			PatientData newData = null!;
			int attempt = 0;

			// Try different offsets until we find a unique record
			do
			{
				int currentOffset = offset + attempt;
				newData = new PatientData
				{
					FirstName = IncrementLastDigit(baseData.FirstName, currentOffset),
					MiddleName = baseData.MiddleName,
					LastName = baseData.LastName,
					Gender = baseData.Gender,
					DateOfBirth = IncrementDate(baseData.DateOfBirth, currentOffset),
					PhoneCode = baseData.PhoneCode,
					PhoneNumber = IncrementPhoneNumberLastDigit(baseData.PhoneNumber, currentOffset)
				};

				attempt++;
			}
			while (IsDuplicate(existingRecords, newData) && attempt < 50);

			if (IsDuplicate(existingRecords, newData))
				throw new Exception("❌ Could not generate a unique patient after multiple attempts.");

			AppendGeneratedData(newData);
			return newData;
		}

		private static bool IsDuplicate(List<PatientData> records, PatientData data)
		{
			return records.Any(r =>
				r.FirstName == data.FirstName &&
				r.LastName == data.LastName &&
				r.PhoneNumber == data.PhoneNumber &&
				r.DateOfBirth == data.DateOfBirth); // Added DateOfBirth to the duplicate check
		}

		private static List<PatientData> LoadExistingRecords()
		{
			if (!File.Exists(GeneratedFile)) return new List<PatientData>();

			var json = File.ReadAllText(GeneratedFile);
			if (string.IsNullOrWhiteSpace(json)) return new List<PatientData>();

			try
			{
				return JsonConvert.DeserializeObject<List<PatientData>>(json) ?? new List<PatientData>();
			}
			catch
			{
				return new List<PatientData>();
			}
		}

		private static string IncrementLastDigit(string value, int offset)
		{
			if (string.IsNullOrEmpty(value)) return value;

			for (int i = value.Length - 1; i >= 0; i--)
			{
				if (char.IsDigit(value[i]))
				{
					var d = value[i] - '0';
					var newD = (d + offset) % 10;
					return value.Substring(0, i) + (char)('0' + newD) + value[(i + 1)..];
				}
			}
			return value + offset.ToString(); // Append the full offset if no digit is found
		}

		private static string IncrementPhoneNumberLastDigit(string phone, int offset)
		{
			if (string.IsNullOrEmpty(phone)) return phone;

			for (int i = phone.Length - 1; i >= 0; i--)
			{
				if (char.IsDigit(phone[i]))
				{
					var d = phone[i] - '0';
					var newD = (d + offset) % 10;
					return phone.Substring(0, i) + (char)('0' + newD) + phone[(i + 1)..];
				}
			}
			return phone + offset.ToString(); // Append the full offset if no digit is found
		}

		private static string IncrementDate(string dob, int offset)
		{
			if (string.IsNullOrWhiteSpace(dob)) return dob;

			var formats = new[] { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };
			foreach (var fmt in formats)
			{
				if (DateTime.TryParseExact(dob, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
					return parsed.AddDays(offset).ToString(fmt, CultureInfo.InvariantCulture);
			}
			return dob;
		}

		private static void AppendGeneratedData(PatientData data)
		{
			var directory = Path.GetDirectoryName(GeneratedFile);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory!);

			var records = LoadExistingRecords();

			if (!IsDuplicate(records, data))
			{
				records.Add(data);
				File.WriteAllText(GeneratedFile, JsonConvert.SerializeObject(records, Formatting.Indented));
				Console.WriteLine($"✅ Appended unique record to: {GeneratedFile}");
			}
			else
			{
				Console.WriteLine($"⚠️ Skipped duplicate record: {data.FirstName} {data.LastName} ({data.PhoneNumber})");
			}
		}
	}
}

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace HyperspectralAutomation.Pages
{
	public class PatientRegPage
	{
		private readonly AndroidDriver driver;
		private readonly WebDriverWait wait;

		public PatientRegPage(AndroidDriver driver, WebDriverWait wait)
		{
			this.driver = driver;
			this.wait = wait;
		}

		// ---------- Locators ----------

		private By FirstName =>
			MobileBy.AccessibilityId("first-name_com.google.android.material.textfield.TextInputEditText");

		private By MiddleName =>
			MobileBy.AccessibilityId("middle-name_com.google.android.material.textfield.TextInputEditText");

		private By LastName =>
			MobileBy.AccessibilityId("last-name_com.google.android.material.textfield.TextInputEditText");

		private By GenderDropdown =>
			By.Id("com.iprd.anc.nigeriaoyo:id/text_input_end_icon");

		private By NextBtn =>
			By.Id("com.iprd.anc.nigeriaoyo:id/pagination_next_button");

		private By DateOfBirth =>
			MobileBy.AccessibilityId("date-of-birthcom.google.android.material.textfield.TextInputEditText");
		// ---------- Locators ----------
		private By PhoneCodeDrop =>
			MobileBy.AccessibilityId("Show dropdown menu"); // clean version
		private By PhoneNo =>
			MobileBy.AccessibilityId("mobile-number_com.google.android.material.textfield.TextInputEditText");
		private By OclId =>
			MobileBy.AccessibilityId("ocl-id_com.google.android.material.button.MaterialButton");

		private By GenOclId =>
			By.Id("com.iprd.anc.nigeriaoyo:id/btnGenerate");

		private By FinishOclId =>
			By.Id("com.iprd.anc.nigeriaoyo:id/btnFinish");

        private By SubmitBtn =>
            By.Id("com.iprd.anc.nigeriaoyo:id / submit_questionnaire");

        private By EditBtn =>
            By.Id("com.iprd.anc.nigeriaoyo:id/review_mode_edit_button");

        // ---------- Actions ----------
        public void SelectPhoneCode(string phoneCode)
		{
			Console.WriteLine($"[INFO] Selecting phone code: {phoneCode}");

				// Open the dropdown first
				var dropdown = wait.Until(d => d.FindElement(PhoneCodeDrop));
				dropdown.Click();
				Thread.Sleep(500); // wait for dropdown items to appear

				// 2️⃣ Prepare pointer and coordinates for the tap
				var finger = new PointerInputDevice(PointerKind.Touch);
				Point tapPoint;

				switch (phoneCode)
				{
					case "+234":
						tapPoint = new Point(137, 1255);
						break;
					case "+91":
						tapPoint = new Point(119, 1379);
						break;
					case "+1":
						tapPoint = new Point(115, 1521);
						break;
					case "+27":
						tapPoint = new Point(124, 1658);
						break;
					default:
						throw new ArgumentException($"Invalid phone code '{phoneCode}'. Supported codes: +234, +91, +1, +27");
				}

				// 3️⃣ Build and perform the tap gesture using the pointer API
				var tap = new ActionSequence(finger);
				tap.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, tapPoint.X, tapPoint.Y, TimeSpan.Zero));
				// pointer down/up for touch; MouseButton.Left is accepted by the API for pointer down/up
				tap.AddAction(finger.CreatePointerDown(MouseButton.Left));
				// small pause to simulate a real tap
				tap.AddAction(finger.CreatePause(TimeSpan.FromMilliseconds(50)));
				tap.AddAction(finger.CreatePointerUp(MouseButton.Left));

				// Perform the action
				driver.PerformActions(new List<ActionSequence> { tap });

				Console.WriteLine($"[INFO] Phone code '{phoneCode}' selected successfully at ({tapPoint.X}, {tapPoint.Y}).");
			}


		// ---------- Actions ----------

		public void EnterFirstName(string firstName)
		{
			var element = wait.Until(d => d.FindElement(FirstName));
			element.Clear();
			element.SendKeys(firstName);
		}

		public void EnterMiddleName(string middleName)
		{
			var element = wait.Until(d => d.FindElement(MiddleName));
			element.Clear();
			element.SendKeys(middleName);
		}

		public void EnterLastName(string lastName)
		{
			var element = wait.Until(d => d.FindElement(LastName));
			element.Clear();
			element.SendKeys(lastName);
		}

        public void SelectGender(string gender)
        {
            Console.WriteLine($"[INFO] Selecting gender: {gender}");

            // Open the dropdown first
            var dropdown = wait.Until(d => d.FindElement(GenderDropdown));
            dropdown.Click();
            Thread.Sleep(500);

            gender = gender?.Trim().ToLower();

            void TapAt(int x, int y)
            {
                var finger = new PointerInputDevice(PointerKind.Touch);
                var tap = new ActionSequence(finger);
                tap.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.Zero));
                tap.AddAction(finger.CreatePointerDown(MouseButton.Left));
                tap.AddAction(finger.CreatePause(TimeSpan.FromMilliseconds(50))); // ✅ correct pause method
                tap.AddAction(finger.CreatePointerUp(MouseButton.Left));
                driver.PerformActions(new List<ActionSequence> { tap });
                Thread.Sleep(500);
            }

            try
            {
                if (gender == "male")
                {
                    // Try both coordinates for male
                    TapAt(139, 1545);
					SelectGender("gender"); // reopen dropdown
                    TapAt(139, 2119);
                    Console.WriteLine("[INFO] Male gender tapped successfully.");
                }
                else if (gender == "female")
                {
                    // Try both coordinates for female
                    TapAt(139, 1684);
                    SelectGender("gender"); // reopen dropdown

                    TapAt(157, 2257);
                    Console.WriteLine("[INFO] Female gender tapped successfully.");
                }
                else
                {
                    throw new ArgumentException($"Invalid gender value '{gender}'. Expected 'Male' or 'Female'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to tap gender: {ex.Message}");
            }
        }


        public void ClickNext1()
		{
			var next = wait.Until(d => d.FindElement(NextBtn));
			next.Click();
		}

		public void EnterDateOfBirth(string dob)
		{
			var dateField = wait.Until(d => d.FindElement(DateOfBirth));
			dateField.Clear();
			dateField.SendKeys(dob);
		}

		public void EnterPhoneNumber(string phoneNumber)
		{
			var phoneField = wait.Until(d => d.FindElement(PhoneNo));
			phoneField.Clear();
			phoneField.SendKeys(phoneNumber);
		}


		public void ClickNext2()
		{
			var next = wait.Until(d => d.FindElement(NextBtn));
			next.Click();
		}

		public void ClickOclId()
		{
			var next = wait.Until(d => d.FindElement(OclId));
			next.Click();
		}

		public void ClickGenOclId()
		{
			var generateBtn = wait.Until(d => d.FindElement(GenOclId));
			generateBtn.Click();
		}

		public void ClickFinishOclId()
		{
			var finishBtn = wait.Until(d => d.FindElement(FinishOclId));
			finishBtn.Click();
		}

		public void ClickNext3()
		{
			var next = wait.Until(d => d.FindElement(NextBtn));
			next.Click();
		}

		public void ClickEditBtn()
		{
			var editBtn = wait.Until(d => d.FindElement(EditBtn));
			editBtn.Click();
        }

        public void ClickSubmitBtn()
		{
			var submitBtn = wait.Until(d => d.FindElement(SubmitBtn));
			submitBtn.Click();
        }

        public void FillBasicDetails(string firstName, string middleName, string lastName, string gender, string dob, string phoneCode,string phoneNumber)
		{
			EnterFirstName(firstName);
			EnterMiddleName(middleName);
			EnterLastName(lastName);
			SelectGender(gender);
			ClickNext1();
			EnterDateOfBirth(dob);
			SelectPhoneCode(phoneCode);
			EnterPhoneNumber(phoneNumber);
			ClickNext2();
			ClickOclId();
			ClickGenOclId();
			ClickFinishOclId();
			ClickNext3();//reviewBtn
			//ClickEditBtn();
            //ClickSubmitBtn();
        }
	}
}

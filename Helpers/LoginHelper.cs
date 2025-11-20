using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace HyperspectralAutomation.Helpers
{
    public static class LoginHelper
    {
        public static void PerformLogin(AndroidDriver driver, WebDriverWait wait)
        {
            Console.WriteLine("---- LOGIN FLOW START ----");

            try
            {
                // Step 1: Click login button
                var loginBtn = wait.Until(d => d.FindElement(By.Id("com.iprd.anc.nigeriaoyo:id/login_button")));
                loginBtn.Click();
                Console.WriteLine("Login button tapped");

                // Step 2: Check if facility popup appears right away
                if (TryHandleFacilityPopup(driver, wait))
                {
                    Console.WriteLine("Directly navigated — login already active");
                    return;
                }

                // Step 3: Wait for WebView to appear
                wait.Timeout = TimeSpan.FromSeconds(10);
                Thread.Sleep(TimeSpan.FromSeconds(3)); // small delay for WebView load

                var webviewContext = driver.Contexts.FirstOrDefault(c => c.Contains("WEBVIEW"));
                if (webviewContext == null)
                {
                    Console.WriteLine("No WebView detected — might already be logged in");
                    return;
                }

                driver.Context = webviewContext;
                Console.WriteLine($"Switched to WebView: {webviewContext}");

                // Step 4: Try finding username field
                var usernameField = driver.FindElements(By.Id("username")).FirstOrDefault();
                var passwordField = driver.FindElements(By.Id("password")).FirstOrDefault();

                if (usernameField == null || passwordField == null)
                {
                    Console.WriteLine("⚠️ Login form not found — possible auto-login or WebView error");
                }
                else
                {
                    // Step 5: Fill login creds
                    var username = Environment.GetEnvironmentVariable("TEST_LOGIN_USERNAME");
                    var password = Environment.GetEnvironmentVariable("TEST_LOGIN_PASSWORD");

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("❌ Missing login credentials. Please set TEST_LOGIN_USERNAME and TEST_LOGIN_PASSWORD environment variables.");
                        return;
                    }

                    usernameField.SendKeys(username);
                    passwordField.SendKeys(password);
                    driver.FindElement(By.Id("kc-login")).Click();
                    Console.WriteLine("Login submitted — waiting for response...");

                    // Step 6: Detect login error
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    var errorMsg = driver.FindElements(By.XPath("//*[contains(text(), 'Invalid username or password')]")).FirstOrDefault();
                    if (errorMsg != null)
                    {
                        Console.WriteLine("❌ Login failed: Invalid username or password");
                        throw new Exception("Login failed: Invalid username or password"); // Make the test fail by throwing an exception
                    }

                    // Step 7: Detect login success
                    var successIndicator = driver.FindElements(By.Id("com.iprd.anc.nigeriaoyo:id/success_indicator")).FirstOrDefault();
                    if (successIndicator == null || !successIndicator.Displayed)
                    {
                        throw new Exception("Login failed: Success indicator not found.");
                    }
                    Console.WriteLine("✅ Login successful.");
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"❌ Element not found during login: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"⚠️ WebView switch failed: {ex.Message}");
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine($"⚙️ WebDriver error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Unexpected error in login flow: {ex.Message}");
                throw;  // <-- Important: rethrow so the test fails
            }

            finally
            {
                // Step 8: Switch back to native
                try
                {
                    driver.Context = "NATIVE_APP";
                }
                catch
                {
                    Console.WriteLine("⚠️ Could not switch back to NATIVE_APP — may already be in native context.");
                }

                // Step 9: Handle post-login facility popup if any
                wait.Timeout = TimeSpan.FromSeconds(7);
                TryHandleFacilityPopup(driver, wait);

                Thread.Sleep(TimeSpan.FromSeconds(5));
                Console.WriteLine("---- LOGIN FLOW DONE ----");
            }
        }

        private static bool TryHandleFacilityPopup(AndroidDriver driver, WebDriverWait wait)
        {
            try
            {
                var yesBtn = wait.Until(d =>
                {
                    try
                    {
                        var y = d.FindElement(By.Id("android:id/button1"));
                        return y.Displayed ? y : null;
                    }
                    catch { return null; }
                });

                yesBtn.Click();
                Console.WriteLine("✅ Facility popup accepted");
                return true;
            }
            catch
            {
                Console.WriteLine("ℹ️ No Facility popup detected");
                return false;
            }
        }
    }
}

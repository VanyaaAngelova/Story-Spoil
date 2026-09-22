
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;

namespace StorySpoil.Exam
{
    public class Tests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private const string BaseUrl = "http://144.91.123.158:100";


        private const string Username = "VanyaTest";
        private const string Password = "vanya123";


        private string lastCreatedTitle;
        private string lastCreatedDescription;
        private string editedTitle;




        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);

            wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(10)
            );

            driver.Navigate().GoToUrl(BaseUrl);


            driver.FindElement(
                By.CssSelector("a[href='/User/Login']")
            ).Click();


            wait.Until(d =>
                d.FindElement(By.Id("username"))
            ).SendKeys(Username);


            driver.FindElement(
                By.Id("password")
            ).SendKeys(Password);


            driver.FindElement(
                By.CssSelector("button[type='submit']")
            ).Click();


            wait.Until(d => d.Url == BaseUrl + "/");
        }




        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }


        [Test, Order(1)]
        public void CreateStorySpoilerWithInvalidDataTest()
        {
            driver.Navigate().GoToUrl(
                BaseUrl + "/Story/Add"
            );

            // Leave both fields empty
            driver.FindElement(By.Id("title")).Clear();
            driver.FindElement(By.Id("description")).Clear();

            // Submit the form
            driver.FindElement(
                By.CssSelector("button[type='submit']")
            ).Click();

            // Verify that we remain on the Create Spoiler page
            Assert.That(
                driver.Url,
                Does.Contain("/Story/Add"),
                "The application should remain on the Create Spoiler page after invalid submission."
            );

            // Find the validation message for the title
            IWebElement titleValidation =
                wait.Until(d =>
                    d.FindElement(
                        By.CssSelector(
                            "[data-valmsg-for='Title']"
                        )
                    )
                );

            // Find the validation message for the description
            IWebElement descriptionValidation =
                wait.Until(d =>
                    d.FindElement(
                        By.CssSelector(
                            "[data-valmsg-for='Description']"
                        )
                    )
                );

            // Verify title validation
            Assert.That(
                titleValidation.Text,
                Is.EqualTo("The Title field is required.")
            );

            // Verify description validation
            Assert.That(
                descriptionValidation.Text,
                Is.EqualTo("The Description field is required.")
            );
        }


        [Test, Order(2)]
        public void CreateRandomStorySpoilerTest()
        {

            driver.Navigate().GoToUrl(
                BaseUrl + "/Story/Add"
            );


            lastCreatedTitle =
                "Test Spoiler " +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8);


            lastCreatedDescription =
                "Test Description " +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8);


            driver.FindElement(
                By.Id("title")
            ).SendKeys(lastCreatedTitle);


            driver.FindElement(
                By.Id("description")
            ).SendKeys(lastCreatedDescription);


            driver.FindElement(
                By.CssSelector("button[type='submit']")
            ).Click();


            wait.Until(d => d.Url == BaseUrl + "/");


            Assert.That(
                driver.Url,
                Is.EqualTo(BaseUrl + "/"),
                "The application did not navigate to the Home page."
            );


            IWebElement createdSpoiler =
                wait.Until(d =>
                    d.FindElement(
                        By.XPath(
                            $"//h2[normalize-space()='{lastCreatedTitle}']"
                        )
                    )
                );


            Assert.That(
                createdSpoiler.Text,
                Is.EqualTo(lastCreatedTitle),
                "The displayed story spoiler title does not match the created title."
            );
        }




        [Test, Order(3)]
        public void EditLastCreatedStorySpoilerTitleTest()
        {

            driver.Navigate().GoToUrl(BaseUrl + "/");


            var spoilers =
                driver.FindElements(
                    By.CssSelector(".col-lg-6.order-lg-1")
                );


            Assert.That(
                spoilers.Count,
                Is.GreaterThan(0),
                "No story spoilers were found on the Home page."
            );


            IWebElement lastSpoiler =
                spoilers[spoilers.Count - 1];


            IWebElement editButton =
                lastSpoiler.FindElement(
                    By.CssSelector(
                        "a[href*='/Story/Edit']"
                    )
                );


            Actions actions =
                new Actions(driver);

            actions
                .MoveToElement(editButton)
                .ScrollToElement(editButton)
                .Click()
                .Perform();


            wait.Until(d =>
                d.Url.Contains("/Story/Edit")
            );


            editedTitle =
                "Edited Spoiler " +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8);


            IWebElement titleInput =
                driver.FindElement(
                    By.Id("title")
                );


            titleInput.Clear();
            titleInput.SendKeys(editedTitle);


            driver.FindElement(
                By.CssSelector("button[type='submit']")
            ).Click();


            wait.Until(d => d.Url == BaseUrl + "/");


            Assert.That(
                driver.Url,
                Is.EqualTo(BaseUrl + "/"),
                "The application did not return to the Home page after editing."
            );


            IWebElement editedSpoiler =
                wait.Until(d =>
                    d.FindElement(
                        By.XPath(
                            $"//h2[normalize-space()='{editedTitle}']"
                        )
                    )
                );


            Assert.That(
                editedSpoiler.Text,
                Is.EqualTo(editedTitle),
                "The edited story spoiler title was not displayed correctly."
            );
        }



        [Test, Order(4)]
        public void DeleteLastCreatedStorySpoilerTest()
        {
            driver.Navigate().GoToUrl(
                BaseUrl + "/"
            );


            IWebElement spoilerTitle =
                wait.Until(d =>
                    d.FindElement(
                        By.XPath(
                            $"//h2[normalize-space()='{editedTitle}']"
                        )
                    )
                );


            IWebElement spoilerCard =
                spoilerTitle.FindElement(
                    By.XPath(
                        "./ancestor::div[contains(@class,'col-lg-6')]"
                    )
                );


            IWebElement deleteButton =
                spoilerCard.FindElement(
                    By.CssSelector(
                        "a[href*='/Story/Delete']"
                    )
                );


            string deleteUrl =
                deleteButton.GetAttribute("href");

            Assert.That(
                deleteUrl,
                Does.Contain("/Story/Delete"),
                "The Delete link was not found."
            );


            driver.Navigate().GoToUrl(deleteUrl);


            wait.Until(d =>
                d.Url == BaseUrl + "/"
            );


            wait.Until(d =>
                d.FindElements(
                    By.XPath(
                        $"//h2[normalize-space()='{editedTitle}']"
                    )
                ).Count == 0
            );

            Assert.That(
                driver.FindElements(
                    By.XPath(
                        $"//h2[normalize-space()='{editedTitle}']"
                    )
                ).Count,
                Is.EqualTo(0),
                "The edited story spoiler was not deleted."
            );
        }


        [Test, Order(5)]
        public void TryToEditNonExistentStorySpoilerTest()
        {

            string invalidId =
                "00000000-0000-0000-0000-000000000000";


            driver.Navigate().GoToUrl(
                BaseUrl +
                "/Story/Edit?storyId=" +
                invalidId
            );


            IWebElement errorMessage =
                wait.Until(d =>
                    d.FindElement(
                        By.XPath(
                            "//*[contains(normalize-space(.), 'No such spoiler!')]"
                        )
                    )
                );


            Assert.That(
                errorMessage.Text,
                Does.Contain("No such spoiler!"),
                "The expected 'No such spoiler!' message was not displayed."
            );
        }




        [Test, Order(6)]
        public void TryToDeleteNonExistentStorySpoilerTest()
        {

            string invalidId =
                "00000000-0000-0000-0000-000000000000";


            driver.Navigate().GoToUrl(
                BaseUrl +
                "/Story/Delete?storyId=" +
                invalidId
            );


            IWebElement errorMessage =
                wait.Until(d =>
                    d.FindElement(
                        By.XPath(
                            "//*[contains(normalize-space(.), 'No such spoiler!')]"
                        )
                    )
                );


            Assert.That(
                errorMessage.Text,
                Does.Contain("No such spoiler!"),
                "The expected 'No such spoiler!' message was not displayed."
            );
        }
    }
}



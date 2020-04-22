using NUnit.Framework;
using VerificationCheck.ViewModels.Authorization;

namespace VerificationCheckTests.ViewModels.FNS
{
    class SignUpViewModelTests
    {
        SignUpViewModel signInViewModel;

        [SetUp]
        public void Setup()
        {
            signInViewModel = new SignUpViewModel(new VerificationCheck.Core.FNS());
        }

        [TestCase("11nature98@gmail.com", "Danil`", "+79000630379", TestName = "UserExists", ExpectedResult = false)]
        [TestCase(null, "Danil`", "+79000630379", TestName = "NullEmail", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", null, "+79000630379", TestName = "NullName", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", "Danil`", null, TestName = "NullPhone", ExpectedResult = false)]
        [TestCase("", "Danil`", "+79000630379", TestName = "EmptyEmail", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", "", "+79000630379", TestName = "EmptyName", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", "Danil`", "", TestName = "EmptyPhone", ExpectedResult = false)]
        [TestCase("      ", "Danil`", "+79000630379", TestName = "WhiteSpacesEmail", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", "    ", "+79000630379", TestName = "WhiteSpacesName", ExpectedResult = false)]
        [TestCase("11nature98@gmail.com", "Danil`", "      ", TestName = "WhiteSpacesPhone", ExpectedResult = false)]
        public bool TrySignUp(string email, string name, string phone)
        {
            signInViewModel.Email = email;
            signInViewModel.Name = name;
            signInViewModel.Phone = phone;

            return signInViewModel.TrySignUp();
        }
    }
}

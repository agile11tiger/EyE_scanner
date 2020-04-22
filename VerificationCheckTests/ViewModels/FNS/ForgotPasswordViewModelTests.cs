using NUnit.Framework;
using VerificationCheck.ViewModels.Authorization;

namespace VerificationCheckTests.ViewModels.FNS
{
    class ForgotPasswordViewModelTests
    {
        ForgotPasswordViewModel forgotPasswordViewModel;

        [SetUp]
        public void Setup()
        {
            forgotPasswordViewModel = new ForgotPasswordViewModel(new VerificationCheck.Core.FNS());
        }

        [TestCase(null, TestName = "NullPhone", ExpectedResult = false)]
        [TestCase("", TestName = "EmptyPhone", ExpectedResult = false)]
        [TestCase("     ", TestName = "WhiteSpacesPhone", ExpectedResult = false)]
        public bool TrySignIn(string phone)
        {
            forgotPasswordViewModel.Phone = phone;

            return forgotPasswordViewModel.TryRestorePassword();
        }
    }
}

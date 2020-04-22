using NUnit.Framework;
using VerificationCheck.ViewModels.Authorization;

namespace VerificationCheckTests.ViewModels.FNS
{
    class SignInViewModelTests
    {
        SignInViewModel signInViewModel;

        [SetUp]
        public void Setup()
        {
            signInViewModel = new SignInViewModel(new VerificationCheck.Core.FNS());
        }

        [TestCase("+79000630379", "233964", TestName = "GoodPhoneAndPassword", ExpectedResult = true)]
        [TestCase("+79000630379", "111111", TestName = "GoodPhoneAndBadPassword", ExpectedResult = false)]
        [TestCase("+79991234567", "233964", TestName = "BadPhoneAndGoodPassword", ExpectedResult = false)]
        [TestCase("+79991234567", "111111", TestName = "BadPhoneAndBadPassword", ExpectedResult = false)]
        [TestCase(null, "111111", TestName = "NullPhone", ExpectedResult = false)]
        [TestCase("+79000630379", null, TestName = "NullPassword", ExpectedResult = false)]
        [TestCase("", "111111", TestName = "EmptyPhone", ExpectedResult = false)]
        [TestCase("+79000630379", "", TestName = "EmptyPassword", ExpectedResult = false)]
        [TestCase("     ", "111111", TestName = "WhiteSpacesPhone", ExpectedResult = false)]
        [TestCase("+79000630379", "     ", TestName = "WhiteSpacesPassword", ExpectedResult = false)]
        public bool TrySignIn(string phone, string password)
        {
            signInViewModel.Phone = phone;
            signInViewModel.Password = password;

            return signInViewModel.TrySignIn();
        }
    }
}

using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Messages;
using Scanner.Models;
using Scanner.Views.Authorization;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerificationCheck.Core;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей регистрации пользователя в ФНС
    /// </summary>
    public class SignUpViewModel : FNSSignViewModel
    {
        public SignUpViewModel(FNS fns, Sign sign, SignInPage signInPage) : base(fns, sign)
        {
            this.signInPage = signInPage;
            SignUpCommand = new AsyncCommand(SignUp);
        }

        private readonly SignInPage signInPage;
        public IAsyncCommand SignUpCommand { get; }

        private async Task SignUp()
        {
            if (await TrySignUp())
            {
                signInPage.ViewModel.Phone = Phone;
                await Navigation.PushAsync(signInPage).ConfigureAwait(false);
            }
        }

        private async Task<bool> TrySignUp()
        {
            var phone = ParsePhone();

            if (string.IsNullOrWhiteSpace(Email)
             || string.IsNullOrWhiteSpace(Name)
             || string.IsNullOrWhiteSpace(phone))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.RegistrationAsync(Email, Name, phone);
            return await TryExecute(task);
        }
    }
}
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
        public SignUpViewModel(FNS fns, Func<Sign, Task> syncWithUserAccount, Sign sign) : base(fns, syncWithUserAccount, sign)
        {
            SignUpCommand = new AsyncCommand(signUp);
        }

        public IAsyncCommand SignUpCommand { get; set; }

        private async Task signUp()
        {
            if (await trySignUp())
            {
                var signInPage = App.Container.Get<SignInPage>();
                signInPage.ViewModel.Phone = Phone;
                await Navigation.PushAsync(signInPage).ConfigureAwait(false);
            }
        }

        private async Task<bool> trySignUp()
        {
            var phone = Regex.Replace(Phone, @"[^+\d]", "");

            if (string.IsNullOrWhiteSpace(Email)
             || string.IsNullOrWhiteSpace(Name)
             || string.IsNullOrWhiteSpace(phone))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.RegistrationAsync(Email, Name, phone);

            if (task != await Task.WhenAny(task, Task.Delay(5000)))
            {
                FailMessage = CommonMessages.NoInternet;
                return false;
            }

            if (task.Result.IsSuccess)
                return true;

            FailMessage = task.Result.Message;
            return false;
        }
    }
}
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
    /// Класс, взаимодействующий со страницей забытия пароля в ФНС
    /// </summary>
    public class ForgotPasswordViewModel : FNSSignViewModel
    {
        public ForgotPasswordViewModel(FNS fns, Sign sign, SignInPage signInPage) : base(fns, sign)
        {
            this.signInPage = signInPage;
            RestorePasswordCommand = new AsyncCommand(RestorePassword);
        }

        private readonly SignInPage signInPage;
        public IAsyncCommand RestorePasswordCommand { get; set; }

        private async Task RestorePassword()
        {
            if (await TryRestorePassword())
            {
                signInPage.ViewModel.Phone = Phone;
                await Navigation.PushAsync(signInPage).ConfigureAwait(false);
            }
        }

        private async Task<bool> TryRestorePassword()
        {
            var phone = ParsePhone();

            if (string.IsNullOrWhiteSpace(phone))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.RestorePasswordAsync(phone);
            return await TryExecute(task);
        }
    }
}

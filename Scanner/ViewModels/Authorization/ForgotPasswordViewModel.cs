using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Messages;
using Scanner.Models;
using System.Threading.Tasks;
using VerifyReceiptSDK;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей забытия пароля в ФНС
    /// </summary>
    public class ForgotPasswordViewModel : FNSSignViewModel
    {
        public ForgotPasswordViewModel(Sign sign) : base(sign)
        {
            RestorePasswordCommand = new AsyncCommand(RestorePassword);
        }

        public IAsyncCommand RestorePasswordCommand { get; set; }

        private async Task RestorePassword()
        {
            if (await TryRestorePassword())
            {
                Pages.SignInPage.ViewModel.Phone = Phone;
                await Navigation.PushAsync(Pages.SignInPage).ConfigureAwait(false);
            }
        }

        private async Task<bool> TryRestorePassword()
        {
            var phone = GetClearPhone();

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

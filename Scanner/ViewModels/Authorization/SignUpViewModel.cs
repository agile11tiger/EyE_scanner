using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Messages;
using Scanner.Models;
using System.Threading.Tasks;
using VerifyReceiptSDK;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей регистрации пользователя в ФНС
    /// </summary>
    public class SignUpViewModel : FNSSignViewModel
    {
        public SignUpViewModel(Sign sign) : base(sign)
        {
            SignUpCommand = new AsyncCommand(SignUp);
        }

        public IAsyncCommand SignUpCommand { get; }

        private async Task SignUp()
        {
            if (await TrySignUp())
            {
                Pages.SignInPage.ViewModel.Phone = Phone;
                await Navigation.PushAsync(Pages.SignInPage).ConfigureAwait(false);
            }
        }

        private async Task<bool> TrySignUp()
        {
            var phone = GetClearPhone();

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
using Newtonsoft.Json;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Messages;
using Scanner.Models;
using System;
using System.Threading.Tasks;
using VerifyReceiptSDK;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей входа пользователя в ФНС
    /// </summary>
    public class SignInViewModel : FNSSignViewModel
    {
        public SignInViewModel(Func<Sign, Task> updateUserAccount, Sign sign) : base(sign)
        {
            UpdateUserAccount = updateUserAccount;
            SignInCommand = new AsyncCommand(SignIn);
            ForgotPasswordCommand = new AsyncCommand(GoToForgotPasswordPage);
        }

        public IAsyncCommand SignInCommand { get; }
        public IAsyncCommand ForgotPasswordCommand { get; }
        public Func<Sign, Task> UpdateUserAccount { get; }

        private async Task SignIn()
        {
            if (await TrySignIn())
            {
                await Navigation.PopToRootAsync().ConfigureAwait(false);
            }
        }

        private Task GoToForgotPasswordPage()
        {
            Pages.ForgotPasswordPage.Value.ViewModel.Phone = Phone;
            return Navigation.PushAsync(Pages.ForgotPasswordPage.Value);
        }

        private async Task<bool> TrySignIn()
        {
            var phone = GetClearPhone();

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(Password))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.LoginAsync(phone, Password);

            if (await TryExecute(task))
            {
                var temp = JsonConvert.DeserializeObject<Sign>(task.Result.Message);
                Name = temp.Name;
                Email = temp.Email;
                IsAuthorization = true;

                await UpdateUserAccount(Sign);
                return true;
            }

            return false;
        }
    }
}
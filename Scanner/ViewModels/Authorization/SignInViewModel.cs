using Newtonsoft.Json;
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
    /// Класс, взаимодействующий со страницей входа пользователя в ФНС
    /// </summary>
    public class SignInViewModel : FNSSignViewModel
    {
        public SignInViewModel(
            FNS fns, 
            Func<Sign, Task> updateUserAccount, 
            Sign sign, 
            Lazy<ForgotPasswordPage> forgotPasswordPage) 
            : base(fns, sign)
        {
            UpdateUserAccount = updateUserAccount;
            this.forgotPasswordPage = forgotPasswordPage;
            SignInCommand = new AsyncCommand(SignIn);
            ForgotPasswordCommand = new AsyncCommand(GoToForgotPasswordPage);
        }

        private readonly Lazy<ForgotPasswordPage> forgotPasswordPage;
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
            forgotPasswordPage.Value.ViewModel.Phone = Phone;
            return Navigation.PushAsync(forgotPasswordPage.Value);
        }

        private async Task<bool> TrySignIn()
        {
            //var b = "{\"email\":\"11nature98@gmail.com\",\"name\":\"Danil`\"}";
            var phone = ParsePhone();

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(Password))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.LoginAsync(phone, Password);

            if (await TryExecute(task))
            {
                var temp = JsonConvert.DeserializeObject<SignViewModel>(task.Result.Message);
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
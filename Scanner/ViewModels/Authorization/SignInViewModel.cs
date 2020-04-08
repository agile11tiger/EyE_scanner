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
        public SignInViewModel(FNS fns, Func<Sign, Task> syncWithUserAccount, Sign sign) : base(fns, syncWithUserAccount, sign)
        {
            SignInCommand = new AsyncCommand(signIn);
            ForgotPasswordCommand = new AsyncCommand(goToForgotPasswordPage);
        }

        public IAsyncCommand SignInCommand { get; set; }
        public IAsyncCommand ForgotPasswordCommand { get; set; }

        private async Task signIn()
        {
            if (await trySignIn())
            {
                await Navigation.PopToRootAsync().ConfigureAwait(false);
            }
        }

        private Task goToForgotPasswordPage()
        {
            var forgotPasswordPage = App.Container.Get<ForgotPasswordPage>();
            forgotPasswordPage.ViewModel.Phone = Phone;
            return Navigation.PushAsync(forgotPasswordPage);
        }

        private async Task<bool> trySignIn()
        {
            //var b = "{\"email\":\"11nature98@gmail.com\",\"name\":\"Danil`\"}";
            var phone = Regex.Replace(Phone, @"[^+\d]", "");

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(Password))
            {
                FailMessage = CommonMessages.UnfilledFields;
                return false;
            }

            var task = FNS.LoginAsync(phone, Password);

            if (task != await Task.WhenAny(task, Task.Delay(5000)))
            {
                FailMessage = CommonMessages.NoInternet;
                return false;
            }

            if (task.Result.IsSuccess)
            {
                var temp = JsonConvert.DeserializeObject<SignViewModel>(task.Result.Message);
                Name = temp.Name;
                Email = temp.Email;
                IsAuthorization = true;

                await SyncWithUserAccount(Sign);
                return true;
            }

            FailMessage = task.Result.Message;
            return false;
        }
    }
}
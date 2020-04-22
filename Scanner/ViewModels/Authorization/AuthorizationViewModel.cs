using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Views.Authorization;
using System;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя
    /// </summary>
    public class AuthorizationViewModel : BaseViewModel
    {
        public AuthorizationViewModel(SignInPage signInPage, SignUpPage signUpPage) : base()
        {
            this.signInPage = signInPage;
            this.signUpPage = signUpPage;
            InfoCommand = new AsyncCommand(ShowInfo);
            SignInCommand = new AsyncCommand(GoToSignInPage);
            SignUpCommand = new AsyncCommand(GoToSignUpPage);
        }

        private readonly SignInPage signInPage;
        private readonly SignUpPage signUpPage;
        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand SignInCommand { get; }
        public IAsyncCommand SignUpCommand { get; }

        private Task ShowInfo()
        {
            return CurrentPage.DisplayAlert(
                "Зачем регистрироваться в ФНС?",
                "Федеральная налоговая служба (ФНС России) является " +
                "федеральным органом исполнительной власти, " +
                "осуществляющим функции по контролю и надзору " +
                "за соблюдением законодательства о налогах и сборах...\n" +
                "Если вкратце, то им отправляются все чеки.",
                "Ок");
        }

        private Task GoToSignInPage()
        {
            return Navigation.PushAsync(signInPage);
        }

        private Task GoToSignUpPage()
        {
            return Navigation.PushAsync(signUpPage);
        }
    }
}

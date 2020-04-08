using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Views.Authorization;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя
    /// </summary>
    public class AuthorizationViewModel : BaseViewModel
    {
        public AuthorizationViewModel() : base()
        {
            InfoCommand = new AsyncCommand(showInfo);
            SignInCommand = new AsyncCommand(goToSignInPage);
            SignUpCommand = new AsyncCommand(goToSignUpPage);
        }

        public IAsyncCommand InfoCommand { get; set; }
        public IAsyncCommand SignInCommand { get; set; }
        public IAsyncCommand SignUpCommand { get; set; }

        private Task showInfo()
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

        private Task goToSignInPage()
        {
            var signInPage = App.Container.Get<SignInPage>();
            return Navigation.PushAsync(signInPage);
        }

        private Task goToSignUpPage()
        {
            var signUpPage = App.Container.Get<SignUpPage>();
            return Navigation.PushAsync(signUpPage);
        }
    }
}

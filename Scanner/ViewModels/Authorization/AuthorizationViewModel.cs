using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
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
            InfoCommand = new AsyncCommand(ShowInfo);
            SignInCommand = new AsyncCommand(GoToSignInPage);
            SignUpCommand = new AsyncCommand(GoToSignUpPage);
        }

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
            return Navigation.PushAsync(Pages.SignInPage);
        }

        private Task GoToSignUpPage()
        {
            return Navigation.PushAsync(Pages.SignUpPage);
        }
    }
}

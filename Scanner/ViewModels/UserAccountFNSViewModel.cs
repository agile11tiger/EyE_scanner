using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.ViewModels.Authorization;
using Scanner.Views.Authorization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий со страницей пользователя в ФНС
    /// </summary>
    public class UserAccountFNSViewModel : SignViewModel
    {
        public UserAccountFNSViewModel(Sign sign) : base(sign)
        {
            AuthorizationCommand = new AsyncCommand<bool>(goToAuthorizationPage);
        }

        public IAsyncCommand<bool> AuthorizationCommand { get; set; }
        public string TextAuthorizationButton { get => "Авторизоваться"; }
        public ImageSource UserImage
        {
            //Сделать. Если пользователь удаляет картинку из галлереи, то нужно вернуть ему картинку по умолчанию
            //Проблема в том, что как узнать, что ImageSource == null. 
            //Если я ему даю ссылку(даже если по ней ничего нет), то он не null и не IsEmpty.
            get
            {
                if (Sign.PathToUserImage == Sign.PathUserImageDefault)
                    return ImageSource.FromResource(Sign.PathToUserImage);

                return ImageSource.FromFile(Sign.PathToUserImage);
            }
        }

        public async Task Synchronization(Sign sign)
        {
            Name = sign.Name;
            Email = sign.Email;
            Phone = sign.Phone;
            Password = sign.Password;
            IsAuthorization = sign.IsAuthorization;

            AsyncDatabase.AddItemAsync(Sign);
        }

        private async Task goToAuthorizationPage(bool isMessageToContinue)
        {
            var isContinue = true;

            if (isMessageToContinue)
            {
                //CurrentPage устанавливается в ChecksWaitingPage
                isContinue = await CurrentPage.DisplayAlert(
                        "Уважаемый пользователь!",
                        "Для продолжения вам необходимо авторизоваться в ФНС, " +
                        "и тогда вам станет доступна возможность по получению кассовых чеков.\r\n" +
                        "Что такое ФНС вы можете прочитать на следующей странице нажав на знак вопроса.",
                        "Продолжить",
                        "Отмена");
            }

            if (isContinue)
            {
                var authorizationPage = App.Container.Get<AuthorizationPage>();
                await Navigation.PushAsync(authorizationPage);
                Shell.Current.FlyoutIsPresented = false;
            }
        }
    }
}

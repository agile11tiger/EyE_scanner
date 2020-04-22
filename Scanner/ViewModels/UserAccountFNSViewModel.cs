using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.ViewModels.Authorization;
using Scanner.Views.Authorization;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий со страницей пользователя в ФНС
    /// </summary>
    public class UserAccountFNSViewModel : SignViewModel
    {
        public UserAccountFNSViewModel(Sign sign, Lazy<AuthorizationPage> authorizationPage) : base(sign)
        {
            this.authorizationPage = authorizationPage;
            AuthorizationCommand = new AsyncCommand<bool>(GoToAuthorizationPage);
        }

        private readonly Lazy<AuthorizationPage> authorizationPage;
        public IAsyncCommand<bool> AuthorizationCommand { get; }
        public ImageSource UserImage
        {
            //TODO: Если пользователь удаляет картинку из галлереи, то нужно вернуть ему картинку по умолчанию
            //Проблема в том, что как узнать, что у него нету картинки в галлереи. 
            //Если я ему даю ссылку(даже если по ней ничего нет), то он не null и не IsEmpty.
            get
            {
                if (Sign.PathToUserImage == Sign.PathUserImageDefault)
                    return ImageSource.FromResource(Sign.PathToUserImage);

                return ImageSource.FromFile(Sign.PathToUserImage);
            }
        }

        public async Task Update(Sign sign)
        {
            Name = sign.Name;
            Email = sign.Email;
            Phone = sign.Phone;
            Password = sign.Password;
            IsAuthorization = sign.IsAuthorization;

            await AsyncDatabase.AddOrReplaceItemAsync(Sign);
        }

        private async Task GoToAuthorizationPage(bool isMessageToContinue)
        {
            var isContinue = true;

            if (isMessageToContinue)
            {
                //CurrentPage устанавливается в WaitingChecksPage
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
                await Navigation.PushAsync(authorizationPage.Value);
                Shell.Current.FlyoutIsPresented = false;
            }
        }
    }
}

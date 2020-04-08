using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Services;
using Scanner.Views.Scanner;
using Scanner.Views.Scanner.Friends;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий с оболочкой приложения
    /// </summary>
    public class AppShellViewModel : BaseViewModel
    {
        public AppShellViewModel(UserAccountFNSViewModel userAccount, ImageHelper imageHelper) : base()
        {
            UserAccount = userAccount;
            this.imageHelper = imageHelper;

            ToFriendsCommand = new AsyncCommand(toFriends);
            ToCodeGenerationCommand = new AsyncCommand(toCodeGeneration);
            SetUserImageCommand = new AsyncCommand(setUserImage);
            ToSettingsCommand = new AsyncCommand(toSettings);
        }

        private ImageHelper imageHelper;
        public UserAccountFNSViewModel UserAccount { get; set; }
        public IAsyncCommand ToFriendsCommand { get; set; }
        public IAsyncCommand ToCodeGenerationCommand { get; set; }
        public IAsyncCommand SetUserImageCommand { get; set; }
        public IAsyncCommand ToSettingsCommand { get; set; }

        private async Task toFriends()
        {
            var friendsPage = App.Container.Get<FriendsPage>();
            await Navigation.PushAsync(friendsPage);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async Task toCodeGeneration()
        {
            var codeGenerationPage = App.Container.Get<CodeGenerationPage>();
            await Navigation.PushAsync(codeGenerationPage);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async Task setUserImage()
        {
            var path = await imageHelper.GetImagePathFromGalleryAsync();

            if (path != null)
            {
                UserAccount.Sign.PathToUserImage = path;
                OnPropertyChanged(nameof(UserAccount.UserImage));
                await AsyncDatabase.AddItemAsync(UserAccount.Sign);
            }
        }

        private async Task toSettings()
        {
            var settingsPage = App.Container.Get<ScannerSettingsPage>();
            await Navigation.PushAsync(settingsPage);
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}

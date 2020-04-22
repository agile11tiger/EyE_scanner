using Ninject;
using Plugin.Media;
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
        public AppShellViewModel(
            UserAccountFNSViewModel userAccount, 
            ImageHelper imageHelper,
            FriendsPage friendsPage,
            CodeGenerationPage codeGenerationPage,
            ScannerSettingsPage scannerSettingsPage) : base()
        {
            UserAccount = userAccount;
            this.imageHelper = imageHelper;
            this.friendsPage = friendsPage;
            this.codeGenerationPage = codeGenerationPage;
            this.scannerSettingsPage = scannerSettingsPage;

            ToFriendsCommand = new AsyncCommand(ToFriends);
            ToCodeGenerationCommand = new AsyncCommand(ToCodeGeneration);
            SetUserImageCommand = new AsyncCommand(SetUserImage);
            ToSettingsCommand = new AsyncCommand(ToSettings);
        }

        private readonly ImageHelper imageHelper;
        private readonly FriendsPage friendsPage;
        private readonly CodeGenerationPage codeGenerationPage;
        private readonly ScannerSettingsPage scannerSettingsPage;

        public UserAccountFNSViewModel UserAccount { get; }
        public IAsyncCommand ToFriendsCommand { get; }
        public IAsyncCommand ToCodeGenerationCommand { get; }
        public IAsyncCommand SetUserImageCommand { get; }
        public IAsyncCommand ToSettingsCommand { get; }

        private async Task ToFriends()
        {
            await Navigation.PushAsync(friendsPage);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async Task ToCodeGeneration()
        {
            await Navigation.PushAsync(codeGenerationPage);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async Task SetUserImage()
        {
            var path = await imageHelper.GetImagePathFromGalleryAsync(CrossMedia.Current);

            if (path != null)
            {
                UserAccount.Sign.PathToUserImage = path;
                OnPropertyChanged(nameof(UserAccount.UserImage));
                await AsyncDatabase.AddOrReplaceItemAsync(UserAccount.Sign);
            }
        }

        private async Task ToSettings()
        {
            await Navigation.PushAsync(scannerSettingsPage);
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}

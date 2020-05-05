using Plugin.Media;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Services;
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
            FriendsPage friendsPage) : base()
        {
            UserAccount = userAccount;
            this.imageHelper = imageHelper;
            this.friendsPage = friendsPage;

            ToFriendsCommand = new AsyncCommand(ToFriends);
            ToCodeGenerationCommand = new AsyncCommand(ToCodeGeneration);
            SetUserImageCommand = new AsyncCommand(SetUserImage);
            ToSettingsCommand = new AsyncCommand(ToSettings);
        }

        private readonly ImageHelper imageHelper;
        private readonly FriendsPage friendsPage;

        public UserAccountFNSViewModel UserAccount { get; }
        public ImageSource UserImage { get => UserAccount.UserImage; }
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
            await Navigation.PushAsync(Pages.CodeGenerationPage);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async Task SetUserImage()
        {
            var path = await imageHelper.GetImagePathFromGalleryAsync(CrossMedia.Current);

            if (path != null)
            {
                UserAccount.Sign.PathToUserImage = path;
                OnPropertyChanged(nameof(UserImage));
                await AsyncDatabase.AddOrReplaceItemAsync(UserAccount.Sign);
            }
        }

        private async Task ToSettings()
        {
            await Navigation.PushAsync(Pages.ScannerSettingsPage);
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}

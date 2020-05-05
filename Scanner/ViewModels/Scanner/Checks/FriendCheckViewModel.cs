using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.ViewModels.Scanner.Friends;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    public class FriendCheckViewModel : CheckViewModel, IEquatable<FriendCheckViewModel>
    {
        public FriendCheckViewModel(FriendViewModel friendVM, Check check) : base(check)
        {
            FriendVM = friendVM;
            InfoCommand = new AsyncCommand<Page>(ShowInfo);
            SyncCheckItemsWithItems();
        }

        public FriendViewModel FriendVM { get; }
        public string TitlePage { get => FriendVM?.Name; }
        public ImageSource IconPage { get => FriendVM?.Image; }

        public bool IsCommonCheck
        {
            get
            {
                if (TitlePage == PageTitles.COMMON_CHECK)
                    return true;

                return false;
            }
        }
        public bool NoIsCommonCheck
        {
            get => !IsCommonCheck;
        }

        public IAsyncCommand<Page> InfoCommand { get; private set; }

        public bool Equals(FriendCheckViewModel other)
        {
            if (FriendVM?.Id == other.FriendVM.Id)
                return true;

            return false;
        }

        private async Task ShowInfo(Page currentPage)
        {
            if (TitlePage == PageTitles.COMMON_CHECK)
            {
                await currentPage.DisplayAlert(
                        "Как работают кнопки?",
                        "1) Кнопка \"Создать чек\" — создает новый чек для выбранного друга из отмеченных товаров.\r\n" +
                        "2) Кнопка \"В мои чеки\" — добавляет отмеченные товары в мои чеки",
                        "Ок");
            }
        }
    }
}

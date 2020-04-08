using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.ViewModels.Scanner.Friends;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    public class FriendCheckViewModel : BaseViewModel
    {
        public FriendCheckViewModel(FriendViewModel friendVM, ObservableCollection<CheckItemViewModel> items)
        {
            FriendVM = friendVM;
            Items = items;
            setCommands();
        }

        public FriendCheckViewModel()
        {
            FriendVM = new FriendViewModel();
            Items = new ObservableCollection<CheckItemViewModel>();
            setCommands();
        }

        private void setCommands()
        {
            InfoCommand = new AsyncCommand(showInfo);
            RemoveCommand = new AsyncCommand<CheckItemViewModel>(remove);
        }

        private ObservableCollection<CheckItemViewModel> items;
        public FriendViewModel FriendVM { get; set; }
        public ObservableCollection<CheckItemViewModel> Items 
        {
            get => items;
            set
            {
                if(items != value)
                {
                    items = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TitlePage { get => FriendVM.Name; }
        public ImageSource IconPage { get => FriendVM.Image; }

        public int CheckAmount
        {
            get
            {
                if (Items.Count > 0)
                    return Items.Sum(i => i.Sum);

                return 0;
            }
        }

        public bool IsCommonCheck
        {
            get
            {
                if (TitlePage == AppConstants.NAME_PAGE_COMMON_CHECK)
                    return true;

                return false;
            }
        }

        public bool NoIsCommonCheck
        {
            get => !IsCommonCheck;
        }

        public IAsyncCommand InfoCommand { get; set; }
        public IAsyncCommand<CheckItemViewModel> RemoveCommand { get; set; }

        private async Task remove(CheckItemViewModel checkItemVM)
        {
            Items.Remove(checkItemVM);
        }

        private async Task showInfo()
        {
            if(TitlePage == AppConstants.NAME_PAGE_COMMON_CHECK)
            {
                await CurrentPage.DisplayAlert(
                        "Как работают кнопки?",
                        "1) Кнопка \"Создать чек\" — создает новый чек для выбранного друга из отмеченных товаров " +
                        "и вычитает выбранное вами количество для товара из общего чека\r\n" +
                        "2) Кнопка \"В мои чеки\" — добавляет все товары(неважно отмеченные или нет) в мои чеки",
                        "Ок");
            }
        }
    }
}

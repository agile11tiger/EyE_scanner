using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner.Friends
{
    public class FriendsViewModel : ListViewModel<FriendViewModel>
    {
        public FriendsViewModel()
        {
            InfoCommand = new AsyncCommand(showInfo);
            SearchCommand = new AsyncCommand<string>(search);
            ItemSelectedCommand = new AsyncCommand<FriendViewModel>(processItemSelected);
        }

        private bool isFriendForCheck;
        public Action<FriendViewModel> ProcessFriendSelected { get; set; }
        public bool IsFriendForCheck
        {
            get => isFriendForCheck;
            set
            {
                if (isFriendForCheck != value)
                {
                    isFriendForCheck = value;
                    OnPropertyChanged();
                }
            }
        }

        public IAsyncCommand InfoCommand { get; set; }
        public IAsyncCommand<string> SearchCommand { get; set; }
        public IAsyncCommand<FriendViewModel> ItemSelectedCommand { get; set; }
        private Task showInfo()
        {
            //TODO: Добавить описание к странице друзей
            return CurrentPage.DisplayAlert(
                    "",
                    "",
                    "Ок");
        }

        private async Task processItemSelected(FriendViewModel friendVM)
        {
            if (IsFriendForCheck)
            {
                ProcessFriendSelected(friendVM);
                ProcessFriendSelected = null;
                IsFriendForCheck = false;
                await Navigation.PopAsync();
            }
        }

        private async Task search(string str)
        {
            IOrderedEnumerable<FriendViewModel> sortedList;
            var strLower = str.ToLower();

            if (string.IsNullOrEmpty(str))
                sortedList = List.OrderBy(i => i.Name);
            else
                sortedList = List.OrderByDescending(i => i.Name.ToLower().StartsWith(strLower));

            List = new ObservableCollection<FriendViewModel>(sortedList);
        }
    }
}

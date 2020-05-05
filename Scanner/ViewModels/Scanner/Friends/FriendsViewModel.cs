using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
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
            InfoCommand = new AsyncCommand(ShowInfo);
            SearchCommand = new AsyncCommand<string>(Search);
            ItemSelectedCommand = new AsyncCommand<FriendViewModel>(ProcessItemSelected);
            CallInitializeListFromDatabase().Wait();
        }

        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand<string> SearchCommand { get; }
        public IAsyncCommand<FriendViewModel> ItemSelectedCommand { get; }
        #region ForChecks
        //Необходимо для того, чтобы понять, что мы работаем сейчас с чеком. 
        //Каждый раз создавать новый класс друзей затратно.
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
        #endregion

        protected override async Task InitializeListFromDatabase()
        {
            await AsyncDatabase.CreateTableAsync<Friend>();
            var friends = await AsyncDatabase.GetItemsAsync<Friend>();

            List = new ObservableCollection<FriendViewModel>(friends.Select(f => new FriendViewModel(f)));
        }

        protected override async Task Add(FriendViewModel item)
        {
            var friendVM = item.Clone();
            List.Add(friendVM);
            await AsyncDatabase.AddOrReplaceItemAsync(friendVM.Friend);
        }

        protected override async Task Remove(FriendViewModel item)
        {
            List.Remove(item);
            await AsyncDatabase.RemoveItemAsync<Friend>(item.Id);
        }

        private Task ShowInfo()
        {
            //TODO: Добавить описание к странице друзей
            return CurrentPage.DisplayAlert(
                    "",
                    "",
                    "Ок");
        }

        private async Task ProcessItemSelected(FriendViewModel friendVM)
        {
            if (IsFriendForCheck)
            {
                ProcessFriendSelected(friendVM);
                ProcessFriendSelected = null;
                IsFriendForCheck = false;
                await Navigation.PopAsync();
            }
        }

        private async Task Search(string input)
        {
            input = input.ToLower();

            var sortedFriends = string.IsNullOrEmpty(input)
                ? await Task.Run(() => List.OrderBy(i => i.Name))
                : await Task.Run(() => List.OrderByDescending(i => i.Name.ToLower().StartsWith(input)));

            List = new ObservableCollection<FriendViewModel>(sortedFriends);
        }
    }
}

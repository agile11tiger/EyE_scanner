using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.Views.Scanner.Checks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner.Checks
{
    public abstract class ChecksListViewModel : ListViewModel<CheckViewModel>
    {
        protected ChecksListViewModel(CheckTypes type) : base()
        {
            CheckType = type;
            InfoCommand = new AsyncCommand(ShowInfo);
            SearchCommand = new AsyncCommand<DateTime>(Search);
            ToCheckCommand = new AsyncCommand<CheckViewModel>(ToCheck);
            CallInitializeListFromDatabase().Wait();
        }

        public CheckTypes CheckType { get; }
        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand<DateTime> SearchCommand { get; }
        public IAsyncCommand<CheckViewModel> ToCheckCommand { get; }

        protected override async Task InitializeListFromDatabase()
        {
            await AsyncDatabase.CreateTableAsync<Check>();
            var checks = await AsyncDatabase.Db.Table<Check>().Where(c => c.Type == CheckType).ToListAsync();
            checks.ForEach(c => c.Deserialize());

            List = new ObservableCollection<CheckViewModel>(checks.Select(c => new CheckViewModel(c)));
        }

        protected override async Task Add(CheckViewModel item)
        {
            item.Check.Type = CheckType;
            List.Add(item);
            item.Check.Serialize();
            await AsyncDatabase.AddItemAsync(item.Check);
        }

        protected override async Task Remove(CheckViewModel item)
        {
            List.Remove(item);
            await AsyncDatabase.RemoveItemAsync<Check>(item.Check.Id);
        }

        protected virtual Task ShowInfo()
        {
            return Task.FromResult(true);
        }

        protected virtual async Task Search(DateTime date)
        {
            var sortedList = date == default
                ? await Task.Run(() => List.OrderByDescending(i => i.DateTime))
                : await Task.Run(() => List.OrderByDescending(i => i.DateTime >= date));

            List = new ObservableCollection<CheckViewModel>(sortedList);
        }

        protected virtual async Task ToCheck(CheckViewModel checkVM)
        {
            if (checkVM.Check.Type == CheckTypes.CommonCheck)
            {
                var checkPage = new CheckTabbedPage(new CheckEditorViewModel(checkVM));
                await Navigation.PushAsync(checkPage).ConfigureAwait(false);
            }
            else
            {
                var friendVM = Pages.FriendsPage.ViewModel.List.FirstOrDefault(f => f.Id == checkVM.Check.FriendId);
                var friendCheckVM = new FriendCheckViewModel(friendVM, checkVM.Check);
                var checkPage = new CheckPage(friendCheckVM);
                await Navigation.PushAsync(checkPage).ConfigureAwait(false);
            }
        }
    }
}

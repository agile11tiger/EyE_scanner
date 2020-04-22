using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Views.Scanner.Checks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VerificationCheck.Core.Results;

namespace Scanner.ViewModels.Scanner.Checks
{
    public abstract class ChecksListViewModel : ListViewModel<FriendsChecksViewModel>
    {
        protected ChecksListViewModel() : base()
        {
            InfoCommand = new AsyncCommand(ShowInfo);
            SearchCommand = new AsyncCommand<string>(Search);
            ToCheckCommand = new AsyncCommand<FriendsChecksViewModel>(ToCheck);
        }

        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand<string> SearchCommand { get; }
        public IAsyncCommand<FriendsChecksViewModel> ToCheckCommand { get; }

        protected override async Task InitializeListFromDatabase()
        {
            await AsyncDatabase.CreateTableAsync<Check>();
            var checks = await AsyncDatabase.GetItemsAsync<Check>();
            checks.ForEach(c => c.Deserialize());

            List = new ObservableCollection<FriendsChecksViewModel>(checks.Select(c => new FriendsChecksViewModel(c)));

        }

        protected override async Task Add(FriendsChecksViewModel item)
        {
            List.Add(item);
            await AsyncDatabase.AddItemAsync(item.Check);
        }

        protected override async Task Remove(FriendsChecksViewModel item)
        {
            List.RemoveAt(item.Check.Id);
            await AsyncDatabase.RemoveItemAsync<Check>(item.Check.Id);
        }

        private Task ShowInfo()
        {
            //TODO: может лучше как в сканере через календарь
            return CurrentPage.DisplayAlert(
                    "Как правильно использовать поиск?",
                    "1) Введите дату в формате dd.MM.yy, либо её часть.\r\n" +
                    "Например:\r\n" +
                    "18.03.2020\r\n" +
                    "18.03\r\n" +
                    "18\r\n" +
                    "2) Также можно ввести диапозон.\r\n" +
                    "Например:\r\n" +
                    "18.03.2019 18.03.2020\r\n" +
                    "18.03 18.04\r\n" +
                    "18 31\r\n" +
                    "3) Нажмите поиск",
                    "Ок");
        }

        private async Task Search(string searchText)
        {
            //TODO: добавить поиск согласно showInfo
            IOrderedEnumerable<FriendsChecksViewModel> sortedList;

            if (string.IsNullOrEmpty(searchText))
                sortedList = await Task.Run(() => List.OrderByDescending(i => i.DateTime));
            else
                sortedList = await Task.Run(() => List.OrderByDescending(i => i.DateTime.ToString("dd.MM.yy").StartsWith(searchText)));

            List = new ObservableCollection<FriendsChecksViewModel>(sortedList);
        }

        private async Task ToCheck(FriendsChecksViewModel friendsChecksVM)
        {
            //TODO: Убрать(расскометировать(должен получать клон чека из бд))
            //var cloneCheckVM = await AsyncDatabase.GetItemAsync<FriendsChecksViewModel>(friendsChecksVM.Id);
            //cloneCheckVM.Deserialize();
            //var checkPage = new CheckTabbedPage(cloneCheckVM, checksListsVM.AddToMyChecks);

            var checkPage = new CheckTabbedPage(friendsChecksVM);
            await Navigation.PushAsync(checkPage).ConfigureAwait(false);
        }
    }
}

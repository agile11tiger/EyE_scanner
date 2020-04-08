using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Views.Scanner.Checks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner.Checks
{
    public abstract class ChecksListViewModel : ListViewModel<FriendsChecksViewModel>
    {
        public ChecksListViewModel(ChecksListsViewModel checksListsVM) : base()
        {
            this.checksListsVM = checksListsVM;
            InfoCommand = new AsyncCommand(showInfo);
            SearchCommand = new AsyncCommand<string>(search);
            ToCheckCommand = new AsyncCommand<FriendsChecksViewModel>(toCheck);
        }

        private ChecksListsViewModel checksListsVM;
        public IAsyncCommand InfoCommand { get; set; }
        public IAsyncCommand<string> SearchCommand { get; set; }
        public IAsyncCommand<FriendsChecksViewModel> ToCheckCommand { get; set; }

        private Task showInfo()
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

        private async Task search(string searchText)
        {
            //TODO: добавить поиск согласно showInfo
            IOrderedEnumerable<FriendsChecksViewModel> sortedList;

            if (string.IsNullOrEmpty(searchText))
                sortedList = List.OrderByDescending(i => i.DateTime);
            else
                sortedList = List.OrderByDescending(i => i.DateTime.ToString("dd.MM.yy").StartsWith(searchText));

            List = new ObservableCollection<FriendsChecksViewModel>(sortedList);
        }

        private async Task toCheck(FriendsChecksViewModel friendsChecksVM)
        {
            //TODO: Убрать(расскометировать(должен получать клон чека из бд))
            //var cloneCheckVM = await AsyncDatabase.GetItemAsync<FriendsChecksViewModel>(friendsChecksVM.Id);
            //cloneCheckVM.Deserialize();
            //var checkPage = new CheckTabbedPage(cloneCheckVM, checksListsVM.AddToMyChecks);

            var checkPage = new CheckTabbedPage(friendsChecksVM, checksListsVM.AddToMyChecks);
            await Navigation.PushAsync(checkPage).ConfigureAwait(false);
        }
    }
}

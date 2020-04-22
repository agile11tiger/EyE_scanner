using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий со списком действительных чеков 
    /// </summary>
    public class ChecksListsViewModel : BaseViewModel
    {
        public ChecksListsViewModel(ObservableCollection<ChecksListViewModel> checks) : base()
        {
            Checks = checks;
            Checks.Add(new CommonChecksListViewModel() { Title = AppConstants.NAME_PAGE_COMMON_CHECKS });
            Checks.Add(new MyChecksListViewModel() { Title = AppConstants.NAME_PAGE_MY_CHECKS });

            SearchCommand = new AsyncCommand<string>(Search);
            InfoCommand = new AsyncCommand(ShowInfo);
        }

        public ObservableCollection<ChecksListViewModel> Checks { get; }
        public new TabbedPage CurrentPage { get; set; }
        public new INavigation Navigation { get => CurrentPage.Navigation; }
        public IAsyncCommand<string> SearchCommand { get; }
        public IAsyncCommand InfoCommand { get; }

        public Task AddToCommonChecks(FriendsChecksViewModel friendsChecksVM)
        {
            return Checks.First(c => c.Title == AppConstants.NAME_PAGE_COMMON_CHECKS)
                .AddCommand
                .ExecuteAsync(friendsChecksVM);
        }

        public Task AddToMyChecks(FriendsChecksViewModel friendsChecksVM)
        {
            return Checks.First(c => c.Title == AppConstants.NAME_PAGE_MY_CHECKS)
                .AddCommand
                .ExecuteAsync(friendsChecksVM);
        }

        //TODO: Возможно это можно сделать как нибудь в xaml
        private Task Search(string searchText)
        {
            var currentChecksVM = (ChecksListViewModel)CurrentPage.SelectedItem;

            return Checks.First(c => c.Title == currentChecksVM.Title)
                .SearchCommand
                .ExecuteAsync(searchText);
        }

        //TODO: Возможно это можно сделать как нибудь в xaml
        private Task ShowInfo()
        {
            var currentChecksVM = (ChecksListViewModel)CurrentPage.SelectedItem;

            return Checks.First(c => c.Title == currentChecksVM.Title)
                .InfoCommand
                .ExecuteAsync();
        }
    }
}
